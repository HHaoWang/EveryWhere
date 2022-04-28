using EveryWhere.Database;
using EveryWhere.Database.PO;
using EveryWhere.MainServer.Entity.Dto;
using EveryWhere.MainServer.Entity.Exception;
using EveryWhere.MainServer.Entity.Request;
using EveryWhere.MainServer.Entity.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Net;
using EveryWhere.MainServer.Infrastructure.Websocket;
using static EveryWhere.Database.PO.Printer;

namespace EveryWhere.MainServer.Services;

public class OrderService:BaseService<Order>
{
    private readonly PrinterService _printerService;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<ShopService> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly Repository _repository;
    private readonly Hub _hub;

    public OrderService(Repository repository, PrinterService printerService, IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<ShopService> logger, IMemoryCache memoryCache, Hub hub) : base(repository)
    {
        _repository = repository;
        _printerService = printerService;
        _configuration = configuration;
        _clientFactory = clientFactory;
        _logger = logger;
        _memoryCache = memoryCache;
        _hub = hub;
    }


    public async Task<Order> GenerateOrder(PostOrderRequest request,int consumerId)
    {
        if (request.PrintTickets == null)
        {
            throw new NoNecessaryParameterException("PrintTickets");
        }

        List<PrintJob> printJobs = new();
        decimal orderPrice = 0;

        //计算订单价格
        foreach (PrintTicketRequest ticket in request.PrintTickets??new List<PrintTicketRequest>())
        {
            decimal ticketPrice = await CalculateJob(ticket);
            orderPrice += ticketPrice;
            printJobs.Add(new PrintJob
            {
                FileId = ticket.FileId,
                IsFinished = false,
                PrinterId = ticket.PrinterId,
                Color = ticket.Color,
                Count = ticket.Count,
                Duplex = ticket.Duplex,
                PageStart = ticket.PagesStart,
                PageEnd = ticket.PagesEnd,
                PageSize = ticket.Size,
                Price = ticketPrice
            });
        }

        //订单数据持久化
        Order order = new()
        {
            ConsumerId = consumerId,
            ShopId = request.ShopId,
            State = Order.OrderState.UnPaid,
            Price = orderPrice,
            PrintJobs = printJobs
        };

        int affectedRows = await AddAsync(order);
        if (affectedRows<=0)
        {
            throw new AddEntityFailedException("订单");
        }

        return order;
    }

    /// <summary>
    /// 计算打印配置的价格
    /// </summary>
    /// <param name="ticket">打印配置</param>
    /// <returns>价格，单位：元</returns>
    /// <exception cref="EntityNotFoundException">没有对应的打印机</exception>
    /// <exception cref="PrinterNotSupportTicketException">打印机不支持打印该配置</exception>
    public async Task<decimal> CalculateJob(PrintTicketRequest ticket)
    {
        //检查是否有对应打印机
        Printer? printer = await _printerService.GetByIdAsync(ticket.PrinterId);
        if (printer == null)
        {
            throw new EntityNotFoundException("打印机",ticket.PrinterId);
        }

        //检查打印机是否支持打印选型
        if (ticket.Color && printer.SupportColor != true)
        {
            throw new PrinterNotSupportTicketException(ticket.PrinterId, "彩色");
        }

        if (ticket.Duplex && printer.SupportDuplex!=true)
        {
            throw new PrinterNotSupportTicketException(ticket.PrinterId, "双面打印");
        }

        bool exist = printer.SupportSizes.TryGetValue(ticket.Size!, out PaperSizePrice sizePrices);

        if (!exist)
        {
            throw new PrinterNotSupportTicketException(ticket.PrinterId, ticket.Size + "纸张大小");
        }

        //计算总价
        int totalPagesCount = ticket.PagesEnd - ticket.PagesStart + 1;

        decimal singlePagePrice;

        if (ticket.Color)
        {
            singlePagePrice = ticket.Duplex ? sizePrices.DuplexColor : sizePrices.SingleColor;
        }
        else
        {
            singlePagePrice = ticket.Duplex ? sizePrices.DuplexBlack : sizePrices.SingleBlack;
        }

        decimal totalPrice = singlePagePrice * totalPagesCount * ticket.Count;

        return totalPrice;
    }

    public async Task<Order> PayOrder(int orderId, int userId)
    {
        Order? order = GetQuery(noTracking: false)
            .Include(o => o.PrintJobs)
            !.ThenInclude(j => j.Printer)
            .ToList()
            .FirstOrDefault(o => o.Id == orderId);

        if (order == null)
        {
            throw new EntityNotFoundException("订单", orderId);
        }

        order.State = Order.OrderState.Printing;
        order.PayTime = DateTime.Now;

        int affectRows = await Repository.SaveChangesAsync();

        if (affectRows<=0)
        {
            throw new UpdateEntityException("订单", orderId);
        }

        foreach (PrintJob job in order.PrintJobs!)
        {
            await _hub.NotifyPrinter(job.Printer?.ComputerId ?? "", job.Id);
        }

        return order;
    }

