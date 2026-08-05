using System.Net;
using System.Net.Mail;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");
        var host = emailSettings["SmtpHost"] ?? "smtp.gmail.com";
        var portStr = emailSettings["SmtpPort"] ?? "587";
        var user = emailSettings["SmtpUser"];
        var pass = emailSettings["SmtpPass"];

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            // Skip sending if no credentials are provided (useful for local dev without errors)
            Console.WriteLine($"[EmailService] Simulation - To: {toEmail}, Subject: {subject}");
            return;
        }

        int port = int.Parse(portStr);

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(user, pass),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(user, "SkillVault Notifications"),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        mailMessage.To.Add(toEmail);

        await client.SendMailAsync(mailMessage);
    }
}
