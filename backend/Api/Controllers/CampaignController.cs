using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; // Necessário para o [Authorize]

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CampaignController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [Authorize]
    [HttpPost("start-your-own")]
    public async Task<IActionResult> StartYourOwn()
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);

        var campaign = new Campaign
        {
            CampaignName = ""
        };

        var membership = new CampaignUser
        {
            UserId = userId,
            Campaign = campaign, 
            IsDm = true
        };

        _context.CampaignMembers.Add(membership);
        await _context.SaveChangesAsync();

        return Ok(new { 
            id = campaign.Id, 
            message = "Campaign initialized with you as Dungeon Master." 
        });
    }
}