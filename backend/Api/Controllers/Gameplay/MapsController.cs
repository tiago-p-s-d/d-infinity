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
public class MapsController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MapModel>>> GetMyMaps()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        return await _context.Maps
            .Include(m => m.MapGroup)
            .Where(m => m.CreatedBy == userId)
            .OrderByDescending(m => m.Id)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<MapModel>> CreateMap(MapModel map)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        map.CreatedBy = userId.Value;
        map.Creator = null;
        map.MapGroup = null;

        _context.Maps.Add(map);
        await _context.SaveChangesAsync();

        var newMap = await _context.Maps
            .Include(m => m.MapGroup)
            .FirstOrDefaultAsync(m => m.Id == map.Id);

        return Ok(newMap);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMap(int id, MapModel map)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var existingMap = await _context.Maps.FindAsync(id);

        if (existingMap == null) return NotFound();
        if (existingMap.CreatedBy != userId) return Forbid();

        existingMap.Name = map.Name;
        existingMap.MapImage = map.MapImage;
        existingMap.MapGroupId = map.MapGroupId;

        await _context.SaveChangesAsync();
        return Ok(existingMap);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMap(int id)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var map = await _context.Maps.FindAsync(id);

        if (map == null) return NotFound();
        if (map.CreatedBy != userId) return Forbid();

        _context.Maps.Remove(map);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // --- Map Groups Endpoints ---

    [HttpGet("groups")]
    public async Task<ActionResult<IEnumerable<MapGroup>>> GetGroups()
    {
        return await _context.MapGroups.ToListAsync();
    }

    [HttpPost("groups")]
    public async Task<ActionResult<MapGroup>> CreateGroup([FromBody] MapGroup group)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        if (string.IsNullOrEmpty(group.Name)) return BadRequest("Name is required");

        group.CreatedBy = userId;
        group.Creator = null;

        _context.MapGroups.Add(group);
        await _context.SaveChangesAsync();

        return Ok(group);
    }

    private int? GetUserId()
    {
        var claim = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out int id) ? id : null;
    }
}