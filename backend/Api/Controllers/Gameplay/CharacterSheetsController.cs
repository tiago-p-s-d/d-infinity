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
[Route("api/[controller]")]
[Authorize]
public class CharacterSheetsController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpGet("{id}")]
    public async Task<ActionResult<CharacterSheet>> GetCharacterSheet(int id)
    {
        var characterSheet = await _context.CharacterSheets
            .Include(c => c.Race)
            .Include(c => c.KnownSkills)
            .Include(c => c.KnownSpells)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (characterSheet == null)
            return NotFound(new { message = "Character not found." });

        return characterSheet;
    }

    [HttpGet("campaign/{campaignId}")]
    public async Task<IActionResult> GetSheetByCampaign(int campaignId)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var sheet = await _context.CharacterSheets
            .Include(s => s.Model)
            .FirstOrDefaultAsync(s => s.CampaignId == campaignId && s.PlayerId == userId);

        if (sheet == null) return NotFound();

        return Ok(new
        {
            sheet.Id,
            sheet.CharacterName,
            sheet.CampaignId,
            sheet.Values,
            Model = new
            {
                sheet.Model!.Id,
                sheet.Model.Name,
                Definitions = JsonSerializer.Deserialize<object>(sheet.Model.Definitions)
            }
        });
    }

    [HttpPost]
    public async Task<ActionResult<CharacterSheet>> PostCharacterSheet(CharacterSheet characterSheet)
    {
        _context.CharacterSheets.Add(characterSheet);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCharacterSheet), new { id = characterSheet.Id }, characterSheet);
    }

    [HttpPut("{id}")]
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