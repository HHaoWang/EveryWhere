using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
    public TimeOnly OpenTime { get; set; }

    [Required]
    [Column("close_time", TypeName = "time")]
    [Comment("结束营业时间")]
    public TimeOnly CloseTime { get; set; }

    [Required]
    [Column("address",TypeName = "varchar(120)")]
    [Comment("店铺位置描述")]
    public string Address { get; set; }

    [Required]
    [Column("area", TypeName = "varchar(10)")]
    [Comment("店铺名称")]
    public string AreaCode { get; set; }

    [Column("location", TypeName = "varchar(20)")]
    [Comment("经纬度")]
    public string Location { get; set; }

    [Required]
    [Column("create_time", TypeName = "datetime")]
    public DateTime CreateTime { get; set; }

    [Required]
    [Column("shopkeeper_id", TypeName = "int(11)")]
    [Comment("店主ID")]
    public int ShopKeeperId { get; set; }

    #region 关联实体

    public List<Printer> Printers { get; set; }

    [ForeignKey("ShopKeeperId")]
    public User Shopkeeper { get; set; }

    #endregion
}