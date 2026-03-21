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
public class RacesController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Race>>> GetMyRaces()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        return await _context.Races
            .Include(r => r.Group) 
            .Where(r => r.CreatedBy == userId)
            .OrderByDescending(r => r.Id) 
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Race>> CreateRace(Race race)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        race.CreatedBy = userId.Value;
        race.Creator = null; 
        race.Group = null;

        _context.Races.Add(race);
        await _context.SaveChangesAsync();

  
        var newRace = await _context.Races
            .Include(r => r.Group)
            .FirstOrDefaultAsync(r => r.Id == race.Id);

        return Ok(newRace);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRace(int id, Race race)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var existingRace = await _context.Races.FindAsync(id);

        if (existingRace == null) return NotFound();
        if (existingRace.CreatedBy != userId) return Forbid();

        existingRace.Name = race.Name;
        existingRace.About = race.About;
        existingRace.Modifiers = race.Modifiers;
        existingRace.RaceGroupId = race.RaceGroupId;

        await _context.SaveChangesAsync();
        return Ok(existingRace);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRace(int id)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var race = await _context.Races.FindAsync(id);

        if (race == null) return NotFound();
        if (race.CreatedBy != userId) return Forbid();

        _context.Races.Remove(race);
        await _context.SaveChangesAsync();

        return NoContent();
    }


    [HttpGet("groups")] 
    public async Task<ActionResult<IEnumerable<RaceGroup>>> GetGroups()
    {
        return await _context.RaceGroups.ToListAsync();
    }

    [HttpPost("groups")]
    public async Task<ActionResult<RaceGroup>> CreateGroup([FromBody] RaceGroup group)
    {
        if (string.IsNullOrEmpty(group.Name)) return BadRequest("Name is required");

        _context.RaceGroups.Add(group);
        await _context.SaveChangesAsync();

        return Ok(group);
    }

    private int? GetUserId()
    {
        var claim = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out int id) ? id : null;
    }
}