    public async Task<Order> FinishPrintJob(int jobId)
    {
        PrintJob? job = await Repository.PrintJobs!
            .FirstOrDefaultAsync(j=>j.Id == jobId);
        if (job == null)
        {
            throw new EntityNotFoundException("打印任务", jobId);
        }

        DateTime now = DateTime.Now;

        string fetchCode = $"{now.Hour + 10}{now.Minute + 24}{now.Second + 36}";

        job.IsFinished = true;
        job.FetchCode = fetchCode;

        Order? order = await _repository.Orders!
            .Include(o=>o.PrintJobs)
            .Include(o=>o.Consumer)
            .Include(o=>o.Shop)
            .FirstOrDefaultAsync(o => o.Id == job.OrderId);

        bool allFinished = true;
        foreach (PrintJob printJob in order!.PrintJobs!)
        {
            if (printJob.IsFinished != true)
            {
                allFinished = false;
            }
        }

        if (allFinished)
        {
            order.State = Order.OrderState.Finished;
            order.CompleteTime = DateTime.Now;
        }
        

        await Repository.SaveChangesAsync();

        await PushNotification(order!.Consumer!.WechatOpenId ?? "", order, job);

        return order;
    }

    /// <summary>
    /// 推送打印任务状态消息到微信
    /// </summary>
    /// <param name="targetUserOpenId">目标用户的OpenId</param>
    /// <param name="order">订单</param>
    /// <param name="printJob">打印任务</param>
    /// <returns>推送结果</returns>
    public async Task<bool> PushNotification(string targetUserOpenId, Order order, PrintJob printJob)
    {
        string requestUrl =
            $"https://api.weixin.qq.com/cgi-bin/message/subscribe/send?access_token={await GetAccessToken()}";
        HttpClient? client = _clientFactory.CreateClient();
        var content = new
        {
            touser = targetUserOpenId,
            template_id = "f_qfrBzThpIfK8WwdSHymugm_PU63Ejc60THzfD_Hpg",
            page = "/pages/order/order?id=" + order.Id,
            miniprogram_state = _configuration["pushWechatCardOpenClientType"],
            data = new
            {
                name3 = new
                {
                    value = order.Shop!.Name
                },
                thing2 = new
                {
                    value = order.Shop!.Address
                },
                phrase4 = new
                {
                    value = printJob.IsFinished == true ? "已打印" : "打印中"
                },
                time5 = new
                {
                    value = printJob.CreateTime!.Value.ToString("HH:mm")
                },
                number12 = new
                {
                    value = Convert.ToInt32(printJob.FetchCode)
                }
            }
        };
        string contentStr = JsonConvert.SerializeObject(content);
        _logger.LogInformation("推送内容：\n"+ contentStr);
        HttpResponseMessage result = await client.PostAsync(requestUrl, new StringContent(contentStr));
        _logger.LogError("推送了打印任务状态消息，返回内容为：" + await result.Content.ReadAsStringAsync());
        if (result.StatusCode == HttpStatusCode.OK)
        {
            return true;
        }
        _logger.LogError("推送打印任务状态消息失败！");
        return false;
    }

    /// <summary>
    /// 从缓存中获取AccessToken
    /// </summary>
    /// <returns>AccessToken</returns>
    private async Task<string> GetAccessToken()
    {
        if (!_memoryCache.TryGetValue("wechatAccessToken", out WechatAccessTokenCacheSetting setting))
        {
            bool successGotToken = await GetAccessTokenFromWechat();
            if (!successGotToken)
            {
                return "";
            }

            return await GetAccessToken();
        }

        if (setting.ExpiresTime < DateTime.Now)
        {
            _memoryCache.Remove("wechatAccessToken");
            return await GetAccessToken();
        }

        return setting.AccessToken ?? "";
    }

    /// <summary>
    /// 从微信服务获取AccessToken并缓存
    /// </summary>
    /// <returns>获取结果</returns>
    private async Task<bool> GetAccessTokenFromWechat()
    {
        string requestUrl =
            $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={_configuration["appId"]}&secret={_configuration["appSecret"]}";

        HttpClient? client = _clientFactory.CreateClient();
        HttpResponseMessage result = await client.GetAsync(requestUrl);
        string content = await result.Content.ReadAsStringAsync();
        WechatAccessTokenResponse? response = JsonConvert.DeserializeObject<WechatAccessTokenResponse>(content);

        if (response == null || response.ErrorCode != null && response.ErrorCode!=0)
        {
            _logger.LogError("获取微信AccessToken失败！");
            return false;
        }

        //设置缓存，提前5分钟过期
        _memoryCache.Set("wechatAccessToken", new WechatAccessTokenCacheSetting()
        {
            AccessToken = response.AccessToken,
            ExpiresTime = DateTime.Now.AddSeconds(response.ValidSeconds)
        }, TimeSpan.FromSeconds(response.ValidSeconds - 300));
        return true;
    }
}