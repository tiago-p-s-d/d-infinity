using Api.Data;
using Api.Models;
using Api.Models.Gameplay;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace Api.Controllers;

public class CampaignCreateDto
{
    public required string CampaignName { get; set; }
    public string? About { get; set; }
    public int SystemId { get; set; }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CampaignController : ControllerBase
{
    private readonly AppDbContext _context;

    public CampaignController(AppDbContext context)
    {
        _context = context;
        // This will show in your terminal when the controller is instantiated
        Console.WriteLine("DEBUG: CampaignController initialized");
    }

    [HttpGet("my-joined-campaigns")] // Changed route name to avoid ANY conflict
    public async Task<IActionResult> GetJoinedCampaigns()
    {
        Console.WriteLine("DEBUG: GetJoinedCampaigns called");
        
        var userId = GetUserId();
        if (userId == null) 
        {
            Console.WriteLine("DEBUG: User unauthorized");
            return Unauthorized();
        }

        var campaigns = await _context.CampaignMembers
            .Where(m => m.UserId == userId && !m.IsDm)
            .Include(m => m.Campaign)
                .ThenInclude(c => c.System)
            .Select(m => new
            {
                CampaignId = m.Campaign.Id,
                m.Campaign.CampaignName,
                m.Campaign.About,
                SystemName = m.Campaign.System.Name,
                SystemId = m.Campaign.System.Id
            })
            .ToListAsync();

        Console.WriteLine($"DEBUG: Found {campaigns.Count} campaigns");
        return Ok(campaigns);
    }

    [HttpGet]
    public async Task<IActionResult> GetCampaigns()
    {
        Console.WriteLine("DEBUG: GetCampaigns (DM) called");
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var campaigns = await _context.CampaignMembers
            .Where(m => m.UserId == userId && m.IsDm)
            .Include(m => m.Campaign)
                .ThenInclude(c => c.System)
            .Select(m => new
            {
                m.Campaign.Id,
                m.Campaign.CampaignName,
                m.Campaign.About,
                m.Campaign.InviteCode,
                m.IsDm,
                System = new
                {
                    m.Campaign.System.Id,
                    m.Campaign.System.Name
                }
            })
            .ToListAsync();

        return Ok(campaigns);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateCampaign([FromBody] CampaignCreateDto dto)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var campaign = new Campaign
        {
            CampaignName = dto.CampaignName,
            About = dto.About,
            SystemId = dto.SystemId,
            InviteCode = Guid.NewGuid().ToString()[..8]
        };

        var membership = new CampaignUser
        {
            UserId = userId.Value,
            Campaign = campaign,
            IsDm = true
        };

        _context.CampaignMembers.Add(membership);
        await _context.SaveChangesAsync();

        return Ok(campaign);
    }

    [HttpPut("update/{id:int}")] // Explicit path
    public async Task<IActionResult> UpdateCampaign(int id, [FromBody] CampaignCreateDto dto)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var membership = await _context.CampaignMembers
            .Include(m => m.Campaign)
            .FirstOrDefaultAsync(m => m.CampaignId == id && m.UserId == userId && m.IsDm);

        if (membership == null) return NotFound();

        membership.Campaign.CampaignName = dto.CampaignName;
        membership.Campaign.About = dto.About;
        membership.Campaign.SystemId = dto.SystemId;

        await _context.SaveChangesAsync();
        return Ok(membership.Campaign);
    }

    [HttpDelete("delete/{id:int}")] // Explicit path
    public async Task<IActionResult> DeleteCampaign(int id)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var membership = await _context.CampaignMembers
            .Include(m => m.Campaign)
            .FirstOrDefaultAsync(m => m.CampaignId == id && m.UserId == userId && m.IsDm);

        if (membership == null) return NotFound();

        _context.Campaigns.Remove(membership.Campaign);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("join/{inviteCode}")]
    public async Task<IActionResult> JoinCampaign(string inviteCode)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var campaign = await _context.Campaigns
            .Include(c => c.System)
            .FirstOrDefaultAsync(c => c.InviteCode == inviteCode);

        if (campaign == null) return NotFound("Invalid invite code.");

        var exists = await _context.CampaignMembers
            .AnyAsync(m => m.CampaignId == campaign.Id && m.UserId == userId);

        if (exists) return BadRequest("You are already a member of this campaign.");

        var sheetModel = await _context.CharacterSheetModels
            .FirstOrDefaultAsync(m => m.Id == campaign.System.CharacterSheetId);

        var defaultValues = new Dictionary<string, object?>();
        if (sheetModel != null)
        {
            var definitions = JsonSerializer.Deserialize<List<JsonElement>>(sheetModel.Definitions);
            if (definitions != null)
            {
                foreach (var field in definitions)
                {
                    var name = field.TryGetProperty("name", out var n) ? n.GetString() : null;
                    var type = field.TryGetProperty("type", out var t) ? t.GetString() : "text";

                    if (name == null) continue;

                    defaultValues[name] = type switch
                    {
                        "number" => 0,
                        _ => ""
                    };
                }
            }
        }

        var membership = new CampaignUser
        {
            UserId = userId.Value,
            CampaignId = campaign.Id,
            IsDm = false
        };
        _context.CampaignMembers.Add(membership);

        if (sheetModel != null)
        {
            var sheet = new CharacterSheet
            {
                CharacterName = "New Character",
                ModelId = sheetModel.Id,
                PlayerId = userId.Value,
                CampaignId = campaign.Id,
                Values = JsonSerializer.Serialize(defaultValues)
            };
            _context.CharacterSheets.Add(sheet);
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            systemId = campaign.SystemId,
            campaignId = campaign.Id,
            message = "Successfully joined the campaign."
        });
    }

    private int? GetUserId()
    {
        var claim = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out int id) ? id : null;
    }
}