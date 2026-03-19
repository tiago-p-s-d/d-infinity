using Api.Data;
using Api.Models.Gameplay;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Api.Controllers.Gameplay;

[ApiController]
[Route("api/[controller]")]
public class SkillController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Skill>>> GetMySkills()
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim == null) return Unauthorized();
        
        if (!int.TryParse(userIdClaim, out int userId)) return BadRequest("Invalid User ID.");

        return await _context.Skills
            .Where(s => s.CreatedBy == userId)
            .OrderByDescending(s => s.Id)
            .ToListAsync();
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Skill>> CreateSkill(Skill skill)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim == null) return Unauthorized();
        
        var userId = int.Parse(userIdClaim);

        skill.CreatedBy = userId;

        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMySkills), new { id = skill.Id }, skill);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSkill(int id, Skill skill)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);

        if (id != skill.Id) return BadRequest();

        var exists = await _context.Skills
            .AnyAsync(s => s.Id == id && s.CreatedBy == userId);

        if (!exists) return NotFound();

        skill.CreatedBy = userId;
        skill.Creator = null;
        
        _context.Entry(skill).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(skill);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSkill(int id)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);

        var skill = await _context.Skills
            .FirstOrDefaultAsync(s => s.Id == id && s.CreatedBy == userId);

        if (skill == null) return NotFound();

        _context.Skills.Remove(skill);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}