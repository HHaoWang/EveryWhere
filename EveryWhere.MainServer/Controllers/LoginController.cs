using EveryWhere.MainServer.Entity.Exception;
using EveryWhere.MainServer.Entity.Request;
using EveryWhere.MainServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EveryWhere.MainServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private IConfiguration _configuration;
    private readonly UserService _userService;

    public LoginController(IConfiguration configuration, UserService userService)
    {
        _configuration = configuration;
        _userService = userService;
    }

    [HttpPost]
    public async Task<ActionResult> LoginAsync(LoginRequest request)
    {
        try
        {
            string token = await _userService.GetAuthenticatedTokenAsync(request);
            return new JsonResult(new { statusCode = 200, data = new { token } });
        }
        catch (RequestWechatOpenIdException exception)
        {
            return StatusCode(500, new JsonResult(new { statusCode = 500, errorMsg = exception.Message }));
        }
        catch (NoNecessaryParameterException exception)
        {
            ModelStateDictionary d = new();
            d.AddModelError(exception.RequiredParameter, "required");
            return ValidationProblem(new ValidationProblemDetails(d));
        }
    }

    [Route("Valid")]
    [HttpGet]
    [Authorize(Roles = "Consumer")]
    public IActionResult HasLogin()
    {
        return new JsonResult(new { statusCode = 200, data = new { key = "value" } });
    }


}