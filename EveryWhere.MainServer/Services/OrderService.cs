using EveryWhere.Database;
using EveryWhere.Database.PO;
using EveryWhere.MainServer.Entity.Exception;
using EveryWhere.MainServer.Entity.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static EveryWhere.Database.PO.Printer;

namespace EveryWhere.MainServer.Services;

public class OrderService:BaseService<Order>
{
    private readonly PrinterService _printerService;

    public OrderService(Repository repository, PrinterService printerService) : base(repository)
    {
        _printerService = printerService;
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
            !.ThenInclude(j => j.File)
            .Include(o => o.Consumer)
            .Include(o => o.Shop)
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

        return order;
    }
}