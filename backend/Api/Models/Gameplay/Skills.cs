using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Api.Models.Gameplay.Groups;

namespace Api.Models.Gameplay;

[Table("skills")]
public class Skill
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public required string Name { get; set; }

    [Column("about")]
    public string? About { get; set; }

    /// <summary>
    /// (ex: {"type": "buff", "stat": "stealth", "value": 2})
    /// </summary>
    [Column("effect", TypeName = "json")]
    public string Effect { get; set; } = "{}";

    [Column("skill_group_id")]
    public int SkillGroupId { get; set; }

    [ForeignKey("SkillGroupId")]
    public virtual SkillGroup? Group { get; set; }
    
    [Column("created_by")]
    public int CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public User? Creator { get; set; }
}