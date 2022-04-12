using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace EveryWhere.Database.PO;

[Index(nameof(AreaCode),IsUnique = true)]
[Table("area")]
public class Area
{
    [Required]
    [Column("id", TypeName = "int(11)")]
    public int? Id { get; set; }

    [Required]
    [Column("area_code", TypeName = "varchar(10)")]
    [Comment("行政区名称")]
    public string? AreaCode { get; set; }

    [Required]
    [Column("name", TypeName = "varchar(120)")]
    [Comment("行政区名称")]
    public string? Name { get; set; }

    [Column("parent_area_id",TypeName = "int(11)")]
    [Comment("上级区划ID")]
    public int? ParentAreaId { get; set; }

    #region 关联实体

    [ForeignKey("ParentAreaId")]
    [JsonIgnore]
    public Area? ParentArea { get; set; }

    public List<Shop>? Shops { get; set; }

    public List<Area>? SubAreas { get; set; }

    #endregion
}