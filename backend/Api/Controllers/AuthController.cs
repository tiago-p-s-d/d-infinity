using Api.Data;
using Api.Models.User; 
using Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.ComponentModel.DataAnnotations;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AppDbContext context, IConfiguration config, EmailService emailService) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly IConfiguration _config = config;
    private readonly EmailService _emailService = emailService;

    [HttpPost("send-code")]
    public async Task<IActionResult> SendCode([FromBody] string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return BadRequest("Email is required.");

        var code = new Random().Next(100000, 999999).ToString();

        var verification = new UserVerification
        {
            Email = email,
            Code = code,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false
        };

        _context.UserVerifications.Add(verification);
        await _context.SaveChangesAsync();

        try 
        {
            await _emailService.SendVerificationCode(email, code);
            return Ok(new { message = "Code sent successfully!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error sending email: {ex.Message}");
        }
    }

    [HttpPost("verify-code")]
    public async Task<IActionResult> VerifyCode([FromBody] VerifyRequest request)
    {
        var record = await _context.UserVerifications
            .Where(v => v.Email == request.Email && v.Code == request.Code && !v.IsUsed)
            .OrderByDescending(v => v.Id)
            .FirstOrDefaultAsync();

        if (record == null || record.ExpiresAt < DateTime.UtcNow)
            return BadRequest(new { message = "Invalid or expired code." });

        record.IsUsed = true;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Email verified!" });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest(new { message = "This email is already registered." });

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok(new { message = "User created successfully!" });
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid email or password." });

        return Ok(new { token = GenerateJwt(user), message = "Login successful!" });
    }

    private string GenerateJwt(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim("id", user.Id.ToString()),
                new Claim("name", user.Name),
                new Claim("email", user.Email)
            ]),
            Expires = DateTime.UtcNow.AddHours(3),
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }
}

// DTOs
public record VerifyRequest(string Email, string Code);

public class RegisterDto {
    public required string Name { get; set; }
    [EmailAddress] public required string Email { get; set; }
    [MinLength(3)] public required string Password { get; set; }
}

public class LoginDto {
    [EmailAddress] public required string Email { get; set; }
    public required string Password { get; set; }
}