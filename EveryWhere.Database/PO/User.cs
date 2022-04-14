using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EveryWhere.Database.PO;

#nullable disable

[Table("user")]
public class User:BasePO
{
    [Required]
    [Column("id", TypeName = "int(11)")]
    public int Id { get; set; }

    [Required]
    [Column("nick_name",TypeName = "varchar(120)")]
    [Comment("昵称")]
    public string NickName { get; set; }

    [Required]
    [Column("avatar",TypeName = "varchar(120)")]
    [Comment("头像")]
    public string Avatar { get; set; }

    [Column("tel", TypeName = "varchar(11)")]
    [Comment("电话号")]
    public string Tel { get; set; }

    [Required]
    [Column("wechat_open_id",TypeName = "varchar(120)")]
    [Comment("微信下发的openId")]
    public string WechatOpenId { get; set; }

    [Column("wechat_union_id",TypeName = "varchar(120)")]
    [Comment("微信下发的unionId")]
    public string WechatUnionId { get; set; }

    [Required]
    [Column("wechat_session_key", TypeName = "varchar(120)")]
    [Comment("微信下发的sessionKey")]
    public string WechatSessionKey { get; set; }

    [Required]
    [Column("is_manager", TypeName = "tinyint(1)")]
    [Comment("是否是管理者")]
    public bool IsManager { get; set; }

    [Required]
    [Column("create_time", TypeName = "datetime")]
    public DateTime CreateTime { get; set; }

    #region 关联实体

    public List<Order> Orders { get; set; }

    #endregion
}