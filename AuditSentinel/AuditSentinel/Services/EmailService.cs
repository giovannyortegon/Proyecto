using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace AuditSentinel.Services;
public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string subject, string body)
    {
        var smtpSettings = _configuration.GetSection("SmtpSettings");

        var client = new SmtpClient(smtpSettings["Host"])
        {
            Port = int.Parse(smtpSettings["Port"]),
            Credentials = new NetworkCredential(
                smtpSettings["Email"],
                smtpSettings["Password"]
            ),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(smtpSettings["Email"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };

        mailMessage.To.Add(smtpSettings["Email"]);

        await client.SendMailAsync(mailMessage);
    }
}
