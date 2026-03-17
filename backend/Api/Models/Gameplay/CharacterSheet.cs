using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Api.Models.Gameplay;

[Table("character_sheets")]
public class CharacterSheet
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("character_name")]
    public required string CharacterName { get; set; }

    [Column("level")]
    public int Level { get; set; } = 1;

    [Column("current_hp")]
    public int CurrentHp { get; set; }

    [Column("max_hp")]
    public int MaxHp { get; set; }

    [Column("current_mp")]
    public int CurrentMp { get; set; }

    [Column("max_mp")]
    public int MaxMp { get; set; }

    [Column("backstory")]
    public string? Backstory { get; set; }

    [Column("characteristics")]
    public string? Characteristics { get; set; }

    [Column("stats", TypeName = "json")]
    public string Stats { get; set; } = "{}";


    [Column("inventory_id")]
    public int? InventoryId { get; set; }
    
    [ForeignKey("InventoryId")]
    public virtual Inventory? Inventory { get; set; }

    [Column("race_id")]
    public int? RaceId { get; set; }
    
    [ForeignKey("RaceId")]
    public virtual Race? Race { get; set; }


    public virtual ICollection<KnownSpell> KnownSpells { get; set; } = [];
    public virtual ICollection<KnownSkill> KnownSkills { get; set; } = [];
}