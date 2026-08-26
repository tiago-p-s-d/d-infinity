using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models.Gameplay;
using Api.Models.User;

[Table("items_inv")]
public class InventoryItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("inventory_id")]
    public int InventoryId { get; set; }
    
    [ForeignKey("InventoryId")]
    public virtual Inventory? Inventory { get; set; }

    [Column("item_id")]
    public int ItemId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; } = 1;
}