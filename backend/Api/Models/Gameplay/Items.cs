using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models.Gameplay;

[Table("items")]
public class Item
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public required string Name { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    // Substituímos Modifier, Damage e AC por Definitions
    [Column("definitions")]
    public string? Definitions { get; set; } // Armazenará o JSON vindo do FieldBuilder

    [Column("created_by")]
    public int CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public virtual User? Creator { get; set; }
}