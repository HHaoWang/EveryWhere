using EveryWhere.MainServer.Entity.Dto;
using EveryWhere.MainServer.Entity.Exception;
using EveryWhere.MainServer.Entity.Request;
using EveryWhere.MainServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace EveryWhere.MainServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private IConfiguration _configuration;
    private readonly UserService _userService;
    private readonly IMemoryCache _memoryCache;

    public LoginController(IConfiguration configuration, UserService userService, IMemoryCache memoryCache)
    {
        _configuration = configuration;
        _userService = userService;
        _memoryCache = memoryCache;
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

    [Route("QRCode")]
    [HttpGet]
    public IActionResult GetLoginQrCode()
    {
        string uuid = Guid.NewGuid().ToString();
        DateTime now = DateTime.Now;
        _memoryCache.Set(uuid, new QrCodeCacheInfo()
        {
            CreateTime = now,
            Uuid = uuid,
            State = QrCodeCacheInfo.QrCodeState.Valid,
            ExpireTime = now.AddSeconds(15)
        }, TimeSpan.FromSeconds(60));
        return new JsonResult(new
        {
            statusCode = 200,
            data = new
            {
                uuid
            }
        });
    }

    [Route("QRCode/{uuid}/Valid")]
    [HttpGet]
    public async Task<IActionResult> ValidateQrCodeAsync(string uuid)
    {
        if (!_memoryCache.TryGetValue(uuid, out QrCodeCacheInfo qrInfo))
        {
            return new JsonResult(new
            {
                statusCode = 404,
                message = "令牌不存在！"
            });
        }

        if (qrInfo.State == QrCodeCacheInfo.QrCodeState.Valid)
        {
            return new JsonResult(new
            {
                statusCode = 400,
                message = "尚未登录！"
            });
        }

        if (qrInfo.ExpireTime > DateTime.Now || qrInfo.State == QrCodeCacheInfo.QrCodeState.Invalid)
        {
            return new JsonResult(new
            {
                statusCode = 400,
                message = "二维码已过期！"
            });
        }

        if (qrInfo.State != QrCodeCacheInfo.QrCodeState.HasLogin || qrInfo.UserId == null)
        {
            return new JsonResult(new
            {
                statusCode = 404,
                message = "令牌不存在！"
            });
        }

        string token = await _userService.GetTokenByQrCodeAsync(qrInfo);
        return new JsonResult(new
        {
            statusCode = 200,
            data = new
            {
                token
            }
        });
    }

    [Route("Valid/Shopkeeper")]
    [HttpGet]
    [Authorize(Roles = "Shopkeeper")]
    public IActionResult HasLoginAsShopkeeper()
    {
        return new JsonResult(new { statusCode = 200, data = new { key = "value" } });
    }

    [Route("Valid/Manager")]
    [HttpGet]
    [Authorize(Roles = "Manager")]
    public IActionResult HasLoginAsManager()
    {
        return new JsonResult(new { statusCode = 200, data = new { key = "value" } });
    }

    [Route("QrCode/{uuid}")]
    [HttpGet]
    [Authorize(Roles = "Consumer")]
    public IActionResult QrCodeLogin(string uuid)
    {
        int uploaderId = Convert.ToInt32(HttpContext.User.FindFirst(c => c.Type.Equals("UserId", StringComparison.CurrentCultureIgnoreCase))?.Value);

        if (!_memoryCache.TryGetValue(uuid, out QrCodeCacheInfo qrInfo))
        {
            return new JsonResult(new
            {
                statusCode = 404,
                message = "令牌不存在！"
            });
        }

        qrInfo.UserId = uploaderId;
        qrInfo.State = QrCodeCacheInfo.QrCodeState.HasLogin;

        _memoryCache.Set(uuid, qrInfo, TimeSpan.FromTicks(20));

        return new JsonResult(new { statusCode = 200, messsage="登录成功！" });
    }
}