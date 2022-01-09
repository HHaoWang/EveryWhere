using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace EveryWhere.DTO.Settings
{
    public class TokenSettings
    {
        /// <summary>
        /// 密钥（token加密盐值）
        /// </summary>
        [JsonProperty("saltValue")]
        public string SaltValue { get; set; }

        /// <summary>
        /// 签发者
        /// </summary>
        [JsonProperty("issuer")]
        public string Issuer { get; set; }

        /// <summary>
        /// 验证者
        /// </summary>
        [JsonProperty("audience")]
        public string Audience { get; set; }

        /// <summary>
        /// 有效时长
        /// </summary>
        [JsonProperty("accessExpiration")]
        public int AccessExpiration { get; set; }

        /// <summary>
        /// 刷新时长，多久之后进行刷新
        /// </summary>
        [JsonProperty("refreshExpiration")]
        public int RefreshExpiration { get; set; }
    }
}
