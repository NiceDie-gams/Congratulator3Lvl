using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using MailKit.Security;
using System.Security.Cryptography;

namespace NewCongratulator.Services;
public class EmailSettings
{
    public string SmtpHost { get; set; }
    public int SmtpPort { get; set; }
    public bool UseSsl { get; set; }
    public string SenderName { get; set; }
    public string FromEmail { get; set; }
    public string AppPassword { get; set; }
}

public class EmailSenderService
{
    private readonly EmailSettings _settings;

    public EmailSenderService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string bodyHtml)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.FromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = bodyHtml };

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.Auto);
        await client.AuthenticateAsync(_settings.FromEmail, _settings.AppPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
