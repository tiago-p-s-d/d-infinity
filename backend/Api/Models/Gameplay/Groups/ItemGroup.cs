using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; 

namespace Api.Models.Gameplay.Groups;

[Table("item_groups")]
public class ItemGroup
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public required string Name { get; set; }

    [Column("description")]
    public string? Description { get; set; }
    
    [Column("created_by")]
    public int? CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    [JsonIgnore] 
    public virtual Api.Models.User.User? Creator { get; set; }

    public virtual ICollection<Item> Items { get; set; } = [];
}