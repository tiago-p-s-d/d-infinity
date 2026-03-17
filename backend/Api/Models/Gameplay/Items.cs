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

    [Column("modifier")]
    public string? Modifier { get; set; }

    [Column("damage")]
    public string? Damage { get; set; }

    [Column("ac")]
    public int? AC { get; set; }

    [Column("created_by")]
    public int CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public User? Creator { get; set; }
}