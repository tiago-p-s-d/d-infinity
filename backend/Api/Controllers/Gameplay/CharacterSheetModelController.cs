using Api.Data;
using Api.Models.Gameplay;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Api.Controllers.Gameplay;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CharacterSheetModelController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CharacterSheetModel>>> GetMyModels()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        return await _context.CharacterSheetModels
            .Where(m => m.CreatedBy == userId)
            .OrderByDescending(m => m.Id)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CharacterSheetModel>> GetById(int id)
    {
        var userId = GetUserId();
        var model = await _context.CharacterSheetModels
            .FirstOrDefaultAsync(m => m.Id == id && m.CreatedBy == userId);

        if (model == null) return NotFound();

        return model;
    }

    [HttpPost]
    public async Task<ActionResult<CharacterSheetModel>> Create(CharacterSheetModel model)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        model.CreatedBy = userId.Value;
        model.Creator = null; 

        _context.CharacterSheetModels.Add(model);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CharacterSheetModel model)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        if (id != model.Id) return BadRequest();

        var exists = await _context.CharacterSheetModels
            .AnyAsync(m => m.Id == id && m.CreatedBy == userId);

        if (!exists) return NotFound("Model not found or access denied.");

        model.CreatedBy = userId.Value;
        model.Creator = null;
        
        _context.Entry(model).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(model);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var model = await _context.CharacterSheetModels
            .FirstOrDefaultAsync(m => m.Id == id && m.CreatedBy == userId);

        if (model == null) return NotFound();

        _context.CharacterSheetModels.Remove(model);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private int? GetUserId()
    {
        var claim = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out int id) ? id : null;
    }
}