using Api.Data;
using Api.Models.Gameplay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Api.Controllers.Gameplay;

public class CreateTokenRequestDto
{
    public required string Name { get; set; }
    public required string TokenImage { get; set; } // Base64
    public int MapId { get; set; }
    public int? CharacterSheetId { get; set; }
    public int? OwnerUserId { get; set; }
    public double CoordX { get; set; } = 50;
    public double CoordY { get; set; } = 50;
    public double Size { get; set; } = 1.0;
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MapTokensController : ControllerBase
{
    private readonly AppDbContext _context;

    public MapTokensController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("map/{mapId:int}")]
    public async Task<IActionResult> GetTokensByMap(int mapId)
    {
        var tokens = await _context.MapTokens
            .Where(t => t.MapId == mapId)
            .Select(t => new
            {
                t.Id,
                t.Name,
                // Converte byte[] para Data URL Base64 para consumo direto no [src] da tag <img>
                TokenImage = "data:image/png;base64," + Convert.ToBase64String(t.TokenImage),
                t.CoordX,
                t.CoordY,
                t.Size,
                t.MapId,
                t.CharacterSheetId,
                t.OwnerUserId
            })
            .ToListAsync();

        return Ok(tokens);
    }

    [HttpPost]
    public async Task<IActionResult> CreateToken([FromBody] CreateTokenRequestDto dto)
    {
        try
        {
            var userIdClaim = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var authUserId))
            {
                return Unauthorized(new { message = "Invalid or missing user authentication token." });
            }

            var resolvedOwnerId = dto.OwnerUserId ?? authUserId;

            // Se for jogador, tenta resolver a ficha caso tenha vindo nula
            int? resolvedSheetId = dto.CharacterSheetId;
            if (resolvedSheetId == null || resolvedSheetId <= 0)
            {
                var sheet = await _context.CharacterSheets
                    .FirstOrDefaultAsync(s => s.PlayerId == resolvedOwnerId);
                resolvedSheetId = sheet?.Id;
            }

            if (resolvedSheetId == null)
            {
                return BadRequest(new { message = "Character Sheet not found for this user. A sheet is required to bind the token." });
            }

            // Sanitiza e converte a string Base64 em byte[] Blob
            var base64Data = dto.TokenImage;
            if (base64Data.Contains(","))
            {
                base64Data = base64Data.Substring(base64Data.IndexOf(",") + 1);
            }
            byte[] imageBytes = Convert.FromBase64String(base64Data);

            var token = new MapTokenModel
            {
                Name = dto.Name,
                TokenImage = imageBytes,
                MapId = dto.MapId,
                CharacterSheetId = resolvedSheetId,
                OwnerUserId = resolvedOwnerId,
                CoordX = dto.CoordX,
                CoordY = dto.CoordY,
                Size = dto.Size
            };

            _context.MapTokens.Add(token);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTokensByMap), new { mapId = token.MapId }, new
            {
                token.Id,
                token.Name,
                TokenImage = "data:image/png;base64," + Convert.ToBase64String(token.TokenImage),
                token.CoordX,
                token.CoordY,
                token.Size,
                token.MapId,
                token.CharacterSheetId,
                token.OwnerUserId
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the token.", error = ex.Message });
        }
    }
}