namespace EveryWhere.MainServer.Entity.Dto;

public class QrCodeCacheInfo
{
    public static string Operation = "Login";
    public string Uuid { get; set; }
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