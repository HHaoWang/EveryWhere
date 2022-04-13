using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EveryWhere.Database.PO;

#nullable disable

[Table("order")]
public class Order:BasePO
{
    [Required]
    [Column("id", TypeName = "int(11)")]
    public int Id { get; set; }

    [Required]
    [Column("create_time", TypeName = "datetime")]
    public DateTime CreateTime { get; set; }

    [Required]
    [Column("consumer_id",TypeName = "int(11)")]
    public int ConsumerId { get; set; }

    [Required]
    [Column("shop_id", TypeName = "int(11)")]
    public int ShopId { get; set; }

    [Required]
    [Column("price",TypeName ="decimal(8,2)")]
    [Comment("订单价格")]
    public decimal Price { get; set; }

    [Required]
    [Column("state", TypeName = "enum('UnPaid','Printing','Finished')")]
    [Comment("订单状态")]
    public OrderState State { get; set; }

    #region 关联实体

    [ForeignKey("ShopId")]
    public Shop Shop { get; set; }

    [ForeignKey("ConsumerId")]
    public User Consumer { get; set; }

    public List<PrintJob> PrintJobs { get; set; }

    #endregion

    public enum OrderState
    {
        UnPaid,
        Printing,
        Finished
    }
}