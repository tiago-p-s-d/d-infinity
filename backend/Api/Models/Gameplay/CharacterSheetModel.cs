using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Api.Models.Gameplay;
using Api.Models.User;

[Table("character_sheet_models")]
public class CharacterSheetModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public required string Name { get; set; } 

    [Column("definitions", TypeName = "json")]
    public string Definitions { get; set; } = "{}";

    [Column("created_by")]
    public int CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public User? Creator { get; set; }
    
    public virtual ICollection<CharacterSheet> Instances { get; set; } = [];
}