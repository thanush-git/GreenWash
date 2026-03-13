using Microsoft.EntityFrameworkCore;
using GreenWash.Models;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Order>()
                .HasMany(o => o.AddOns)
                .WithOne(a => a.Order)
                .HasForeignKey(a => a.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}