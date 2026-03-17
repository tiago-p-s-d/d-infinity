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

    public virtual ICollection<CampaignUser> CampaignMembers { get; set; } = [];

    public virtual ICollection<MapModel> Maps { get; set; } = [];
}