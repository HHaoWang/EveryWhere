using Newtonsoft.Json;

namespace EveryWhere.MainServer.Entity.Response;

public class WechatCode2SessionResponse
{
    /// <summary>
    /// 微信下发的用户唯一标识
    /// </summary>
    [JsonProperty("openid")]
    public string? OpenId;

    /// <summary>
    /// 微信下发的会话密钥
    /// </summary>
    [JsonProperty("session_key")]
    public string? SessionKey;

    /// <summary>
    /// 用户在微信开放平台的唯一标识符，在满足 UnionID 下发条件的情况下会返回，详见 UnionID 机制说明。
    /// </summary>
    [JsonProperty("unionid")]
    public string? UnionId;

    /// <summary>
    /// 错误码
    /// </summary>
    [JsonProperty("errcode")]
    public int ErrorCode;

    /// <summary>
    /// 错误信息
    /// </summary>
    [JsonProperty("errmsg")]
    public string? ErrorMsg;
}