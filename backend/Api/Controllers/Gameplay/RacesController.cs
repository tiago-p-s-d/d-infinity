using Api.Data;
using Api.Models.Gameplay;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Api.Controllers.Gameplay;

[ApiController]
[Route("api/[controller]")]
public class RacesController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Race>>> GetMyRaces()
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim == null) return Unauthorized();
        
        if (!int.TryParse(userIdClaim, out int userId)) return BadRequest("Invalid User ID.");

        return await _context.Races
            .Where(r => r.CreatedBy == userId)
            .OrderByDescending(r => r.Id) 
            .ToListAsync();
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Race>> CreateRace(Race race)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim == null) return Unauthorized();
        
        var userId = int.Parse(userIdClaim);

        race.CreatedBy = userId;
        race.Creator = null; 

        _context.Races.Add(race);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMyRaces), new { id = race.Id }, race);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRace(int id, Race race)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);

        if (id != race.Id) return BadRequest();

        var exists = await _context.Races
            .AnyAsync(r => r.Id == id && r.CreatedBy == userId);

        if (!exists) return NotFound();

        race.CreatedBy = userId;
        race.Creator = null;
        
        _context.Entry(race).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(race);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRace(int id)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);

        var race = await _context.Races
            .FirstOrDefaultAsync(r => r.Id == id && r.CreatedBy == userId);

        if (race == null) return NotFound();

        _context.Races.Remove(race);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}