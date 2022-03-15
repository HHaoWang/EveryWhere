using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveryWhere.Database.PO;

#nullable disable

[Table("shop")]
public class Shop
{
    [Required]
    [Column("id", TypeName = "int(11)")]
    public int Id { get; set; }

    [Required]
    [Column("name", TypeName = "varchar(30)")]
    public string Name { get; set; }

    [Required]
    [Column("create_time", TypeName = "datetime")]
    public DateTime CreateTime { get; set; }

    [Required]
    [Column("update_time", TypeName = "datetime")]
    public DateTime UpdateTime { get; set; }

    public List<Order> Orders { get; set; }

    public List<Printer> Printers { get; set; }
}