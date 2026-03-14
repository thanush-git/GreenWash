using GreenWash.Data;
using GreenWash.DTO;
using GreenWash.Exceptions;
using GreenWash.Interfaces;
using GreenWash.Models;
using Microsoft.EntityFrameworkCore;

namespace GreenWash.Services
{
    public class AdminService : IAdminService
    {
        private readonly GreenWashDbContext _context;
        private readonly IEmailService _email;

        public AdminService(GreenWashDbContext context, IEmailService email)
        {
            _context = context;
            _email = email;
        }

        // ── SERVICE PLANS ──────────────────────────────────────────────────────────

        public async Task<ServicePlan> CreateServicePlanAsync(CreateServicePlanRequest request)
        {
            var plan = new ServicePlan
            {
                Name = request.Name,
                Description = request.Description,
                BasePrice = request.BasePrice
            };
            _context.ServicePlans.Add(plan);
            await _context.SaveChangesAsync();
            return plan;
        }

        public async Task<ServicePlan> UpdateServicePlanAsync(long id, CreateServicePlanRequest request)
        {
            var plan = await _context.ServicePlans.FindAsync(id)
                ?? throw new NotFoundException("Service plan not found");

            plan.Name = request.Name;
            plan.Description = request.Description;
            plan.BasePrice = request.BasePrice;

            await _context.SaveChangesAsync();
            return plan;
        }

        public async Task UpdateServicePlanStatusAsync(long id, bool isActive)
        {
            var plan = await _context.ServicePlans.FindAsync(id)
                ?? throw new NotFoundException("Service plan not found");

            plan.IsActive = isActive;
            await _context.SaveChangesAsync();
        }

        public async Task<List<ServicePlan>> GetAllServicePlansAsync()
        {
            return await _context.ServicePlans.ToListAsync();
        }

        // ── ADD-ONS ────────────────────────────────────────────────────────────────

        public async Task<AddOn> CreateAddOnAsync(CreateAddOnRequest request)
        {
            var addOn = new AddOn
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price
            };
            _context.AddOns.Add(addOn);
            await _context.SaveChangesAsync();
            return addOn;
        }

        public async Task<AddOn> UpdateAddOnAsync(long id, CreateAddOnRequest request)
        {
            var addOn = await _context.AddOns.FindAsync(id)
                ?? throw new NotFoundException("Add-on not found");

            addOn.Name = request.Name;
            addOn.Description = request.Description;
            addOn.Price = request.Price;

            await _context.SaveChangesAsync();
            return addOn;
        }

        public async Task UpdateAddOnStatusAsync(long id, bool isActive)
        {
            var addOn = await _context.AddOns.FindAsync(id)
                ?? throw new NotFoundException("Add-on not found");

            addOn.IsActive = isActive;
            await _context.SaveChangesAsync();
        }

        public async Task<List<AddOn>> GetAllAddOnsAsync()
        {
            return await _context.AddOns.ToListAsync();
        }

        // ── PROMO CODES ────────────────────────────────────────────────────────────

        public async Task<PromoCode> CreatePromoCodeAsync(CreatePromoCodeRequest request)
        {
            var promo = new PromoCode
            {
                Code = request.Code,
                DiscountType = request.DiscountType,
                DiscountValue = request.DiscountValue,
                ExpiryDate = request.ExpiryDate,
                UsageLimit = request.UsageLimit
            };
            _context.PromoCodes.Add(promo);
            await _context.SaveChangesAsync();
            return promo;
        }

        public async Task<PromoCode> UpdatePromoCodeAsync(long id, CreatePromoCodeRequest request)
        {
            var promo = await _context.PromoCodes.FindAsync(id)
                ?? throw new NotFoundException("Promo code not found");

            promo.Code = request.Code;
            promo.DiscountType = request.DiscountType;
            promo.DiscountValue = request.DiscountValue;
            promo.ExpiryDate = request.ExpiryDate;
            promo.UsageLimit = request.UsageLimit;

            await _context.SaveChangesAsync();
            return promo;
        }

        public async Task<List<PromoCode>> GetAllPromoCodesAsync()
        {
            return await _context.PromoCodes.ToListAsync();
        }

        // ── CUSTOMERS ──────────────────────────────────────────────────────────────

