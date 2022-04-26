using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace EveryWhere.MainServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WechatController : ControllerBase
{
    private readonly ILogger<WechatController> _logger;

    public WechatController(ILogger<WechatController> logger)
    {
        _logger = logger;
    }

    [Route("OnReceivedMessage")]
    [HttpGet]
    public string MessagePushValidate([FromQuery] string signature, [FromQuery] string timestamp, [FromQuery] string nonce, [FromQuery] string echostr)
    {
        const string token = "everywhere";
        List<string> stringList = new()
        {
            token,
            timestamp,
            nonce
        };
        stringList.Sort();
        string s = stringList.Aggregate((workingSentence, next) =>
            workingSentence + next);

        _logger.LogInformation(s);

        s = SHA1_Encrypt(s);

        _logger.LogInformation(s);

        return s == signature ? echostr : "";
    }

    /// <summary>
    /// 对字符串进行SHA1加密
    /// </summary>
    /// <param name="sourceString">需要加密的字符串</param>
    /// <returns>密文</returns>
    private static string SHA1_Encrypt(string sourceString)
    {
        byte[] strRes = Encoding.Default.GetBytes(sourceString);
        SHA1 iSha = SHA1.Create();
        strRes = iSha.ComputeHash(strRes);
        StringBuilder enText = new();
        foreach (byte iByte in strRes)
        {
            enText.AppendFormat("{0:x2}", iByte);
        }
        return enText.ToString().ToLower();
    }

    [Route("OnReceivedMessage")]
    [HttpPost]
    public string OnReceivedMessage()
    {
        StreamReader stream = new(HttpContext.Request.Body);
        string body = stream.ReadToEndAsync().GetAwaiter().GetResult();
        _logger.LogInformation("收到微信消息：\n" + body);
        return "success";
    }
}