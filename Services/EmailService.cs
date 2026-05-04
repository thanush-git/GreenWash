using System.Net;
using System.Net.Mail;
using GreenWash.Interfaces;

namespace GreenWash.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ICustomerRepository _customer;
        private readonly ILogger<EmailService> _logger;

        public EmailService
        (IConfiguration config, ICustomerRepository customer, ILogger<EmailService> logger)
        {
            _config = config;
            _customer = customer;
            _logger = logger;
        }

        public async Task<(string email, string firstName)> GetCustomerContactAsync(long customerId)
        {
            var profile = await _customer.GetByCustomerId(customerId);
            if (profile == null) return ("", "Customer");

            var user = await _customer.GetUserById(profile.UserId);
            return (user?.Email ?? "", profile.FirstName);
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
                // catching to not let the application crash
                _logger.LogError(ex, "Failed to send email to {Email} — {Subject}", toEmail, subject);
            }
        }
    }
}
