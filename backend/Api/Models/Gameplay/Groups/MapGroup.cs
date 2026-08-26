using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using UserEntity = Api.Models.User.User;

namespace Api.Models.Gameplay.Groups;

[Table("map_groups")]
public class MapGroup
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
    public virtual UserEntity? Creator { get; set; }

    public virtual ICollection<MapModel> Maps { get; set; } = [];
}