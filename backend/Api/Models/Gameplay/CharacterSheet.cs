using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models.Gameplay;
using Api.Models.User;

[Table("character_sheets")]
public class CharacterSheet
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("character_name")]
    public required string CharacterName { get; set; }

    [Column("model_id")]
    public int ModelId { get; set; }

    [ForeignKey("ModelId")]
    public virtual CharacterSheetModel? Model { get; set; }

    [Column("values", TypeName = "json")]
    public string Values { get; set; } = "{}";

    [Column("player_id")]
    public int PlayerId { get; set; }

    [ForeignKey("PlayerId")]
    public User? Player { get; set; }

    [Column("campaign_id")]
    public int CampaignId { get; set; }

    [ForeignKey("CampaignId")]
    public virtual Campaign? Campaign { get; set; }

    [Column("race_id")]
    public int? RaceId { get; set; }
    [ForeignKey("RaceId")]
    public virtual Race? Race { get; set; }

    public virtual ICollection<KnownSpell> KnownSpells { get; set; } = [];
    public virtual ICollection<KnownSkill> KnownSkills { get; set; } = [];
}