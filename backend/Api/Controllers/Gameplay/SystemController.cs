using Api.Data;
using Api.Models.Gameplay;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers.Gameplay;

[ApiController]
[Route("api/[controller]")]
public class SystemController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;


    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SystemModel>>> GetMySystems()
    {
       
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);

        return await _context.Systems
            .Where(s => s.CreatedBy == userId)
            .ToListAsync();
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<SystemModel>> CreateSystem(SystemModel system)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        var userId = int.Parse(userIdClaim!);


        system.CreatedBy = userId;

        _context.Systems.Add(system);
        await _context.SaveChangesAsync();

        return Ok(system);
    }
}