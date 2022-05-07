using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace EveryWhere.MainServer.Entity.Request;

public class PatchPrinterRequest
{
    [Required]
    public int Id { get; set; }
    
    public string? Name { get; set; }
    
    public bool? IsWork { get; set; }
    
    public bool? SupportColor { get; set; }

    public bool? SupportDuplex { get; set; }

    public string? DeviceName { get; set; }
    
    public string? SupportSizesJson { get; set; }
    
    public Dictionary<string, SizePrice>? SupportSizes
    {
        get => JsonConvert.DeserializeObject<Dictionary<string, SizePrice>>(SupportSizesJson ?? "")!;
        set => SupportSizesJson = JsonConvert.SerializeObject(value);
    }

    public struct SizePrice
    {
        public decimal SingleBlack { get; set; }
        public decimal SingleColor { get; set; }
        public decimal DuplexBlack { get; set; }
        public decimal DuplexColor { get; set; }
    }
}