using Api.Data;
using Api.Models.Gameplay;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers.Gameplay;

[ApiController]
[Route("api/[controller]")]
public class CurrencyController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Currency>>> GetCurrencies()
    {
        return await _context.Currencies
            .Include(c => c.Values)
            .OrderByDescending(c => c.Id)
            .ToListAsync();
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Currency>> Create([FromBody] Currency currency)
    {
        try 
        {
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

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var currency = await _context.Currencies
            .Include(c => c.Values)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (currency == null) return NotFound();

        _context.Currencies.Remove(currency);
        await _context.SaveChangesAsync();
        return NoContent();
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