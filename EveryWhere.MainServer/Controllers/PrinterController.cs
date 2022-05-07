using AutoMapper;
using EveryWhere.Database.PO;
using EveryWhere.DTO.Entity;
using EveryWhere.MainServer.Entity.Request;
using EveryWhere.MainServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EveryWhere.MainServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PrinterController : ControllerBase
{
    private readonly PrinterService _printerService;
    private readonly PrintJobService _printJobService;
    private readonly ILogger<PrinterController> _logger;

    public PrinterController(ILogger<PrinterController> logger, PrinterService printerService, PrintJobService printJobService)
    {
        _logger = logger;
        _printerService = printerService;
        _printJobService = printJobService;
    }

    /// <summary>
    /// 获取店铺打印机列表
    /// </summary>
    /// <param name="shopId">店铺ID</param>
    /// <returns></returns>
    [HttpGet]
    [Route("Shop/{shopId:int}")]
    [Authorize(Roles = "Shopkeeper")]
    public IActionResult GetByShop(int shopId)
    {
        List<Printer> printers = _printerService.GetQuery(p => p.ShopId == shopId).ToList();
        return new JsonResult(new
        {
            statusCode = 200,
            data = new
            {
                printers
            }
        });
    }

    /// <summary>
    /// 获取指定打印机未完成的打印任务
    /// </summary>
    /// <param name="printerId">打印机ID</param>
    /// <returns>指定打印机未完成的打印任务信息</returns>
    [HttpGet]
    [Route("{printerId:int}/UnfinishedJobs")]
    [Authorize(Roles = "Shopkeeper")]
    public IActionResult GetUnfinishedJobs(int printerId)
    {
        List<PrintJob> jobs = _printJobService
            .GetQuery(p => p.PrinterId == printerId && p.IsFinished != true)
            .Include(j=>j.File)
            .ToList();
        return new JsonResult(new
        {
            statusCode = 200,
            data = new
            {
                jobs
            }
        });
    }

    /// <summary>
    /// 添加打印机
    /// </summary>
    /// <param name="printer">打印机信息</param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = "Shopkeeper")]
    public async Task<IActionResult> AddPrinter([FromBody] NewPrinter printer)
    {
        MapperConfiguration config = new(cfg
            =>
        {
            cfg.CreateMap<NewPrinter.PaperSizePrice, Printer.PaperSizePrice>();
            cfg.CreateMap<NewPrinter, Printer>();
        });

        Printer newPrinter = config.CreateMapper().Map<Printer>(printer);

        int affectedRows = await _printerService.AddAsync(newPrinter);
        if (affectedRows>0)
        {
            return new JsonResult(new
            {
                statusCode = 200
            });
        }

        return new JsonResult(new
        {
            statusCode = 500,
            message = "添加失败！"
        })
        {
            StatusCode = 500
        };
    }

    /// <summary>
    /// 删除打印机
    /// </summary>
    /// <param name="printerId">打印机ID</param>
    /// <returns></returns>
    [HttpDelete]
    [Route("{printerId:int}")]
    [Authorize(Roles = "Shopkeeper")]
    public async Task<IActionResult> DeletePrinter(int printerId)
    {
        int affectedRows = await _printerService.Delete(printerId);
        if (affectedRows > 0)
        {
            return new JsonResult(new
            {
                statusCode = 200
            });
        }

        return new JsonResult(new
        {
            statusCode = 500,
            message = "删除失败！"
        })
        {
            StatusCode = 500
        };
    }

    /// <summary>
    /// 更新打印机
    /// </summary>
    /// <param name="request">更新后的打印机信息</param>
    /// <returns></returns>
    [HttpPatch]
    [Authorize(Roles = "Shopkeeper,Manager")]
    public async Task<IActionResult> Patch(PatchPrinterRequest request)
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(c => c.Type.Equals("UserId", StringComparison.CurrentCultureIgnoreCase))?.Value);
        bool isManager = HttpContext.User.Claims.FirstOrDefault(c =>
            c.ValueType == ClaimTypes.Role && c.Value.Equals("Manager", StringComparison.CurrentCultureIgnoreCase)) != null;

        MapperConfiguration config = new(cfg
            =>
        {
            cfg.CreateMap<PatchPrinterRequest.SizePrice, Printer.PaperSizePrice>();
            cfg.CreateMap<PatchPrinterRequest, Printer>()
                .ForMember(p => p.SupportSizes, opt => opt.Ignore());
        });
        Printer? newPrinter = config.CreateMapper().Map<Printer>(request);

        if (!isManager)
        {
            Printer? printer = await _printerService
                .GetQuery(p => p.Id == request.Id)
                .Include(p => p.Shop)
                .FirstOrDefaultAsync();
            //先看是否有该打印机
            if (printer is null)
            {
                return new JsonResult(new
                {
                    statusCode = 404,
                    message = "未找到符合的打印机！"
                })
                {
                    StatusCode = 404
                };
            }
            //再看是否是当前店铺的打印机
            if (printer.Shop!.ShopKeeperId != userId)
            {
                return new JsonResult(new
                {
                    statusCode = 401,
                    message = "没有权限操作！"
                })
                {
                    StatusCode = 401
                };
            }
        }

        int affectRows = await _printerService.UpdateAsync(newPrinter);
        return new JsonResult(new
        {
            statusCode = affectRows > 0 ? 200 : 500
        })
        {
            StatusCode = affectRows > 0 ? 200 : 500
        };
    }
}