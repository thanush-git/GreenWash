using GreenWash.Data;
using GreenWash.Models;
using GreenWash.Services;
using GreenWash.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GreenWash.BackgroundServices
{
    /// <summary>
    /// Runs every 10 minutes, finds orders scheduled within the next 2 hours
    /// that haven't been reminded yet, and sends the customer a reminder email.
    /// </summary>
    public class ScheduledWashReminderService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ScheduledWashReminderService> _logger;

        // How often the background loop ticks
        private static readonly TimeSpan TickInterval = TimeSpan.FromMinutes(10);

        // The reminder window — send when wash is between 2h 10m and 2h 0m away
        // (the 10 min buffer prevents double-sending across ticks)
        private static readonly TimeSpan ReminderWindowMax = TimeSpan.FromMinutes(130);
        private static readonly TimeSpan ReminderWindowMin = TimeSpan.FromMinutes(120);

        public ScheduledWashReminderService(
            IServiceScopeFactory scopeFactory,
            ILogger<ScheduledWashReminderService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger       = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ScheduledWashReminderService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SendRemindersAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in ScheduledWashReminderService tick");
                }

                await Task.Delay(TickInterval, stoppingToken);
            }
        }

        private async Task SendRemindersAsync(CancellationToken ct)
        {
            using var scope   = _scopeFactory.CreateScope();
            var db            = scope.ServiceProvider.GetRequiredService<GreenWashDbContext>();
            var emailService  = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var now           = DateTime.UtcNow;
            var windowStart   = now.Add(ReminderWindowMin);
            var windowEnd     = now.Add(ReminderWindowMax);

            // Orders that are scheduled in the 2-hour window and still pending/accepted
            var upcomingOrders = await db.Orders
                .Where(o => o.ScheduledAt.HasValue
                         && o.ScheduledAt >= windowStart
                         && o.ScheduledAt <= windowEnd
                         && (o.Status == OrderStatus.Pending || o.Status == OrderStatus.Accepted))
                .ToListAsync(ct);

            foreach (var order in upcomingOrders)
            {
                var profile = await db.CustomerProfiles
                    .FirstOrDefaultAsync(c => c.CustomerId == order.CustomerId, ct);
                if (profile == null) continue;

                var user = await db.Users.FindAsync(new object[] { profile.UserId }, ct);
                if (user == null || string.IsNullOrEmpty(user.Email)) continue;

                var (subject, html) = EmailTemplates.ScheduledWashReminder(
                    profile.FirstName, order.OrderId,
                    order.ScheduledAt!.Value, order.ServiceAddress);

                await emailService.SendAsync(user.Email, profile.FirstName, subject, html);

                _logger.LogInformation(
                    "Reminder sent for Order #{OrderId} to {Email}", order.OrderId, user.Email);
            }
        }
    }
}
