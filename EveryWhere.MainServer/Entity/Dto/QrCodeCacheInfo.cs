namespace EveryWhere.MainServer.Entity.Dto;

public class QrCodeCacheInfo
{
    public static string Operation = "Login";
    #pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public string Uuid { get; set; }
    #pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public DateTime CreateTime { get; set; }
    public DateTime ExpireTime { get; set; }
    public QrCodeState State { get; set; }
    public int? UserId { get; set; }
    
    public enum QrCodeState
    {
        Valid,
        Invalid,
        HasLogin
    }
}