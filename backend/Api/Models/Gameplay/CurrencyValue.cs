using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models.Gameplay;

[Table("currency_values")]
public class CurrencyValue
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("currency_id")]
    public int CurrencyId { get; set; }

    [Column("name")]
    public required string Name { get; set; } 

    [Column("conversion_rate")]
    public decimal ConversionRate { get; set; }

    [ForeignKey("CurrencyId")]
    public virtual Currency? Currency { get; set; }
}