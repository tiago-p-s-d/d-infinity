using Api.Data;
using Api.Models.Gameplay;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Api.Controllers.Gameplay;

[ApiController]
[Route("api/[controller]")]
public class CharacterSheetModelController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CharacterSheetModel>>> GetMyModels()
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim == null) return Unauthorized();
        
        if (!int.TryParse(userIdClaim, out int userId)) return BadRequest("Invalid User ID.");

        return await _context.CharacterSheetModels
            .Where(m => m.CreatedBy == userId)
            .OrderByDescending(m => m.Id)
            .ToListAsync();
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<CharacterSheetModel>> GetById(int id)
    {
        var model = await _context.CharacterSheetModels.FindAsync(id);

        if (model == null) return NotFound();

        return model;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CharacterSheetModel>> Create(CharacterSheetModel model)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim == null) return Unauthorized();
        
        var userId = int.Parse(userIdClaim);


        model.CreatedBy = userId;
        model.Creator = null; 

        _context.CharacterSheetModels.Add(model);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CharacterSheetModel model)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);

        if (id != model.Id) return BadRequest();


        var exists = await _context.CharacterSheetModels
            .AnyAsync(m => m.Id == id && m.CreatedBy == userId);

        if (!exists) return NotFound("Model not found or access denied.");

        model.CreatedBy = userId;
        model.Creator = null;
        
        _context.Entry(model).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(model);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);

        var model = await _context.CharacterSheetModels
            .FirstOrDefaultAsync(m => m.Id == id && m.CreatedBy == userId);

        if (model == null) return NotFound();

        _context.CharacterSheetModels.Remove(model);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}