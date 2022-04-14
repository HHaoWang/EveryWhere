using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

#nullable disable

namespace EveryWhere.Database.PO;

[Table("printer")]
public class Printer:BasePO
{
    [Required]
    [Column("id", TypeName = "int(11)")]
    public int Id { get; set; }

    [Required]
    [Column("name", TypeName = "varchar(120)")]
    [Comment("打印机的名称，可由用户更改")]
    public string Name { get; set; }

    [Required]
    [Column("shop_id", TypeName = "int(11)")]
    public int ShopId { get; set; }

    [Required]
    [Column("is_work", TypeName = "tinyint(1)")]
    public bool IsWork { get; set; }

    [Required]
    [Column("support_color", TypeName = "tinyint(1)")]
    [Comment("打印机是否支持彩色打印")]
    public bool SupportColor { get; set; }

    [Required]
    [Column("support_duplex", TypeName = "tinyint(1)")]
    [Comment("打印机是否支持双面打印")]
    public bool SupportDuplex { get; set; }

    [Required]
    [Column("computer_id", TypeName = "varchar(40)")]
    [Comment("计算机标识")]
    public string ComputerId { get; set; }

    [Required]
    [Column("support_sizes", TypeName = "json")]
    [Comment("支持的纸张大小")]
    public string SupportSizesJson { get; set; }

    [NotMapped]
    public Dictionary<string,PaperSizePrice> SupportSizes
    {
        get => JsonConvert.DeserializeObject<Dictionary<string, PaperSizePrice>>(SupportSizesJson);
        set => SupportSizesJson = JsonConvert.SerializeObject(value);
    }

    [Required]
    [Column("create_time", TypeName = "datetime")]
    public DateTime CreateTime { get; set; }

    #region 关联实体

    [ForeignKey("ShopId")]
    public Shop Shop { get; set; }

    public List<PrintJob> PrintJobs { get; set; }

    #endregion

    public struct PaperSizePrice
    {
        public decimal SingleBlack;
        public decimal SingleColor;
        public decimal DuplexBlack;
        public decimal DuplexColor;
    }
}