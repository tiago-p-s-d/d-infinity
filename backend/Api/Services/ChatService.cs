using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using Api.Data;
using Api.Hubs;
using Api.Models.Gameplay;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Gameplay;

public class ChatService(AppDbContext context, IHubContext<ChatHub> hubContext)
{
    private readonly AppDbContext _context = context;
    private readonly IHubContext<ChatHub> _hubContext = hubContext;

    public async Task<List<ChatMessageDto>> GetRecentMessagesAsync(int campaignId, int count = 50)
    {
        var messages = await _context.Set<ChatMessage>()
            .Where(m => m.CampaignId == campaignId)
            .OrderByDescending(m => m.SentAt)
            .Take(count)
            .ToListAsync();

        messages.Reverse();

        return messages.Select(MapToDto).ToList();
    }

    public async Task<ChatMessageDto> ProcessAndSendMessageAsync(int campaignId, int userId, string userName, string content)
    {
        var trimmedContent = content.Trim();
        var messageType = ChatMessageType.Text;
        string? metadata = null;

        // Check if message is a dice roll command (e.g., /r 2d20+5 or /roll 1d6)
        var rollMatch = Regex.Match(trimmedContent, @"^/(?:roll|r)\s+(\d+)?d(\d+)(?:([+-])(\d+))?", RegexOptions.IgnoreCase);

        if (rollMatch.Success)
        {
            messageType = ChatMessageType.DiceRoll;
            var diceCount = string.IsNullOrEmpty(rollMatch.Groups[1].Value) ? 1 : int.Parse(rollMatch.Groups[1].Value);
            var diceSides = int.Parse(rollMatch.Groups[2].Value);
            var operatorSign = rollMatch.Groups[3].Value;
            var modifier = string.IsNullOrEmpty(rollMatch.Groups[4].Value) ? 0 : int.Parse(rollMatch.Groups[4].Value);

            // Cap limits to prevent resource exhaustion
            diceCount = Math.Clamp(diceCount, 1, 100);
            diceSides = Math.Clamp(diceSides, 2, 1000);

            var rolls = new List<int>();
            for (var i = 0; i < diceCount; i++)
            {
                rolls.Add(RandomNumberGenerator.GetInt32(1, diceSides + 1));
            }

            var baseSum = rolls.Sum();
            var total = operatorSign == "-" ? baseSum - modifier : baseSum + modifier;

            var rollResult = new
            {
                DiceCount = diceCount,
                DiceSides = diceSides,
                Rolls = rolls,
                Modifier = operatorSign == "-" ? -modifier : modifier,
                Total = total
            };

            metadata = JsonSerializer.Serialize(rollResult);
            trimmedContent = $"rolled {diceCount}d{diceSides}{(modifier != 0 ? $"{operatorSign}{modifier}" : "")}: **{total}**";
        }

        var message = new ChatMessage
        {
            CampaignId = campaignId,
            UserId = userId,
            SenderName = userName,
            Content = trimmedContent,
            Type = messageType,
            MetadataJson = metadata,
            SentAt = DateTime.UtcNow
        };

        _context.Set<ChatMessage>().Add(message);
        await _context.SaveChangesAsync();

        var dto = MapToDto(message);

        // Broadcast to campaign room via SignalR
        await _hubContext.Clients.Group($"campaign_{campaignId}").SendAsync("ReceiveMessage", dto);

        return dto;
    }

    private static ChatMessageDto MapToDto(ChatMessage m) => new(
        m.Id,
        m.CampaignId,
        m.UserId,
        m.SenderName,
        m.Content,
        m.Type.ToString(),
        m.MetadataJson,
        m.SentAt
    );
}

public record ChatMessageDto(
    int Id,
    int CampaignId,
    int UserId,
    string SenderName,
    string Content,
    string Type,
    string? MetadataJson,
    DateTime SentAt
);