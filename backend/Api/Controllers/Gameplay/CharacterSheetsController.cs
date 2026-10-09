using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.Json;
using Api.Data;
using Api.Models.Gameplay;

namespace Api.Controllers.Gameplay;

public class CharacterSheetUpdateDto
{
    public required string CharacterName { get; set; }
    public string Values { get; set; } = "{}";
}

[ApiController]
[Route("api/character-sheet")]
[Authorize]
public class CharacterSheetsController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;
    [HttpGet]
    public async Task<IActionResult> GetUserCharacterSheets([FromQuery] int? campaignId = null)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var query = _context.CharacterSheets
            .Include(s => s.Model)
            .Where(s => s.PlayerId == userId.Value);

        if (campaignId.HasValue)
        {
            query = query.Where(s => s.CampaignId == campaignId.Value);
        }

        var sheets = await query
            .Select(sheet => new
            {
                sheet.Id,
                sheet.CharacterName,
                sheet.CampaignId,
                sheet.Values,
                Model = sheet.Model != null ? new
                {
                    sheet.Model.Id,
                    sheet.Model.Name,
                    Definitions = JsonSerializer.Deserialize<object>(sheet.Model.Definitions, (JsonSerializerOptions?)null)
                } : null
            })
            .ToListAsync();

        return Ok(sheets);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CharacterSheet>> GetCharacterSheet(int id)
    {
        var characterSheet = await _context.CharacterSheets
            .Include(c => c.Race)
            .Include(c => c.KnownSkills)
            .Include(c => c.KnownSpells)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (characterSheet == null)
            return NotFound(new { message = "Character not found." });

        return Ok(characterSheet);
    }

    [HttpGet("campaign/{campaignId:int}")]
    public async Task<IActionResult> GetSheetByCampaign(int campaignId)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var sheet = await _context.CharacterSheets
            .Include(s => s.Model)
            .FirstOrDefaultAsync(s => s.CampaignId == campaignId && s.PlayerId == userId);

        if (sheet == null) return Ok(null);

        return Ok(new
        {
            sheet.Id,
            sheet.CharacterName,
            sheet.CampaignId,
            sheet.Values,
            Model = sheet.Model != null ? new
            {
                sheet.Model.Id,
                sheet.Model.Name,
                Definitions = JsonSerializer.Deserialize<object>(sheet.Model.Definitions, (JsonSerializerOptions?)null)
            } : null
        });
    }

    /// <summary>
    /// POST /api/character-sheet
    /// Criação de nova ficha (RESTful: POST apenas para criar).
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CharacterSheet>> PostCharacterSheet(CharacterSheet characterSheet)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        characterSheet.PlayerId = userId.Value;

        _context.CharacterSheets.Add(characterSheet);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCharacterSheet), new { id = characterSheet.Id }, characterSheet);
    }

    /// <summary>
    /// PUT /api/character-sheet/{id}
    /// Atualização de uma ficha existente.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateSheet(int id, [FromBody] CharacterSheetUpdateDto dto)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var sheet = await _context.CharacterSheets
            .FirstOrDefaultAsync(s => s.Id == id && s.PlayerId == userId);

        if (sheet == null) return NotFound();

        sheet.CharacterName = dto.CharacterName;
        sheet.Values = dto.Values;

        await _context.SaveChangesAsync();
        return Ok(sheet);
    }

    private int? GetUserId()
    {
        var claim = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out int id) ? id : null;
    }
}