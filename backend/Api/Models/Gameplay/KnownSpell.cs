using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models.Gameplay;

[Table("known_spells")]
public class KnownSpell
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("spell_id")]
    public int SpellId { get; set; }

    [ForeignKey("SpellId")]
    public virtual Spell? Spell { get; set; }

    [Column("character_sheet_id")]
    public int CharacterSheetId { get; set; }

    [ForeignKey("CharacterSheetId")]
    public virtual CharacterSheet? CharacterSheet { get; set; }
}