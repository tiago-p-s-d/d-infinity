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
public class SkillController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Skill>>> GetMySkills()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        return await _context.Skills
            .Include(s => s.Group) 
            .Where(s => s.CreatedBy == userId)
            .OrderByDescending(s => s.Id)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Skill>> CreateSkill(Skill skill)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        skill.CreatedBy = userId.Value;
        skill.Creator = null;
        skill.Group = null; 

        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        var newSkill = await _context.Skills
            .Include(s => s.Group)
            .FirstOrDefaultAsync(s => s.Id == skill.Id);

        return Ok(newSkill);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSkill(int id, Skill skill)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var existingSkill = await _context.Skills.FindAsync(id);

        if (existingSkill == null) return NotFound();
        if (existingSkill.CreatedBy != userId) return Forbid();

        existingSkill.Name = skill.Name;
        existingSkill.About = skill.About;
        existingSkill.Effect = skill.Effect;
        existingSkill.SkillGroupId = skill.SkillGroupId;

        await _context.SaveChangesAsync();
        return Ok(existingSkill);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSkill(int id)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var skill = await _context.Skills.FindAsync(id);

        if (skill == null) return NotFound();
        if (skill.CreatedBy != userId) return Forbid();

        _context.Skills.Remove(skill);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("groups")] 
    public async Task<ActionResult<IEnumerable<SkillGroup>>> GetGroups()
    {
        return await _context.SkillGroups.ToListAsync();
    }

    [HttpPost("groups")] 
    public async Task<ActionResult<SkillGroup>> CreateGroup([FromBody] SkillGroup group)
    {
        if (string.IsNullOrEmpty(group.Name)) return BadRequest("Name is required");

        _context.SkillGroups.Add(group);
        await _context.SaveChangesAsync();

        return Ok(group);
    }


    private int? GetUserId()
    {
        var claim = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out int id) ? id : null;
    }
}