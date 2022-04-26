using Newtonsoft.Json;

namespace EveryWhere.MainServer.Entity.Response;

public class WechatAccessTokenResponse
{
    [JsonProperty("access_token")]
    public string? AccessToken { get; set; }

    [JsonProperty("expires_in")]
    public int ValidSeconds { get; set; }

    [JsonProperty("errcode")]
    public int? ErrorCode { get; set; }

    [JsonProperty("errmsg")]
    public string? ErrorMsg { get; set; }
}