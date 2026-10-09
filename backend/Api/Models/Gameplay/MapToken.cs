using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserEntity = Api.Models.User.User;

namespace Api.Models.Gameplay;

[Table("map_tokens")]
public class MapTokenModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public required string Name { get; set; }

    [Column("token_image")]
    public required byte[] TokenImage { get; set; }

    [Column("coord_x")]
    public double CoordX { get; set; }

    [Column("coord_y")]
    public double CoordY { get; set; }

    [Column("size")]
    public double Size { get; set; } = 1.0;

    [Column("map_id")]
    public int MapId { get; set; }

    [ForeignKey("MapId")]
    public virtual MapModel? Map { get; set; }

    [Column("character_sheet_id")]
    public int? CharacterSheetId { get; set; }

    [ForeignKey("CharacterSheetId")]
    public virtual CharacterSheet? CharacterSheet { get; set; }

    [Column("owner_user_id")]
    public int? OwnerUserId { get; set; }

    [ForeignKey("OwnerUserId")]
    public virtual UserEntity? OwnerUser { get; set; }
}