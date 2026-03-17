using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
}