using Api.Data;
using Api.Models.Gameplay;
using Api.Models.Gameplay.Groups;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Api.Controllers.Gameplay;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SpellController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Spell>>> GetMySpells()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        return await _context.Spells
            .Include(s => s.Group) 
            .Where(s => s.CreatedBy == userId)
            .OrderByDescending(s => s.Id)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Spell>> CreateSpell(Spell spell)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        spell.CreatedBy = userId.Value;
        spell.Creator = null; 
        spell.Group = null;

        _context.Spells.Add(spell);
        await _context.SaveChangesAsync();

        var newSpell = await _context.Spells
            .Include(s => s.Group)
            .FirstOrDefaultAsync(s => s.Id == spell.Id);

        return Ok(newSpell);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSpell(int id, Spell spell)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var existingSpell = await _context.Spells.FindAsync(id);

        if (existingSpell == null) return NotFound();
        if (existingSpell.CreatedBy != userId) return Forbid();

        existingSpell.Name = spell.Name;
        existingSpell.About = spell.About;
        existingSpell.Effect = spell.Effect;
        existingSpell.SpellGroupId = spell.SpellGroupId;

        await _context.SaveChangesAsync();
        return Ok(existingSpell);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSpell(int id)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var spell = await _context.Spells.FindAsync(id);

        if (spell == null) return NotFound();
        if (spell.CreatedBy != userId) return Forbid();

        _context.Spells.Remove(spell);
        await _context.SaveChangesAsync();

        return NoContent();
    }


    [HttpGet("groups")]
    public async Task<ActionResult<IEnumerable<SpellGroup>>> GetGroups()
    {
        return await _context.SpellGroups.ToListAsync();
    }

    [HttpPost("groups")] 
    public async Task<ActionResult<SpellGroup>> CreateGroup([FromBody] SpellGroup group)
    {
        if (string.IsNullOrEmpty(group.Name)) return BadRequest("Name is required");

        _context.SpellGroups.Add(group);
        await _context.SaveChangesAsync();

        return Ok(group);
    }

    private int? GetUserId()
    {
        var claim = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out int id) ? id : null;
    }
}