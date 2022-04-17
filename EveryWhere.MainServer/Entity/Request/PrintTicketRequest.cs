using System.ComponentModel.DataAnnotations;

namespace EveryWhere.MainServer.Entity.Request;

public class PrintTicketRequest
{
    /// <summary>
    /// 文件ID
    /// </summary>
    [Required]
    public int FileId { get; set; }

    /// <summary>
    /// 打印机ID
    /// </summary>
    [Required]
    public int PrinterId { get; set; }

    /// <summary>
    /// 打印纸张大小
    /// </summary>
    [Required]
    public string? Size { get; set; }

    /// <summary>
    /// 打印开始页
    /// </summary>
    [Required]
    public int PagesStart { get; set; }

    /// <summary>
    /// 打印结束页
    /// </summary>
    [Required]
    public int PagesEnd { get; set; }

    /// <summary>
    /// 是否彩印
    /// </summary>
    [Required]
    public bool Color { get; set; }

    /// <summary>
    /// 是否双面打印
    /// </summary>
    [Required]
    public bool Duplex { get; set; }

    /// <summary>
    /// 打印数量
    /// </summary>
    [Required]
    public int Count { get; set; }
}