using System.ComponentModel.DataAnnotations;

namespace EveryWhere.MainServer.Entity.Request;

public class PostOrderRequest
{
    /// <summary>
    /// 店铺ID
    /// </summary>
    [Required]
    public int ShopId { get; set; }

    /// <summary>
    /// 打印设置
    /// </summary>
    [Required]
    public List<PrintTicketRequest>? PrintTickets { get; set; }
}