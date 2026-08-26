using System.Security.Cryptography;
using Api.Data;
using Api.Models.User;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class VerificationService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<string> GenerateAndSaveCodeAsync(string email, int expirationMinutes = 15)
    {
        var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

        var verification = new UserVerification
        {
            Email = email,
            Code = code,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
            IsUsed = false
        };

        _context.UserVerifications.Add(verification);
        await _context.SaveChangesAsync();

        return code;
    }

    public async Task<bool> ValidateCodeAsync(string email, string code, bool markAsUsed = false)
    {
        var record = await _context.UserVerifications
            .Where(v => v.Email == email && v.Code == code && !v.IsUsed)
            .OrderByDescending(v => v.Id)
            .FirstOrDefaultAsync();

        if (record is null || record.ExpiresAt < DateTime.UtcNow)
        {
            return false;
        }

        if (markAsUsed)
        {
            record.IsUsed = true;
            await _context.SaveChangesAsync();
        }

        return true;
    }
}