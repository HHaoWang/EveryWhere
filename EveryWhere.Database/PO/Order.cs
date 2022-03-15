﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EveryWhere.Database.PO;

#nullable disable

[Table("order")]
public class Order
{
    [Required]
    [Column("id", TypeName = "int(11)")]
    public int Id { get; set; }

    [Required]
    [Column("create_time", TypeName = "datetime")]
    public DateTime CreateTime { get; set; }

    [Required]
    [Column("status",TypeName = "ENUM('NotUploaded','UnPaid','Converting','Printing','NotTaken','Finish')")]
    public StatusState Status { get; set; }

    [Required]
    [Column("shop_id", TypeName = "int(11)")]
    public int ShopId { get; set; }

    [ForeignKey("ShopId")]
    public Shop Shop { get; set; }

    public List<PrintJob> PrintJobs { get; set; }

    public enum StatusState
    {
        NotUploaded,
        UnPaid,
        Converting,
        Printing,
        NotTaken,
        Finish
    }
}