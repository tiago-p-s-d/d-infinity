using System.ComponentModel.DataAnnotations;
using Api.Data;
using Api.Models.User;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    AppDbContext context,
    TokenService tokenService,
    VerificationService verificationService,
    EmailService emailService) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly TokenService _tokenService = tokenService;
    private readonly VerificationService _verificationService = verificationService;
    private readonly EmailService _emailService = emailService;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest(new { message = "This email is already registered." });
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Created(string.Empty, new { message = "User created successfully!" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        return Ok(new { token = _tokenService.GenerateJwt(user), message = "Login successful!" });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        // Security: Return generic message to avoid email enumeration
        if (user is null)
        {
            return Ok(new { message = "If the email is registered, a recovery code has been sent." });
        }

        var code = await _verificationService.GenerateAndSaveCodeAsync(request.Email, expirationMinutes: 10);

        try
        {
            await _emailService.SendVerificationCode(request.Email, code);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while sending the email." });
        }

        return Ok(new { message = "If the email is registered, a recovery code has been sent." });
    }

    [HttpPost("verify-reset-code")]
    public async Task<IActionResult> VerifyResetCode([FromBody] VerifyCodeDto request)
    {
        var isValid = await _verificationService.ValidateCodeAsync(request.Email, request.Code, markAsUsed: false);

        if (!isValid)
        {
            return BadRequest(new { message = "Invalid or expired recovery code." });
        }

        return Ok(new { message = "Code verified successfully." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
    {
        var isValid = await _verificationService.ValidateCodeAsync(request.Email, request.Code, markAsUsed: true);

        if (!isValid)
        {
            return BadRequest(new { message = "Invalid or expired recovery code." });
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user is null)
        {
            return NotFound(new { message = "User not found." });
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Password updated successfully!" });
    }
    [HttpPost("send-code")]
public async Task<IActionResult> SendCode([FromBody] SendCodeDto request)
{
    var code = await _verificationService.GenerateAndSaveCodeAsync(request.Email, expirationMinutes: 10);

    try 
    {
        await _emailService.SendVerificationCode(request.Email, code);
        return Ok(new { message = "Code sent successfully!" });
    }
    catch (Exception)
    {
        return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error sending email." });
    }
}
}


#region DTOs

public record ForgotPasswordDto([Required, EmailAddress] string Email);

public record VerifyCodeDto([Required, EmailAddress] string Email, [Required] string Code);

public record ResetPasswordDto(
    [Required, EmailAddress] string Email,
    [Required] string Code,
    [Required, MinLength(6)] string NewPassword
);

public class RegisterDto
{
    [Required] public required string Name { get; set; }
    [Required, EmailAddress] public required string Email { get; set; }
    [Required, MinLength(6)] public required string Password { get; set; }
}

public class LoginDto
{
    [Required, EmailAddress] public required string Email { get; set; }
    [Required] public required string Password { get; set; }
}
public record SendCodeDto([Required, EmailAddress] string Email);

#endregion