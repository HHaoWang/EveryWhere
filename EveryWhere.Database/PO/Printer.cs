using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveryWhere.Database.PO;

#nullable disable

[Table("printer")]
public class Printer
{
    [Required]
    [Column("id", TypeName = "int(11)")]
    public int Id { get; set; }

    [Required]
    [Column("name", TypeName = "varchar(100)")]
    public string Name { get; set; }

    [Required]
    [Column("machine_guid", TypeName = "varchar(30)")]
    public string MachineGUID { get; set; }

    [Required]
    [Column("create_time", TypeName = "datetime")]
    public DateTime CreateTime { get; set; }

    [Required]
    [Column("shop_id", TypeName = "int(11)")]
    public int ShopId { get; set; }

    [ForeignKey("ShopId")]
    public Shop Shop { get; set; }
}