using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Api.Models.User;

namespace Api.Models.Gameplay;

public enum ChatMessageType
{
    Text = 1,
    DiceRoll = 2,
    System = 3
}

[Table("chat_messages")]
public class ChatMessage
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CampaignId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string SenderName { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;

    public ChatMessageType Type { get; set; } = ChatMessageType.Text;

    [MaxLength(1000)]
    public string? MetadataJson { get; set; }

    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public User.User? User { get; set; }
}