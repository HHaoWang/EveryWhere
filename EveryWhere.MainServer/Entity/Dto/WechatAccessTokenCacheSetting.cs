namespace EveryWhere.MainServer.Entity.Dto;

public class WechatAccessTokenCacheSetting
{
    public string? AccessToken { get; set; }
    public DateTime ExpiresTime { get; set; }
}