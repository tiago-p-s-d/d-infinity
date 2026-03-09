using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models;

public class Usuario {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("nome")]
    public required string Nome { get; set; }

    [Column("email")]
    public required string Email { get; set; }

    [Column("senhaHash")]
    public required string SenhaHash { get; set; }
}