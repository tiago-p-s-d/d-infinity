using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models.User;

[Table("user_verifications")]
public class UserVerification
{
    [Key]
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string Code { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; } = false;
}