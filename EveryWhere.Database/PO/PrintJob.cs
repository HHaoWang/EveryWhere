using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EveryWhere.Database.PO;

[Table("print_job")]
public class PrintJob:BasePO
{
    [Required]
    [Column("id", TypeName = "int(11)")]
    public int Id { get; set; }

    [Required]
    [Column("is_finished", TypeName = "tinyint(1)")]
    [Comment("是否完成")]
    public bool? IsFinished { get; set; }

    [Required]
    [Column("file_id", TypeName = "int(11)")]
    [Comment("文件ID")]
    public int? FileId { get; set; }

    [Required]
    [Column("order_id", TypeName = "int(11)")]
    [Comment("订单ID")]
    public int? OrderId { get; set; }

    [Column("printer_id", TypeName = "int(11)")]
    [Comment("打印机ID")]
    public int? PrinterId { get; set; }

    [Required]
    [Column("count",TypeName = "int(11)")]
    [Comment("打印份数")]
    public int? Count { get; set; }

    [Required]
    [Column("color", TypeName = "tinyint(1)")]
    [Comment("彩色打印")]
    public bool? Color { get; set; }

    [Required]
    [Column("duplex", TypeName = "tinyint(1)")]
    [Comment("双面打印")]
    public bool? Duplex { get; set; }

    [Required]
    [Column("page_start", TypeName = "int(11)")]
    [Comment("打印开始页")]
    public int? PageStart { get; set; }

    [Required]
    [Column("page_end", TypeName = "int(11)")]
    [Comment("打印结束页")]
    public int? PageEnd { get; set; }

    [Required]
    [Column("page_size", TypeName = "varchar(30)")]
    [Comment("打印纸张大小")]
    public string? PageSize { get; set; }

    [Column("fetch_code", TypeName = "varchar(10)")]
    [Comment("取件码")]
    public string? FetchCode { get; set; }

    [Required]
    [Column("create_time", TypeName = "datetime")]
    public DateTime? CreateTime { get; set; }

    [Required]
    [Column("price", TypeName = "decimal(8,2)")]
    [Comment("任务价格")]
    public decimal? Price { get; set; }

    #region 关联实体

    [ForeignKey("OrderId")]
    public Order? Order { get; set; }

    [ForeignKey("FileId")]
    public File? File { get; set; }

    [ForeignKey("PrinterId")]
    public Printer? Printer { get; set; }

    #endregion
}