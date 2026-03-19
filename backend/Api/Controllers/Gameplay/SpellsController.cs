using Api.Data;
using Api.Models.Gameplay;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Api.Controllers.Gameplay;

[ApiController]
[Route("api/[controller]")]
public class SpellController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Spell>>> GetMySpells()
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim == null) return Unauthorized();
        
        if (!int.TryParse(userIdClaim, out int userId)) return BadRequest("Invalid User ID.");

        return await _context.Spells
            .Where(s => s.CreatedBy == userId)
            .OrderByDescending(s => s.Id)
            .ToListAsync();
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Spell>> CreateSpell(Spell spell)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim == null) return Unauthorized();
        
        var userId = int.Parse(userIdClaim);

        spell.CreatedBy = userId;
        spell.Creator = null; 

        _context.Spells.Add(spell);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMySpells), new { id = spell.Id }, spell);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSpell(int id, Spell spell)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);

        if (id != spell.Id) return BadRequest();

        var exists = await _context.Spells
            .AnyAsync(s => s.Id == id && s.CreatedBy == userId);

        if (!exists) return NotFound();

        spell.CreatedBy = userId;
        spell.Creator = null;
        
        _context.Entry(spell).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(spell);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSpell(int id)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);

        var spell = await _context.Spells
            .FirstOrDefaultAsync(s => s.Id == id && s.CreatedBy == userId);

        if (spell == null) return NotFound();

        _context.Spells.Remove(spell);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}