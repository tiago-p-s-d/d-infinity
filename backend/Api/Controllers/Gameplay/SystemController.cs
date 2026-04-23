using Api.Data;
using Api.Models.Gameplay;
using Api.Models.Gameplay.Groups;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Api.Controllers.Gameplay;

public class SystemCreateDto
{
    public string Name { get; set; } = string.Empty;
    public int CharacterSheetId { get; set; }
    public int CurrencyId { get; set; }
    public int? ClassesId { get; set; }
    public List<int> ItemsId { get; set; } = [];
    public List<int> SpellsId { get; set; } = [];
    public List<int> SkillsId { get; set; } = [];
    public List<int> RacesId { get; set; } = [];
    public List<int> MapsId { get; set; } = [];
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SystemController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SystemModel>>> GetMySystems()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();
        return await _context.Systems
            .Include(s => s.SystemRaces)
            .Include(s => s.SystemItems)
            .Include(s => s.SystemSpells)
            .Include(s => s.SystemSkills)
            .Include(s => s.SystemMaps)
            .Where(s => s.CreatedBy == userId)
            .OrderByDescending(s => s.Id)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<SystemModel>> CreateSystem(SystemCreateDto dto)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var system = new SystemModel
        {
            Name = dto.Name,
            CharacterSheetId = dto.CharacterSheetId,
            CurrencyId = dto.CurrencyId,
            ClassesId = dto.ClassesId,
            CreatedBy = userId.Value,

            // Convertemos os arrays de IDs do Angular para as entidades de ligação
            SystemRaces = dto.RacesId.Select(id => new SystemRaceGroup { RaceGroupId = id }).ToList(),
            SystemItems = dto.ItemsId.Select(id => new SystemItemGroup { ItemGroupId = id }).ToList(),
            SystemSpells = dto.SpellsId.Select(id => new SystemSpellGroup { SpellGroupId = id }).ToList(),
            SystemSkills = dto.SkillsId.Select(id => new SystemSkillGroup { SkillGroupId = id }).ToList(),
            SystemMaps = dto.MapsId.Select(id => new SystemMapGroup { MapGroupId = id }).ToList()
        };

        _context.Systems.Add(system);
        await _context.SaveChangesAsync();

        return Ok(system);
    }

    private int? GetUserId()
    {
        var claim = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out int id) ? id : null;
    }
}