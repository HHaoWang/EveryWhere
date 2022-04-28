using AutoMapper;
using EveryWhere.Database.PO;
using EveryWhere.DTO.Entity;
using EveryWhere.MainServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
}