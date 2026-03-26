using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Api.Models.Gameplay.Groups;

namespace Api.Models.Gameplay;
using Api.Models.User;

[Table("classes")]
public class ClassModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public required string Name { get; set; }

    [Column("about")]
    public string? About { get; set; }

    [Column("definitions", TypeName = "json")]
    public string Definitions { get; set; } = "[]"; 

    [Column("class_group_id")]
    public int ClassGroupId { get; set; }

    [ForeignKey("ClassGroupId")]
    public virtual ClassGroup? Group { get; set; }

    [Column("created_by")]
    public int CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public virtual User? Creator { get; set; }
}