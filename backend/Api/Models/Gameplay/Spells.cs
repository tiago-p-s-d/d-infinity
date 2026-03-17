using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models.Gameplay;

[Table("spells")]
public class Spell
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
    /// Ex: {"mana_cost": 10, "dice": "2d6", "element": "fire", "range": "30ft"}
    /// </summary>
    [Column("effect", TypeName = "json")]
    public string Effect { get; set; } = "{}";
}