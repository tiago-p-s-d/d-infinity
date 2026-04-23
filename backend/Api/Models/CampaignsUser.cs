using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Api.Models.User;

using UserEntity = Api.Models.User.User;

namespace Api.Models;

[Table("campaign_users")]
public class CampaignUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_usr_cmpg")]
    public int Id { get; set; }

    [Column("id_user")]
    public int UserId { get; set; }
    public virtual UserEntity User { get; set; } = null!;

    [Column("id_campaign")]
    public int CampaignId { get; set; }
    public virtual Campaign Campaign { get; set; } = null!;

    [Column("is_dm")]
    public bool IsDm { get; set; }
}