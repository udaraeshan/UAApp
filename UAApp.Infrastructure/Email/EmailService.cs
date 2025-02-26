using System;
using System.Net;
using System.Net.Mail;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UAApp.Domain.Common;
using UAApp.Shared.Log;

public class EmailService
{
    private readonly string _host;
    private readonly string _from;
    private readonly string _alias;
    private readonly string _password;
    private readonly int _port;
    private readonly bool _emailNotifications;
    private readonly IApplicationLogger _logger;
    private readonly ICurrentUserService _currentUserService;

    public EmailService(IConfiguration iConfiguration, IApplicationLogger logger, ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;

        var smtpSection = iConfiguration.GetSection("SMTP");
        _host = smtpSection["Host"] ?? string.Empty;
        _from = smtpSection["From"] ?? string.Empty;
        _alias = smtpSection["Alias"] ?? string.Empty;
        _password = smtpSection["Password"] ?? string.Empty;
        _emailNotifications = bool.TryParse(smtpSection["EmailNotifications"], out bool emailNotifications) && emailNotifications;
        _port = int.TryParse(smtpSection["Port"], out int port) ? port : 587;
    }

    public async Task SendEmail(string to, string subject, string body)
    {
        if (!_emailNotifications)
        {
            return;
        }

        try
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_from, _alias),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(to));

            using var client = new SmtpClient(_host)
            {
                Port = _port,
                Credentials = new NetworkCredential(_from, _password),
                EnableSsl = true
            };
            await client.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, new LogFormat
            {
                Message = $"Email sending failed: {ex.Message}",
                UserID = _currentUserService.UserId,
                Description = ex.InnerException?.Message ?? string.Empty,
                Method = "SendEmail",
            });
        }
    }
}
