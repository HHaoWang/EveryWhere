using EveryWhere.MainServer.Contexts.Order;
using EveryWhere.MainServer.Contexts.Order.Representation;
using EveryWhere.MainServer.Controllers.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EveryWhere.MainServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderApplicationService _orderApplicationService;
        private readonly OrderRepresentationService _orderRepresentationService;

        public OrderController(OrderApplicationService orderApplicationService, OrderRepresentationService orderRepresentationService)
        {
            _orderApplicationService = orderApplicationService;
            _orderRepresentationService = orderRepresentationService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return new JsonResult(new
            {
                StatusCode = 200,
                Data = _orderRepresentationService.GetOrders()
            });
        }

        [HttpPost]
        public IActionResult CreateOrder([Required]CreateOrderRequest request)
        {
            try
            {
                int orderId = _orderApplicationService.CreateOrder(request.ShopId);
                return new JsonResult(new
                {
                    StatusCode = 200,
                    Data = new
                    {
                        orderId = orderId
                    }
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
