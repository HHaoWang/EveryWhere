using System.Security.Claims;
using AutoMapper;
using EveryWhere.Database.PO;
using EveryWhere.MainServer.Entity.Request;
using EveryWhere.MainServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EveryWhere.MainServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShopController : ControllerBase
{
    private readonly ShopService _shopService;
    private readonly ILogger<ShopController> _logger;

    public ShopController(ShopService shopService, ILogger<ShopController> logger)
    {
        _shopService = shopService;
        _logger = logger;
    }

    [HttpGet]
    [Route("Area/{areaCode}")]
    public IActionResult GetShopsByArea(string areaCode)
    {
        return new JsonResult(new
        {
            statusCode = 200,
            shopList = _shopService.GetShopsByAreaCode(areaCode)
        });
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        return new JsonResult(new
        {
            statusCode = 200,
            shop = await _shopService.GetQuery(s=>s.Id==id)
                .Include(s=>s.Printers)
                .Include(s=>s.Area)
                .Include(s=>s.Shopkeeper)
                .FirstOrDefaultAsync()
        });
    }

    [HttpGet]
    [Route("Shopkeeper")]
    [Authorize(Roles = "Shopkeeper")]
    public async Task<IActionResult> GetOwnShop()
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(c => c.Type.Equals("UserId", StringComparison.CurrentCultureIgnoreCase))?.Value);
        Shop? shop = await _shopService.GetAsync(s=>s.ShopKeeperId == userId);
        return new JsonResult(new
        {
            statusCode = shop==null?404:200,
            data = new
            {
                shop
            }
        });
    }

    [HttpPost]
    [Authorize(Roles = "Shopkeeper")]
    public async Task<IActionResult> Post(PostShopRequest request)
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(c => c.Type.Equals("UserId", StringComparison.CurrentCultureIgnoreCase))?.Value);
        MapperConfiguration config = new(cfg 
            => cfg.CreateMap<PostShopRequest, Shop>());
        Shop? newShop = config.CreateMapper().Map<Shop>(request);
        newShop.ShopKeeperId = userId;
        newShop.IsOpening = false;
        int affectRows = await _shopService.AddAsync(newShop);
        return new JsonResult(new
        {
            statusCode = affectRows>0?200:500
        });
    }

    [HttpPatch]
    [Authorize(Roles = "Shopkeeper,Manager")]
    public async Task<IActionResult> Patch(PatchShopRequest request)
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(c => c.Type.Equals("UserId", StringComparison.CurrentCultureIgnoreCase))?.Value);
        bool isManager = HttpContext.User.Claims.FirstOrDefault(c =>
            c.ValueType == ClaimTypes.Role && c.Value.Equals("Manager", StringComparison.CurrentCultureIgnoreCase)) != null;
        MapperConfiguration config = new(cfg
            => cfg.CreateMap<PatchShopRequest, Shop>());
        Shop? newShop = config.CreateMapper().Map<Shop>(request);

        if (!isManager)
        {
            Shop? userShop = await _shopService.GetAsync(s => s.ShopKeeperId == userId);
            if (userShop == null)
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

        int affectRows = await _shopService.UpdateAsync(newShop);
        return new JsonResult(new
        {
            statusCode = affectRows > 0 ? 200 : 500
        });
    }

    /// <summary>
    /// 管理端获取商家信息
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Roles = "Manager")]
    public IActionResult GetShops()
    {
        List<Shop> shops = _shopService.GetQuery()
            .Include(s => s.Printers)
            .Include(s => s.Area)
            .ThenInclude(a => a!.ParentArea)
            .ThenInclude(a => a!.ParentArea)
            .ToList();

        return new JsonResult(new
        {
            statusCode = 200,
            data = new
            {
                shops
            }
        });
    }
}