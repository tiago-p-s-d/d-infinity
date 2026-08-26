using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Api.Services.Gameplay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Gameplay;

[Authorize]
[ApiController]
[Route("api/campaigns/{campaignId:int}/[controller]")]
public class ChatController(ChatService chatService) : ControllerBase
{
    private readonly ChatService _chatService = chatService;

    [HttpGet]
    public async Task<IActionResult> GetMessages([FromRoute] int campaignId, [FromQuery] int count = 50)
    {
        var messages = await _chatService.GetRecentMessagesAsync(campaignId, count);
        return Ok(messages);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromRoute] int campaignId, [FromBody] SendMessageRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userNameClaim = User.FindFirstValue(ClaimTypes.Name) ?? "Adventurer";

        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var result = await _chatService.ProcessAndSendMessageAsync(campaignId, userId, userNameClaim, request.Content);
        return Ok(result);
    }
}

public record SendMessageRequest([Required, MaxLength(2000)] string Content);