        public async Task<List<CustomerProfile>> GetAllCustomersAsync()
        {
            return await _context.CustomerProfiles.ToListAsync();
        }

        public async Task UpdateCustomerStatusAsync(long customerId, bool isActive)
        {
            var profile = await _context.CustomerProfiles.FindAsync(customerId)
                ?? throw new NotFoundException("Customer not found");

            var user = await _context.Users.FindAsync(profile.UserId)
                ?? throw new NotFoundException("User not found");

            user.IsActive = isActive;
            await _context.SaveChangesAsync();
        }

        public async Task<List<Rating>> GetCustomerRatingsAsync(long customerId)
        {
            return await _context.Ratings
                .Where(r => r.RevieweeId == customerId)
                .ToListAsync();
        }

        // ── WASHERS ────────────────────────────────────────────────────────────────

        public async Task<List<WasherProfile>> GetAllWashersAsync()
        {
            return await _context.WasherProfiles.ToListAsync();
        }

        public async Task<List<Rating>> GetWasherRatingsAsync(long washerId)
        {
            return await _context.Ratings
                .Where(r => r.RevieweeId == washerId)
                .ToListAsync();
        }

        // ── ORDERS ─────────────────────────────────────────────────────────────────

        public async Task<List<Order>> GetAllOrdersAsync(
            string? status, long? washerId, long? customerId,
            DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Orders.Include(o => o.AddOns).AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
                query = query.Where(o => o.Status == parsedStatus);

            if (washerId.HasValue)
                query = query.Where(o => o.WasherId == washerId);

            if (customerId.HasValue)
                query = query.Where(o => o.CustomerId == customerId);

            if (startDate.HasValue)
                query = query.Where(o => o.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(o => o.CreatedAt <= endDate.Value);

            return await query.ToListAsync();
        }

        public async Task<Order> AssignWasherToOrderAsync(long orderId, long washerId)
        {
            var order = await _context.Orders.FindAsync(orderId)
                ?? throw new NotFoundException("Order not found");

            if (order.Status != OrderStatus.Pending)
                throw new BadRequestException("Only pending orders can be assigned");

            var washer = await _context.WasherProfiles.FindAsync(washerId)
                ?? throw new NotFoundException("Washer not found");

            order.WasherId = washerId;
            order.Status = OrderStatus.Accepted;

            await _context.SaveChangesAsync();

            // Email customer
            var profile = await _context.CustomerProfiles
                .FirstOrDefaultAsync(c => c.CustomerId == order.CustomerId);
            if (profile != null)
            {
                var user = await _context.Users.FindAsync(profile.UserId);
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    var washerName = $"{washer.FirstName} {washer.LastName}";
                    var (subject, html) = EmailTemplates.OrderAccepted(
                        profile.FirstName, order.OrderId, washerName, order.ServiceAddress);
                    await _email.SendAsync(user.Email, profile.FirstName, subject, html);
                }
            }

            return order;
        }

        public async Task CancelOrderAsync(long orderId)
        {
            var order = await _context.Orders.FindAsync(orderId)
                ?? throw new NotFoundException("Order not found");

            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();

            // Email customer
            var profile = await _context.CustomerProfiles
                .FirstOrDefaultAsync(c => c.CustomerId == order.CustomerId);
            if (profile != null)
            {
                var user = await _context.Users.FindAsync(profile.UserId);
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    var (subject, html) = EmailTemplates.OrderCancelled(profile.FirstName, order.OrderId);
                    await _email.SendAsync(user.Email, profile.FirstName, subject, html);
                }
            }
        }

        // ── LEADERBOARD ────────────────────────────────────────────────────────────

        private const double GallonsPerWash = 3.5;

        public async Task<List<object>> GetLeaderboardAsync()
        {
            var washers = await _context.WasherProfiles.ToListAsync();

            return washers
                .OrderByDescending(w => w.TotalWashes)
                .Select(w => (object)new
                {
                    w.WasherId,
                    w.FirstName,
                    w.LastName,
                    w.TotalWashes,
                    w.AverageRating,
                    GallonsSaved = Math.Round(w.TotalWashes * GallonsPerWash, 2)
                })
                .ToList();
        }
    }
}
