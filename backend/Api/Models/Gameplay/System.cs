using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Api.Models.User; // Namespace do usuário
using Api.Models.Gameplay.Groups; 

namespace Api.Models.Gameplay;

[Table("systems")]
public class SystemModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public required string Name { get; set; }

    [Column("created_by")]
    public int CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public Api.Models.User.User? Creator { get; set; }

    [Column("character_sheet_model_id")]
    public int CharacterSheetId { get; set; }

    [Column("currency_id")]
    public int CurrencyId { get; set; }

    [Column("classes_id")]
    public int? ClassesId { get; set; }

    [Column("skill_tree_id")]
    public int? SkillTreeId { get; set; }


    public virtual ICollection<SystemRaceGroup> SystemRaces { get; set; } = [];
    public virtual ICollection<SystemItemGroup> SystemItems { get; set; } = [];
    public virtual ICollection<SystemSpellGroup> SystemSpells { get; set; } = [];
    public virtual ICollection<SystemSkillGroup> SystemSkills { get; set; } = [];
    public virtual ICollection<SystemMapGroup> SystemMaps { get; set; } = [];
}