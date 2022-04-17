using EveryWhere.Database.JsonConverter;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace EveryWhere.MainServer.Entity.Request;

public class PatchShopRequest
{
    [Required]
    public int Id { get; set; }

    public string? Name { get; set; }

    [JsonConverter(typeof(TimeOnlyJsonConverter))]
    public TimeOnly? OpenTime { get; set; }

    [JsonConverter(typeof(TimeOnlyJsonConverter))]
    public TimeOnly? CloseTime { get; set; }

    public string? Address { get; set; }

    public string? AreaCode { get; set; }

    public string? Location { get; set; }

    public string? Tel { get; set; }

    public bool? IsOpening { get; set; }
}