using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EveryWhere.Database.JsonConverter;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EveryWhere.Database.PO;

#nullable disable

[Table("shop")]
public class Shop
{
    [Required]
    [Column("id", TypeName = "int(11)")]
    public int Id { get; set; }

    [Required]
    [Column("name",TypeName = "varchar(120)")]
    [Comment("店铺名称")]
    public string Name { get; set; }

    [Required]
    [Column("open_time", TypeName = "time")]
    [Comment("开始营业时间")]
    [JsonConverter(typeof(TimeOnlyJsonConverter))]
    public TimeOnly OpenTime { get; set; }

    [Required]
    [Column("close_time", TypeName = "time")]
    [JsonConverter(typeof(TimeOnlyJsonConverter))]
    [Comment("结束营业时间")]
    public TimeOnly CloseTime { get; set; }

    [Required]
    [Column("address",TypeName = "varchar(120)")]
    [Comment("店铺位置描述")]
    public string Address { get; set; }

    [Required]
    [Column("area", TypeName = "varchar(10)")]
    [Comment("所在行政区域")]
    public string AreaCode { get; set; }

    [Column("location", TypeName = "varchar(20)")]
    [Comment("经纬度")]
    public string Location { get; set; }

    [Column("tel", TypeName = "varchar(11)")]
    [Comment("电话号")]
    public string Tel { get; set; }

    [Required]
    [Column("create_time", TypeName = "datetime")]
    public DateTime CreateTime { get; set; }

    [Required]
    [Column("shopkeeper_id", TypeName = "int(11)")]
    [Comment("店主ID")]
    public int ShopKeeperId { get; set; }

    [Column("is_opening",TypeName = "tinyint(1)")]
    [Comment("是否营业")]
    public bool IsOpening { get; set; }

    #region 关联实体

    public List<Printer> Printers { get; set; }

    [ForeignKey("ShopKeeperId")]
    public User Shopkeeper { get; set; }
    
    public Area Area { get; set; }

    #endregion
}