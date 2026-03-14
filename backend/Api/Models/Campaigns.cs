using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
}