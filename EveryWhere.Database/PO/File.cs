using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EveryWhere.Database.PO;

[Table("file")]
public class File:BasePO
{
    [Required]
    [Column("id",TypeName = "int(11)")]
    public int Id { get; set; }

    [Required]
    [Column("create_time", TypeName = "datetime")]
    [Comment("上传时间")]
    public DateTime? CreateTime { get; set; }

    [Required]
    [Column("uploader_id", TypeName = "int(11)")]
    public int? UploaderId { get; set; }

    [Required]
    [Column("size", TypeName = "double(8,2)")]
    [Comment("文件大小")]
    public double? Size { get; set; }

    [Required]
    [Column("original_name", TypeName = "varchar(120)")]
    [Comment("文件上传时的原始名称")]
    public string? OriginalName { get; set; }

    [Required]
    [Column("name",TypeName = "varchar(120)")]
    [Comment("在服务器上的文件名")]
    public string? Name { get; set; }

    [Required]
    [Column("location", TypeName = "varchar(120)")]
    [Comment("存放位置")]
    public string? Location { get; set; }

    [Column("page_count", TypeName = "int")]
    [Comment("页数")]
    public int? PageCount { get; set; }

    [Required]
    [Column("is_converted", TypeName = "tinyint(1)")]
    [Comment("是否转换完成")]
    public bool? IsConverted { get; set; }

    #region 关联实体

    [ForeignKey("UploaderId")]
    public User? Uploader { get; set; }

    #endregion
}