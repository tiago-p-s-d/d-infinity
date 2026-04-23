using System.Net;
using System.Net.Mail;

namespace Api.Infrastructure;

public class EmailService(IConfiguration config)
{
    private readonly IConfiguration _config = config;

    public async Task SendVerificationCode(string email, string code)
    {
        var host = _config["Email:Host"] ?? string.Empty;
        var portStr = _config["Email:Port"] ?? "587"; 
        var user = _config["Email:User"] ?? string.Empty;
        var pass = _config["Email:Pass"] ?? string.Empty;

        using var smtpClient = new SmtpClient(host)
        {
            Port = int.Parse(portStr),
            Credentials = new NetworkCredential(user, pass),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(user.Length > 0 ? user : "qalhowow@gmail.com"),
            Subject = "D-Infinity: Verification code",
            Body = $@"
                <div style='font-family: sans-serif; background-color: #121212; color: #ffffff; padding: 20px; border-radius: 10px;'>
                    <h2 style='color: #007bff;'>D-Infinity Forge</h2>
                    <p>Your verification code is:</p>
                    <h1 style='background: #1e1e1e; padding: 10px; text-align: center; letter-spacing: 5px; border: 1px solid #333;'>{code}</h1>
                    <p style='color: #888; font-size: 0.8rem;'>this code expires in 10 minutes.</p>
                </div>",
            IsBodyHtml = true,
        };
        
        mailMessage.To.Add(email);
        await smtpClient.SendMailAsync(mailMessage);
    }
}