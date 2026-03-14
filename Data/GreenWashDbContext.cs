using Microsoft.EntityFrameworkCore;
using GreenWash.Models;

namespace GreenWash.Data
{
    public class GreenWashDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public GreenWashDbContext(DbContextOptions<GreenWashDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<CustomerProfile> CustomerProfiles { get; set; }
        public DbSet<WasherProfile> WasherProfiles { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderAddOn> OrderAddOns { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<ServicePlan> ServicePlans { get; set; }
        public DbSet<AddOn> AddOns { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.AddOns)
                .WithOne(a => a.Order)
                .HasForeignKey(a => a.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── Seed default admin ─────────────────────────────────────────────────
            // Default credentials: admin@greenwash.com / Admin@1234
            // The hash below is a fixed BCrypt hash (cost 12) of "Admin@1234".
            // To use a different password, replace this hash with a new one generated
            // via: BCrypt.Net.BCrypt.HashPassword("YourNewPassword")
            modelBuilder.Entity<User>().HasData(new User
            {
                UserId = 1,
                Email = "admin@greenwash.com",
                PasswordHash = "$2b$12$N6q4Rrfrke4V5SgyO8HKk.xh9dnK7CLjW8jOdgfUXlFmJ2rdMqKme",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
        }
    }
}
