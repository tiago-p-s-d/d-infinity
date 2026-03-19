using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Data; 
using Api.Models.Gameplay;

namespace Api.Controllers.Gameplay;

[ApiController]
[Route("api/[controller]")]
public class CharacterSheetsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CharacterSheetsController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<CharacterSheet>> GetCharacterSheet(int id)
    {
        var characterSheet = await _context.CharacterSheets
            .Include(c => c.Race)           
            .Include(c => c.KnownSkills)    
            .Include(c => c.KnownSpells)   
            .FirstOrDefaultAsync(c => c.Id == id);

        if (characterSheet == null)
        {
            return NotFound(new { message = "Personagem não encontrado" });
        }

        return characterSheet;
    }


    [HttpPost]
    public async Task<ActionResult<CharacterSheet>> PostCharacterSheet(CharacterSheet characterSheet)
    {
        _context.CharacterSheets.Add(characterSheet);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCharacterSheet), new { id = characterSheet.Id }, characterSheet);
    }
}