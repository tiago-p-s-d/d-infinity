using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Api.Models.Gameplay; 

namespace Api.Models.Gameplay.Groups;

[Table("spell_groups")]
public class SpellGroup
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public required string Name { get; set; }

    [Column("description")]
    public string? Description { get; set; }
    public virtual ICollection<Spell> Spells { get; set; } = new List<Spell>();
}