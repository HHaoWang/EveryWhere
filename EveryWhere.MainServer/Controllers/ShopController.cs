using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EveryWhere.MainServer.Contexts.Shop.Representation;

namespace EveryWhere.MainServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly ShopRepresentationService _shopRepresentService;

        public ShopController(ShopRepresentationService shopRepresentService)
        {
            _shopRepresentService = shopRepresentService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return new JsonResult(new
            {
                StatusCode = 200,
                Data = _shopRepresentService.GetShops()
            });
        }
    }
}
