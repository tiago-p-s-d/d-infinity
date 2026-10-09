using System.Security.Claims;
using Api.Data;
using Api.DTOs.Gameplay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers.Gameplay;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CampaignSessionController : ControllerBase
{
    private readonly AppDbContext _context;

    public CampaignSessionController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("campaigns/{campaignId:int}/end-session")]
    public async Task<IActionResult> EndSession(int campaignId, [FromBody] EndSessionRequestDto request)
    {
        var userIdStr = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out var currentUserId))
        {
            return Unauthorized();
        }

        // Validação correta: verifica se o usuário é o DM na campanha via CampaignMembers
        var isDm = await _context.CampaignMembers
            .AnyAsync(m => m.CampaignId == campaignId && m.UserId == currentUserId && m.IsDm);

        if (!isDm)
        {
            return Forbid();
        }

        var tokenIds = request.Tokens.Select(t => t.TokenId).ToList();
        var tokensFromDb = await _context.MapTokens
            .Where(t => t.MapId == request.MapId && tokenIds.Contains(t.Id))
            .ToListAsync();

        var positionsMap = request.Tokens.ToDictionary(t => t.TokenId);

        foreach (var token in tokensFromDb)
        {
            if (positionsMap.TryGetValue(token.Id, out var newPos))
            {
                token.CoordX = newPos.CoordX;
                token.CoordY = newPos.CoordY;
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Session ended and token positions saved successfully." });
    }
}