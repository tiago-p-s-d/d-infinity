using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Api.Models.User;

namespace Api.Models.Gameplay;

[Table("currencies")]
public class Currency
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public required string Name { get; set; }

    [Column("created_by")]
    public int CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public virtual Api.Models.User.User? Creator { get; set; }
    public virtual ICollection<CurrencyValue> Values { get; set; } = [];
}