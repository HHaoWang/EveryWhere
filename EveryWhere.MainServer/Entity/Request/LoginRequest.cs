using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace EveryWhere.MainServer.Entity.Request;

public class LoginRequest
{
    [Required]
    [JsonProperty("userCode")]
    public string? UserCode { set; get; }

    [JsonProperty("token")]
    public string? Token { set; get; }

    [JsonProperty("avatarUrl")]
    public string? AvatarUrl { set; get; }

    [JsonProperty("nickName")]
    public string? NickName { get; set; }
}