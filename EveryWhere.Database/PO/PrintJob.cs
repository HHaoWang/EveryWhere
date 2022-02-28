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

#nullable disable

[Table("print_job")]
public class PrintJob
{
    [Required]
    [Column("id", TypeName = "int(11)")]
    public int Id { get; set; }

    [Required]
    [Column("job_sequence", TypeName = "int(11)")]
    [Comment("打印顺序")]
    public int JobSequence { get; set; }

    [Required]
    [Column("order_id", TypeName = "int(11)")]
    [Comment("订单号")]
    public int OrderId { get; set; }

    [ForeignKey("OrderId")]
    public Order Order { get; set; }

    public File File { get; set; }

    [Required]
    [Column("status", TypeName = "ENUM('NotUploaded','UploadFailed','Converting','Uploaded','Queuing','Printing','NotTaken','Finish')")]
    public StatusState Status { get; set; }

    [Required]
    [Column("create_time", TypeName = "datetime")]
    public DateTime CreateTime { get; set; }

    public enum StatusState
    {
        NotUploaded,
        UploadFailed,
        Converting,
        Uploaded,
        Queuing,
        Printing,
        NotTaken,
        Finish
    }
}