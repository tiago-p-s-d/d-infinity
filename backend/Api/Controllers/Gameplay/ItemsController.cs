using Api.Data;
using Api.Models.Gameplay;
using Api.Models.Gameplay.Groups;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers.Gameplay;

[ApiController]
[Route("api/[controller]")]
public class ItemController(AppDbContext context) : BaseGameplayController
{
    private readonly AppDbContext _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Item>>> GetMyItems()
    {
        var userId = GetUserId();
        
        return await _context.Items
            .Include(i => i.Group) 
            .Where(i => i.CreatedBy == userId) 
            .OrderByDescending(i => i.Id) 
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Item>> CreateItem(Item item)
    {
        var userId = GetUserId();

        item.CreatedBy = userId;
        item.Creator = null; 
        item.Group = null; 

        _context.Items.Add(item);
        await _context.SaveChangesAsync();

        var newItem = await _context.Items
            .Include(i => i.Group)
            .FirstOrDefaultAsync(i => i.Id == item.Id);

        return Ok(newItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateItem(int id, Item item)
    {
        var userId = GetUserId();
        var existingItem = await _context.Items.FindAsync(id);

        if (existingItem == null) return NotFound();
        if (existingItem.CreatedBy != userId) return Forbid();

        existingItem.Name = item.Name;
        existingItem.Description = item.Description;
        existingItem.ItemGroupId = item.ItemGroupId;
        existingItem.Definitions = item.Definitions;

        await _context.SaveChangesAsync();
        return Ok(existingItem);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var userId = GetUserId();
        var item = await _context.Items.FindAsync(id);

        if (item == null) return NotFound();
        if (item.CreatedBy != userId) return Forbid();

        _context.Items.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("groups")] 
    public async Task<ActionResult<IEnumerable<ItemGroup>>> GetGroups()
    {
        var userId = GetUserId();
        return await _context.ItemGroups
            .Where(g => g.CreatedBy == userId)
            .ToListAsync();
    }

    [HttpPost("groups")] 
    public async Task<ActionResult<ItemGroup>> CreateGroup([FromBody] ItemGroup group)
    {
        if (string.IsNullOrEmpty(group.Name)) return BadRequest("Name is required");

        var userId = GetUserId();
        group.CreatedBy = userId;

        _context.ItemGroups.Add(group);
        await _context.SaveChangesAsync();

        return Ok(group);
    }
}