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
    [Column("name",TypeName = "varchar(30)")]
    [Comment("店铺名称")]
    public string Name { get; set; }

    [Required]
    [Column("location_description",TypeName = "varchar(80)")]
    [Comment("店铺位置描述")]
    public string LocationDescription { get; set; }

    public List<Printer> Printers { get; set; }
}