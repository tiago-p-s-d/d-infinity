using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Api.Models.Gameplay.Groups; 

namespace Api.Models.Gameplay;
using Api.Models.User;


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

    [Column("effect", TypeName = "json")]
    public string Effect { get; set; } = "{}";

    [Column("spell_group_id")]
    public int SpellGroupId { get; set; }

    [ForeignKey("SpellGroupId")]
    public virtual SpellGroup? Group { get; set; }

    [Column("created_by")]
    public int CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public User? Creator { get; set; }
}