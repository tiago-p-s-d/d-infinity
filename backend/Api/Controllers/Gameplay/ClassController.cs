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
public class ClassController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClassModel>>> GetMyClasses()
    {
        var userId = GetUserId();
        return await _context.Classes
            .Include(c => c.Group)
            .Where(c => c.CreatedBy == userId)
            .OrderByDescending(c => c.Id)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<ClassModel>> CreateClass(ClassModel classModel)
    {
        var userId = GetUserId();
        classModel.CreatedBy = userId ?? 0;
        classModel.Creator = null;
        classModel.Group = null;

        _context.Classes.Add(classModel);
        await _context.SaveChangesAsync();

        var result = await _context.Classes
            .Include(c => c.Group)
            .FirstOrDefaultAsync(c => c.Id == classModel.Id);

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClass(int id, ClassModel classModel)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var existingClass = await _context.Classes.FindAsync(id);

        if (existingClass == null) return NotFound();
        if (existingClass.CreatedBy != userId) return Forbid();

        // Atualize os campos necessários da sua ClassModel aqui
        existingClass.Name = classModel.Name;
        existingClass.About = classModel.About;
        existingClass.ClassGroupId = classModel.ClassGroupId;
        // Se houver campos como Modifiers ou Effects, adicione-os aqui

        await _context.SaveChangesAsync();
        
        // Retornamos o objeto atualizado com o grupo incluído para o frontend
        var result = await _context.Classes
            .Include(c => c.Group)
            .FirstOrDefaultAsync(c => c.Id == id);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClass(int id)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var classEntry = await _context.Classes.FindAsync(id);

        if (classEntry == null) return NotFound();
        if (classEntry.CreatedBy != userId) return Forbid();

        _context.Classes.Remove(classEntry);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("groups")]
    public async Task<ActionResult<IEnumerable<ClassGroup>>> GetGroups()
    {
        var userId = GetUserId();
        return await _context.ClassGroups
            .Where(g => g.CreatedBy == userId)
            .ToListAsync();
    }

    [HttpPost("groups")]
    public async Task<ActionResult<ClassGroup>> CreateGroup([FromBody] ClassGroup group)
    {
        var userId = GetUserId();
        group.CreatedBy = userId ?? 0;
        
        _context.ClassGroups.Add(group);
        await _context.SaveChangesAsync();
        return Ok(group);
    }

    private int? GetUserId()
    {
        var claim = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out int id) ? id : null;
    }
}