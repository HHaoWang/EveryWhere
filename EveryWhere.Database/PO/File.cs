using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace EveryWhere.Database.PO;

[Table("file")]
public class File
{
    [Required]
    [Column("id",TypeName = "int(11)")]
    public int Id { get; set; }

    [Required]
    [Column("name",TypeName = "varchar(50)")]
    public string Name { get; set; }

    [Required]
    [Column("original_name",TypeName = "varchar(50)")]
    public string OriginalName { get; set; }

    [Required]
    [Column("create_time",TypeName = "datetime")]
    public DateTime CreateTime { get; set; }

    [Required]
    [Column("order_id", TypeName = "int(11)")]
    public int OrderId { get; set; }

    [ForeignKey("OrderId")]
    public Order Order { get; set; }

    [Required]
    [Column("print_job_id", TypeName = "int(11)")]
    public int PrintJobId { get; set; }

    [ForeignKey("PrintJobId")]
    public PrintJob PrintJob { get; set; }
}