using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class AuthController(AppDbContext context, IConfiguration config) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly IConfiguration _config = config;


    [HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterDto request)
{
    if (await _context.Usuarios.AnyAsync(u => u.Email == request.Email))
    {
        return BadRequest(new { message = "Este e-mail já está cadastrado." });
    }

    var novoUsuario = new Usuario
    {
        Nome = request.Nome,
        Email = request.Email,
        SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha)
    };

    _context.Usuarios.Add(novoUsuario);
    await _context.SaveChangesAsync();

    return Ok(new { message = "Usuário criado com sucesso!" });
}








    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] RegisterDto request)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (usuario == null)
        {
            return Unauthorized(new { message = "Email inválido." });
        }
        if (!BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
        {
            return Unauthorized(new { message = "Senha inválida." });
        }
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(3),
            signingCredentials: creds
        );
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { token = jwt, message = "Login realizado!" });
    }

}

public class RegisterDto
{
    public required string Nome { get; set; }
    public required string Email { get; set; }
    public required string Senha { get; set; }

}

