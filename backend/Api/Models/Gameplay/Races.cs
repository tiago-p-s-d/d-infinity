using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Api.Models.Gameplay.Groups;

namespace Api.Models.Gameplay;

[Table("races")]
public class Race
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public required string Name { get; set; }

    [Column("about")]
    public string? About { get; set; }

    [Column("modifiers", TypeName = "json")]
    public string Modifiers { get; set; } = "{}";

    [Column("race_group_id")]
    public int RaceGroupId { get; set; }

    [ForeignKey("RaceGroupId")]
    public virtual RaceGroup? Group { get; set; }

    [Column("created_by")]
    public int CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public User? Creator { get; set; }
}