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
public class ItemController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    // --- ITENS ---

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Item>>> GetMyItems()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        return await _context.Items
            .Include(i => i.Group) // Fundamental para mostrar o nome do grupo no card
            .Where(i => i.CreatedBy == userId)
            .OrderByDescending(i => i.Id) 
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Item>> CreateItem(Item item)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        item.CreatedBy = userId.Value;
        item.Creator = null; 
        item.Group = null; // Evita que o EF tente criar um grupo novo se vier ID no JSON

        _context.Items.Add(item);
        await _context.SaveChangesAsync();

        // Recarrega para trazer o nome do Grupo no retorno para o Frontend
        var newItem = await _context.Items
            .Include(i => i.Group)
            .FirstOrDefaultAsync(i => i.Id == item.Id);

        return Ok(newItem);
    }

    // FALTA: Update (Para o botão Edit do Frontend)
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

    // FALTA: Delete (Para o botão Delete do Frontend)
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

    // --- GRUPOS ---

    [HttpGet("groups")] 
    public async Task<ActionResult<IEnumerable<ItemGroup>>> GetGroups()
    {
        return await _context.ItemGroups.ToListAsync();
    }

    [HttpPost("groups")] 
    public async Task<ActionResult<ItemGroup>> CreateGroup([FromBody] ItemGroup group)
    {
        if (string.IsNullOrEmpty(group.Name)) return BadRequest("Name is required");

        _context.ItemGroups.Add(group);
        await _context.SaveChangesAsync();

        return Ok(group);
    }

    private int? GetUserId()
    {
        var claim = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out int id) ? id : null;
    }
}