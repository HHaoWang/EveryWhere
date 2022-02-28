using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace EveryWhere.Database.PO;

public class Printer
{
    [Required]
    [Column("id", TypeName = "int(11)")]
    public int Id { get; set; }

    [Required]
    [Column("name", TypeName ="varchar(50)")]
    [Comment("打印机的名称，可由用户更改")]
    public string Name { get; set; }

    [Required]
    [Column("support_color",TypeName = "tinyint(1)")]
    [Comment("打印机是否支持彩色打印")]
    public bool SupportColor { get; set; }

    [Required]
    [Column("support_duplex",TypeName = "tinyint(1)")]
    [Comment("打印机是否支持双面打印")]
    public bool SupportDuplex { get; set; }

    [Required]
    [Column("shop_id", TypeName = "int(11)")]
    public int ShopId { get; set; }

    [ForeignKey("ShopId")]
    public Shop Shop { get; set; }
}