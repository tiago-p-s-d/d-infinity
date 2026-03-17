using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Api.Models.Gameplay;

[Table("maps")]
public class MapModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public required string Name { get; set; }

    [Column("map_image")]
    public required string MapImage { get; set; }

    [Column("campaign_id")]
    public int CampaignId { get; set; }

    [ForeignKey("CampaignId")]

    public virtual Campaign? Campaign { get; set; } 

    [Column("created_by")]
    public int CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public User? Creator { get; set; }
}