using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Api.Models.Gameplay.Groups;

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

    [Column("definitions")]
    public string? Definitions { get; set; } 

    [Column("item_group_id")]
    public int ItemGroupId { get; set; }

    [ForeignKey("ItemGroupId")]
    public virtual ItemGroup? Group { get; set; }

    [Column("created_by")]
    public int CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public virtual User? Creator { get; set; }
}