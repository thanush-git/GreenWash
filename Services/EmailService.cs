using System.Net;
using System.Net.Mail;
using GreenWash.Interfaces;

namespace GreenWash.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            var host     = _config["Smtp:Host"]!;
            var port     = int.Parse(_config["Smtp:Port"]!);
            var username = _config["Smtp:Username"]!;
            var password = _config["Smtp:Password"]!;
            var fromName = _config["Smtp:FromName"] ?? "GreenWash";

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            using var message = new MailMessage
            {
                From       = new MailAddress(username, fromName),
                Subject    = subject,
                Body       = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(toEmail, toName));

            try
            {
                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent to {Email} — {Subject}", toEmail, subject);
            }
            catch (Exception ex)
            {
                // Never let a failed email crash the business operation
                _logger.LogError(ex, "Failed to send email to {Email} — {Subject}", toEmail, subject);
            }
        }
    }
}
