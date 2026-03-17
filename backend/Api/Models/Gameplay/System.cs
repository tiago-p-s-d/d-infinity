using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models.Gameplay;

[Table("systems")]
public class SystemModel 
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public required string Name { get; set; }

    // Foreign Keys (IDs)
    [Column("character_sheet_model_id")]
    public int CharacterSheetId { get; set; }

    [Column("items_id")]
    public int ItemsId { get; set; }

    [Column("spells_id")]
    public int SpellsId { get; set; }

    [Column("skills_id")]
    public int SkillsId { get; set; }

    [Column("maps_id")]
    public int MapsId { get; set; }

    [Column("currency_id")]
    public int CurrencyId { get; set; }

    [Column("races_id")]
    public int RacesId { get; set; }

    //(Nullable)
    [Column("classes_id")]
    public int? ClassesId { get; set; }

    [Column("skill_tree_id")]
    public int? SkillTreeId { get; set; }


}