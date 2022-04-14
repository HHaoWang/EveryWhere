using EveryWhere.Database;
using EveryWhere.MainServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EveryWhere.MainServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShopController : ControllerBase
{
    private readonly Repository _repository;
    private readonly ShopService _shopService;

    public ShopController(Repository repository, ShopService shopService)
    {
        _repository = repository;
        _shopService = shopService;
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
}