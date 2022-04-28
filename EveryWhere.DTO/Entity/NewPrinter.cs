using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EveryWhere.DTO.Entity;

public class NewPrinter
{
    public string? Name { get; set; }

    public int? ShopId { get; set; }

    public bool? IsWork { get; set; }

    public bool? SupportColor { get; set; }

    public bool? SupportDuplex { get; set; }

    public string? DeviceName { get; set; }

    public string? ComputerId { get; set; }

    public string? SupportSizesJson { get; set; }

    [NotMapped]
    public Dictionary<string, PaperSizePrice> SupportSizes
    {
        get => JsonConvert.DeserializeObject<Dictionary<string, PaperSizePrice>>(SupportSizesJson ?? "{}")!;
        set => SupportSizesJson = JsonConvert.SerializeObject(value);
    }

    public struct PaperSizePrice
    {
        public decimal SingleBlack;
        public decimal SingleColor;
        public decimal DuplexBlack;
        public decimal DuplexColor;
    }
}