using System.Diagnostics.CodeAnalysis;
using EveryWhere.Database.PO;
using EveryWhere.MainServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EveryWhere.MainServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StatisticsController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly ShopService _shopService;
    private readonly ShopViewService _shopViewService;

    public StatisticsController(OrderService orderService, ShopService shopService, ShopViewService shopViewService)
    {
        _orderService = orderService;
        _shopService = shopService;
        _shopViewService = shopViewService;
    }

    /// <summary>
    /// 浏览店铺
    /// </summary>
    /// <param name="id">浏览的店铺</param>
    /// <returns></returns>
    [HttpGet]
    [Route("Shop/{id:int}/Visit")]
    public async Task<IActionResult> VisitShop(int id)
    {
        await _shopViewService.VisitShop(id);
        return Ok();
    }

    /// <summary>
    /// 获取店铺数据统计信息
    /// </summary>
    /// <param name="fromTime">统计起始日期</param>
    /// <param name="toTime">统计结束日期</param>
    /// <returns></returns>
    [HttpGet]
    [Route("Shop/From/{fromTime:datetime}/To/{toTime:datetime}")]
    [Authorize(Roles = "Shopkeeper")]
    public async Task<IActionResult> ShopStatistics([FromRoute]DateTime fromTime, [FromRoute] DateTime toTime)
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(c => c.Type.Equals("UserId", StringComparison.CurrentCultureIgnoreCase))?.Value);
        Shop? shop = await _shopService.GetQuery(s => s.ShopKeeperId == userId)
            .FirstOrDefaultAsync();
        if (shop is null)
        {
            return new JsonResult(new
            {
                statusCode = 404,
                message = "未找到店铺！"
            })
            {
                StatusCode = 404
            };
        }

        int shopId = shop.Id;
        DateOnly from = DateOnly.FromDateTime(fromTime);
        DateOnly to = DateOnly.FromDateTime(toTime.AddDays(1));

        List<object> statisticsData = new();

        IQueryable<Order> orders = _orderService
            .GetQuery(o => o.ShopId == shopId
                           && o.CreateTime!.Value <= to.ToDateTime(new TimeOnly(0,0))
                           && o.CreateTime!.Value >= from.ToDateTime(new TimeOnly(0, 0)))
            .Include(o => o.PrintJobs);

        #region 订单相关数据

        var orderData = orders
            .GroupBy(o => DateOnly.FromDateTime(o.CreateTime!.Value))
            .Select(o => new
            {
                date = o.Key,
                totalCount = o.Count(),
                totalIncome = o.Sum(order => order.Price)!.Value
            })
            .ToList();

        for (DateOnly day = from; day < to; day = day.AddDays(1))
        {
            if (!orderData.Exists(d => d.date.Equals(day)))
            {
                orderData.Add(new
                {
                    date = day,
                    totalCount = 0,
                    totalIncome = 0m
                });
            }
        }

        orderData.Sort((a, b) => a.date.CompareTo(b.date));

        statisticsData.Add(new DateData<int>
        {
            Name = "订单量",
            Data = orderData.Select(d => d.totalCount).ToList()
        });
        statisticsData.Add(new DateData<decimal>
        {
            Name = "总收入",
            Data = orderData.Select(d => d.totalIncome).ToList()
        });

        #endregion

        #region 打印任务相关数据

        var jobData = orders.SelectMany(o => o.PrintJobs!)
            .GroupBy(j => DateOnly.FromDateTime(j.CreateTime!.Value))
            .Select(j => new
            {
                date = j.Key,
                totalPages = j.Sum(job => (job.PageEnd - job.PageStart + 1) * job.Count)!.Value,
                totalBlackPages = j
                    .Where(job => job.Color != true)
                    .Sum(job => (job.PageEnd - job.PageStart + 1) * job.Count)!.Value,
                totalColorPages = j
                    .Where(job => job.Color == true)
                    .Sum(job => (job.PageEnd - job.PageStart + 1) * job.Count)!.Value,
            })
            .ToList();

        for (DateOnly day = from; day < to; day = day.AddDays(1))
        {
            if (!jobData.Exists(j=>j.date.Equals(day)))
            {
                jobData.Add(new
                {
                    date = day,
                    totalPages = 0,
                    totalBlackPages = 0,
                    totalColorPages = 0,
                });
            }
        }

        jobData.Sort((a, b) => a.date.CompareTo(b.date));
        statisticsData.Add(new DateData<int>
        {
            Name = "黑白打印页数",
            Data = jobData.Select(j=>j.totalBlackPages).ToList()
        });
        statisticsData.Add(new DateData<int>
        {
            Name = "彩色打印页数",
            Data = jobData.Select(j => j.totalColorPages).ToList()
        });
        statisticsData.Add(new DateData<int>
        {
            Name = "总打印页数",
            Data = jobData.Select(j => j.totalPages).ToList()
        });

        #endregion

        #region 店铺浏览量

        var shopViewData = _shopViewService
            .GetQuery(view => view.ShopId == shopId
                              && view.CreateTime <= to.ToDateTime(new TimeOnly(0, 0))
                              && view.CreateTime >= from.ToDateTime(new TimeOnly(0, 0)))
            .GroupBy(view => DateOnly.FromDateTime(view.CreateTime))
            .Select(v => new
            {
                date = v.Key,
                viewCount = v.Count()
            })
            .ToList();

        for (DateOnly day = from; day < to; day = day.AddDays(1))
        {
            if (!shopViewData.Exists(j => j.date.Equals(day)))
            {
                shopViewData.Add(new
                {
                    date = day,
                    viewCount = 0
                });
            }
        }

        shopViewData.Sort((a, b) => a.date.CompareTo(b.date));
        statisticsData.Add(new DateData<int>()
        {
            Name = "浏览量",
            Data = shopViewData.Select(v=>v.viewCount).ToList()
        });

        #endregion

        List<DateOnly> days = new();
        for (DateOnly day = from; day < to; day = day.AddDays(1))
        {
            days.Add(day);
        }

        return new JsonResult(new
        {
            statusCode = 200,
            data = new
            {
                days,
                data = statisticsData
            }
        });
    }

    /// <summary>
    /// 获取平台数据统计信息
    /// </summary>
    /// <param name="fromTime">统计起始日期</param>
    /// <param name="toTime">统计结束日期</param>
    /// <returns></returns>
    [HttpGet]
    [Route("System/From/{fromTime:datetime}/To/{toTime:datetime}")]
    [Authorize(Roles = "Manager")]
    public IActionResult SystemStatistics([FromRoute] DateTime fromTime, [FromRoute] DateTime toTime)
    {
        DateOnly from = DateOnly.FromDateTime(fromTime);
        DateOnly to = DateOnly.FromDateTime(toTime.AddDays(1));

        List<object> statisticsData = new();

        IQueryable<Order> orders = _orderService
            .GetQuery(o => o.CreateTime!.Value <= to.ToDateTime(new TimeOnly(0, 0))
                           && o.CreateTime!.Value >= from.ToDateTime(new TimeOnly(0, 0)))
            .Include(o => o.PrintJobs);

        #region 订单相关数据

        var orderData = orders
            .GroupBy(o => DateOnly.FromDateTime(o.CreateTime!.Value))
            .Select(o => new
            {
                date = o.Key,
                totalCount = o.Count(),
                totalIncome = o.Sum(order => order.Price)!.Value
            })
            .ToList();

        for (DateOnly day = from; day < to; day = day.AddDays(1))
        {
            if (!orderData.Exists(d => d.date.Equals(day)))
            {
                orderData.Add(new
                {
                    date = day,
                    totalCount = 0,
                    totalIncome = 0m
                });
            }
        }

        orderData.Sort((a, b) => a.date.CompareTo(b.date));

        statisticsData.Add(new DateData<int>
        {
            Name = "订单量",
            Data = orderData.Select(d => d.totalCount).ToList()
        });
        statisticsData.Add(new DateData<decimal>
        {
            Name = "交易额",
            Data = orderData.Select(d => d.totalIncome).ToList()
        });

        #endregion

        #region 打印任务相关数据

        var jobData = orders.SelectMany(o => o.PrintJobs!)
            .GroupBy(j => DateOnly.FromDateTime(j.CreateTime!.Value))
            .Select(j => new
            {
                date = j.Key,
                totalPages = j.Sum(job => (job.PageEnd - job.PageStart + 1) * job.Count)!.Value,
                totalCount = j.Count()
            })
            .ToList();

        for (DateOnly day = from; day < to; day = day.AddDays(1))
        {
            if (!jobData.Exists(j => j.date.Equals(day)))
            {
                jobData.Add(new
                {
                    date = day,
                    totalPages = 0,
                    totalCount = 0
                });
            }
        }

        jobData.Sort((a, b) => a.date.CompareTo(b.date));
        statisticsData.Add(new DateData<int>
        {
            Name = "打印页数",
            Data = jobData.Select(j => j.totalPages).ToList()
        });
        statisticsData.Add(new DateData<int>
        {
            Name = "打印文档数",
            Data = jobData.Select(j => j.totalCount).ToList()
        });

        #endregion

        #region 店铺浏览量

        var shopViewData = _shopViewService
            .GetQuery(view => view.CreateTime <= to.ToDateTime(new TimeOnly(0, 0))
                              && view.CreateTime >= from.ToDateTime(new TimeOnly(0, 0)))
            .GroupBy(view => DateOnly.FromDateTime(view.CreateTime))
            .Select(v => new
            {
                date = v.Key,
                viewCount = v.Count()
            })
            .ToList();

        for (DateOnly day = from; day < to; day = day.AddDays(1))
        {
            if (!shopViewData.Exists(j => j.date.Equals(day)))
            {
                shopViewData.Add(new
                {
                    date = day,
                    viewCount = 0
                });
            }
        }

        shopViewData.Sort((a, b) => a.date.CompareTo(b.date));
        statisticsData.Add(new DateData<int>()
        {
            Name = "访问量",
            Data = shopViewData.Select(v => v.viewCount).ToList()
        });

        #endregion

        List<DateOnly> days = new();
        for (DateOnly day = from; day < to; day = day.AddDays(1))
        {
            days.Add(day);
        }

        return new JsonResult(new
        {
            statusCode = 200,
            data = new
            {
                days,
                data = statisticsData
            }
        });
    }

    // ReSharper disable once MemberCanBePrivate.Global
    #pragma warning disable IDE0079 // 请删除不必要的忽略
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    #pragma warning restore IDE0079 // 请删除不必要的忽略
    public struct DateData<T>
    {
        public string Name { get; set; }
        public List<T> Data { get; set; }
    }
}