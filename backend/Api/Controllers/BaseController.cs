using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Api.Controllers; 

[Authorize]
public class BaseGameplayController : ControllerBase
{
    protected int GetUserId()
    {
        var claim = User.FindFirst("id")?.Value 
                    ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return int.TryParse(claim, out int id) ? id : 0;
    }
}