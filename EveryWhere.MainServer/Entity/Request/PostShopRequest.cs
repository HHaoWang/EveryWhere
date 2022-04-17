using EveryWhere.Database.JsonConverter;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace EveryWhere.MainServer.Entity.Request;

public class PostShopRequest
{
    [Required]
    public string Name { get; set; }

    [Required]
    [JsonConverter(typeof(TimeOnlyJsonConverter))]
    public TimeOnly OpenTime { get; set; }

    [Required]
    [JsonConverter(typeof(TimeOnlyJsonConverter))]
    public TimeOnly CloseTime { get; set; }

    [Required]
    public string Address { get; set; }

    [Required]
    public string AreaCode { get; set; }

    public string Location { get; set; }

    [Required]
    public string Tel { get; set; }

    public bool IsOpening { get; set; }
}