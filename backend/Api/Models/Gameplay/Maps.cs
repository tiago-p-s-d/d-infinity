using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

    [Column("grid_cell_size")]
    public int GridCellSize { get; set; } = 70;

    [Column("grid_offset_x")]
    public int GridOffsetX { get; set; } = 0;

    [Column("grid_offset_y")]
    public int GridOffsetY { get; set; } = 0;
    
    [Column("created_by")]
    public int CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public UserEntity? Creator { get; set; }

    [Column("map_group_id")]
    public int? MapGroupId { get; set; }

    [ForeignKey("MapGroupId")]
    public virtual MapGroup? MapGroup { get; set; }

    [InverseProperty("Map")]
    public virtual ICollection<MapTokenModel> Tokens { get; set; } = new List<MapTokenModel>();
}