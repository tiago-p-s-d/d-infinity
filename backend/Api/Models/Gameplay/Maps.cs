using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; 
using Api.Models.Gameplay.Groups;
using UserEntity = Api.Models.User.User;

namespace Api.Models.Gameplay;

[Table("maps")]
public class MapModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public required string Name { get; set; }

    [Column("map_image")]
    public required string MapImage { get; set; }

    [Column("created_by")]
    public int CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public UserEntity? Creator { get; set; }

    [Column("map_group_id")]
    public int? MapGroupId { get; set; }

    [ForeignKey("MapGroupId")]
    [JsonIgnore] 
    public virtual MapGroup? MapGroup { get; set; }
}