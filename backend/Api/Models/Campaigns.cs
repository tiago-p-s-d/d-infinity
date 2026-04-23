using System.Collections.Generic;   
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Api.Models.Gameplay; 

namespace Api.Models;

[Table("campaigns")]
public class Campaign
{    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("campaign_name")]
    public required string CampaignName { get; set; }

    [Column("about")]
    public string? About { get; set; }

    [Column("system_id")]
    public int SystemId { get; set; }
    
    [ForeignKey("SystemId")]
    public virtual SystemModel System { get; set; } = null!;

    [Column("invite_code")]
    public string InviteCode { get; set; } = string.Empty;

    public virtual ICollection<CampaignUser> CampaignMembers { get; set; } = [];
}