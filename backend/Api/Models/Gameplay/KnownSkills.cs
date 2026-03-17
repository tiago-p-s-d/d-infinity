using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models.Gameplay;

[Table("known_skills")]
public class KnownSkill
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("skill_id")]
    public int SkillId { get; set; }

    [ForeignKey("SkillId")]
    public virtual Skill? Skill { get; set; }

    [Column("character_sheet_id")]
    public int CharacterSheetId { get; set; }

    [ForeignKey("CharacterSheetId")]
    public virtual CharacterSheet? CharacterSheet { get; set; }

}