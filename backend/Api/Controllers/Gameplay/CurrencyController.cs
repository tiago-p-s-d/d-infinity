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
public class CurrencyController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Currency>>> GetCurrencies()
    {
        var userId = GetUserId();
        return await _context.Currencies
            .Include(c => c.Values)
            .Where(c => c.CreatedBy == userId)
            .OrderByDescending(c => c.Id)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Currency>> Create([FromBody] Currency currency)
    {
        try 
        {
            var userId = GetUserId();
            currency.CreatedBy = userId ?? 0;
            var preparedCurrency = PrepareCurrencyPayload(currency);

            _context.Currencies.Add(preparedCurrency);
            await _context.SaveChangesAsync();
            
            var result = await _context.Currencies
                .Include(c => c.Values)
                .FirstOrDefaultAsync(c => c.Id == preparedCurrency.Id);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        var currency = await _context.Currencies
            .Include(c => c.Values)
            .FirstOrDefaultAsync(c => c.Id == id && c.CreatedBy == userId);

        if (currency == null) return NotFound();

        _context.Currencies.Remove(currency);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private int? GetUserId()
    {
        var claim = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out int id) ? id : null;
    }

    private Currency PrepareCurrencyPayload(Currency incoming)
    {
        incoming.Id = 0;
        if (incoming.Values != null)
        {
            foreach (var val in incoming.Values)
            {
                val.Id = 0;
                val.CurrencyId = 0;
                val.Currency = null; 
            }
        }
        else 
        {
            incoming.Values = new List<CurrencyValue>();
        }
        return incoming;
    }
}