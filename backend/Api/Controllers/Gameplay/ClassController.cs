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


    [HttpGet("groups")]
    public async Task<ActionResult<IEnumerable<ClassGroup>>> GetGroups()
    {
        return await _context.ClassGroups.ToListAsync();
    }

    [HttpPost("groups")]
    public async Task<ActionResult<ClassGroup>> CreateGroup([FromBody] ClassGroup group)
    {
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