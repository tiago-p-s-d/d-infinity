using Api.Data;
using Api.Models.Gameplay;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Api.Controllers.Gameplay;

[ApiController]
[Route("api/[controller]")]
public class ItemController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Item>>> GetMyItems()
    {

        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim == null) return Unauthorized();
        
        if (!int.TryParse(userIdClaim, out int userId)) return BadRequest("Invalid User ID.");

        return await _context.Items
            .Where(i => i.CreatedBy == userId)
            .OrderByDescending(i => i.Id) 
            .ToListAsync();
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Item>> CreateItem(Item item)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim == null) return Unauthorized();
        
        var userId = int.Parse(userIdClaim);

        item.CreatedBy = userId;
        item.Creator = null; 

        _context.Items.Add(item);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMyItems), new { id = item.Id }, item);
    }
}