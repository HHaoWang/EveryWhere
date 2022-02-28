using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EveryWhere.Database.PO;

#nullable disable

[Table("consumer")]
public class Consumer
{
    [Required]
    [Column("id", TypeName = "int(11)")]
    public int Id { get; set; }

    [Required]
    [Column("nick_name",TypeName = "varchar(30)")]
    [Comment("昵称")]
    public string NickName { get; set; }

    [Required]
    [Column("wechat_open_id",TypeName = "varchar(120)")]
    [Comment("微信下发的openId")]
    public string WechatOpenId { get; set; }

    [Required]
    [Column("wechat_union_id",TypeName = "varchar(120)")]
    [Comment("微信下发的unionId")]
    public string WechatUnionId { get; set; }

    [Required]
    [Column("wechat_session_key", TypeName = "varchar(120)")]
    [Comment("微信下发的sessionKey")]
    public string WechatSessionKey { get; set; }
}