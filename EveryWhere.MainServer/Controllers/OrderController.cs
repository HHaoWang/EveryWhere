using EveryWhere.Database.PO;
using EveryWhere.MainServer.Entity.Exception;
using EveryWhere.MainServer.Entity.Request;
using EveryWhere.MainServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EveryWhere.MainServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(OrderService orderService, ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// 根据ID获取订单
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        return new JsonResult(new
        {
            statusCode = 200,
            data = new
            {
                order = _orderService.GetQuery()
                    .Include(o=>o.PrintJobs)
                    !.ThenInclude(j=>j.File)
                    .Include(o=>o.Consumer)
                    .Include(o => o.Shop)
                    .ToList()
                    .FirstOrDefault(o => o.Id == id)
            }
        });
    }

    /// <summary>
    /// 根据用户ID获取订单(仅管理员)
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns></returns>
    [HttpGet("User/{id:int}")]
    [Authorize(Roles = "Manager")]
    public IActionResult GetUserOrders(int id)
    {
        return new JsonResult(new
        {
            statusCode = 200,
            data = new
            {
                orders = _orderService.GetQuery()
                    .Include(o => o.PrintJobs)
                    !.ThenInclude(j => j.File)
                    .Include(o => o.Consumer)
                    .Include(o=>o.Shop)
                    .Where(o => o.ConsumerId == id)
                    .ToList()
            }
        });
    }

    /// <summary>
    /// 获取当前登录用户订单
    /// </summary>
    /// <returns></returns>
    [HttpGet("User")]
    [Authorize(Roles = "Consumer")]
    public IActionResult GetUserOrders()
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(c => c.Type.Equals("UserId", StringComparison.CurrentCultureIgnoreCase))?.Value);
        return new JsonResult(new
        {
            statusCode = 200,
            data = new
            {
                orders = _orderService.GetQuery()
                    .Include(o => o.PrintJobs)
                    !.ThenInclude(j => j.File)
                    .Include(o => o.Consumer)
                    .Include(o => o.Shop)
                    .Where(o => o.ConsumerId == userId)
                    .ToList()
            }
        });
    }

    /// <summary>
    /// 根据店铺ID获取订单(仅管理员和商家)
    /// </summary>
    /// <param name="shopId">店铺ID</param>
    /// <returns></returns>
    [HttpGet("Shop/{shopId:int}")]
    [Authorize(Roles = "Manager,Shopkeeper")]
    public IActionResult GetShopOrders(int shopId)
    {
        return new JsonResult(new
        {
            statusCode = 200,
            data = new
            {
                orders = _orderService.GetQuery()
                    .Include(o => o.PrintJobs)
                    !.ThenInclude(j => j.File)
                    .Include(o => o.Consumer)
                    .Where(o => o.ShopId == shopId)
                    .ToList()
            }
        });
    }

    /// <summary>
    /// 生成订单
    /// </summary>
    /// <param name="request">订单信息</param>
    [HttpPost]
    [Authorize(Roles = "Consumer")]
    public async Task<IActionResult> Post([FromBody] PostOrderRequest request)
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(c => c.Type.Equals("UserId", StringComparison.CurrentCultureIgnoreCase))?.Value);
        try
        {
            Order order = await _orderService.GenerateOrder(request, userId);
            return new JsonResult(new
            {
                statusCode = 200,
                data = new
                {
                    order
                }
            });
        }
        catch (EntityNotFoundException e)
        {
            return new JsonResult(new
            {
                statusCode = 404,
                e.Message
            })
            {
                StatusCode = 404
            };
        }
        catch (PrinterNotSupportTicketException e)
        {
            return new JsonResult(new
            {
                statusCode = 404,
                e.Message
            })
            {
                StatusCode = 404
            };
        }
        catch (AddEntityFailedException e)
        {
            _logger.LogError(e.Message);
            return new JsonResult(new
            {
                statusCode = 500,
                message = e.Message
            })
            {
                StatusCode = 500
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return new JsonResult(new
            {
                statusCode = 500,
                message = "服务器内部出现错误！"
            })
            {
                StatusCode = 500
            };
        }
    }
    
    /// <summary>
    /// 支付订单
    /// </summary>
    /// <param name="id">订单ID</param>
    /// <returns></returns>
    [HttpPost("{id}/Pay")]
    [Authorize(Roles = "Consumer")]
    public async Task<IActionResult> Pay(int id)
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(c => c.Type.Equals("UserId", StringComparison.CurrentCultureIgnoreCase))?.Value);
        try
        {
            Order order = await _orderService.PayOrder(id, userId);
            return new JsonResult(new
            {
                statusCode = 200,
                data = new
                {
                    order
                }
            });
        }
        catch (EntityNotFoundException e)
        {
            return new JsonResult(new
            {
                statusCode = 404,
                e.Message
            })
            {
                StatusCode = 404
            };
        }
        catch (UpdateEntityException e)
        {
            return new JsonResult(new
            {
                statusCode = 404,
                e.Message
            })
            {
                StatusCode = 404
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return new JsonResult(new
            {
                statusCode = 500,
                message = "服务器内部出现错误！"
            })
            {
                StatusCode = 500
            };
        }
        
    }

    /// <summary>
    /// 根据打印设置计算打印金额
    /// </summary>
    /// <param name="ticket">打印设置</param>
    /// <returns>以分为单位的金额</returns>
    [HttpPost("Job/Calculate")]
    public async Task<IActionResult> CalculateJob([FromBody]PrintTicketRequest ticket)
    {
        try
        {
            return new JsonResult(new
            {
                statusCode = 200,
                data = new
                {
                    totalPrice = await _orderService.CalculateJob(ticket)*100
                }
            });
        }
        catch (EntityNotFoundException e)
        {
            return new JsonResult(new
            {
                statusCode = 404,
                e.Message
            })
            {
                StatusCode = 404
            };
        }
        catch (PrinterNotSupportTicketException e)
        {
            return new JsonResult(new
            {
                statusCode = 404,
                e.Message
            })
            {
                StatusCode = 404
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return new JsonResult(new
            {
                statusCode = 500,
                message = "服务器内部错误！"
            })
            {
                StatusCode = 500
            };
        }
    }

    /// <summary>
    /// 完成打印任务(仅商家)
    /// </summary>
    /// <param name="jobId">打印任务ID</param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Roles = "Shopkeeper")]
    [Route("PrintJob/{jobId:int}/Finish")]
    public async Task<IActionResult> FinishPrintJob(int jobId)
    {
        try
        {
            Order order = await _orderService.FinishPrintJob(jobId);
            return new JsonResult(new
            {
                statusCode = 200,
                data = new
                {
                    order
                }
            });
        }
        catch (EntityNotFoundException e)
        {
            _logger.LogError(e.Message);
            return new JsonResult(new
            {
                statusCode = 500,
                message = "服务器错误"+e.Message
            })
            {
                StatusCode = 500
            };
        }
    }
}