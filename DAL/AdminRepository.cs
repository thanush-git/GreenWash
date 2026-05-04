using GreenWash.Data;
using GreenWash.Interfaces;
using GreenWash.Models;
using Microsoft.EntityFrameworkCore;

namespace GreenWash.DAL
{
    public class AdminRepository : IAdminRepository
    {
        private readonly GreenWashDbContext _context;

        public AdminRepository(GreenWashDbContext context)
        {
            _context = context;
        }

        // SERVICE PLANS

        public async Task<ServicePlan> AddServicePlanAsync(ServicePlan plan)
        {
            _context.ServicePlans.Add(plan);
            await _context.SaveChangesAsync();
            return plan;
        }

        public async Task<ServicePlan?> GetServicePlanByIdAsync(long id)
            => await _context.ServicePlans.FindAsync(id);

        public async Task<List<ServicePlan>> GetAllServicePlansAsync()
            => await _context.ServicePlans.ToListAsync();

        // ADD-ONS

        public async Task<AddOn> AddAddOnAsync(AddOn addOn)
        {
            _context.AddOns.Add(addOn);
            await _context.SaveChangesAsync();
            return addOn;
        }

        public async Task<AddOn?> GetAddOnByIdAsync(long id)
            => await _context.AddOns.FindAsync(id);

        public async Task<List<AddOn>> GetAllAddOnsAsync()
            => await _context.AddOns.ToListAsync();

        // PROMO CODES

        public async Task<PromoCode> AddPromoCodeAsync(PromoCode promo)
        {
            _context.PromoCodes.Add(promo);
            await _context.SaveChangesAsync();
            return promo;
        }

        public async Task<PromoCode?> GetPromoCodeByIdAsync(long id)
            => await _context.PromoCodes.FindAsync(id);

        public async Task<List<PromoCode>> GetAllPromoCodesAsync()
            => await _context.PromoCodes.ToListAsync();

        // CUSTOMERS

        public async Task<List<CustomerProfile>> GetAllCustomersAsync()
            => await _context.CustomerProfiles.ToListAsync();

        public async Task<List<Rating>> GetRatingsByRevieweeIdAsync(long revieweeId)
            => await _context.Ratings.Where(r => r.RevieweeId == revieweeId).ToListAsync();

        // WASHERS

        public async Task<List<WasherProfile>> GetAllWasherProfilesAsync()
            => await _context.WasherProfiles.ToListAsync();

        public async Task<WasherProfile?> GetWasherProfileByIdAsync(long washerId)
            => await _context.WasherProfiles.FirstOrDefaultAsync(w => w.WasherId == washerId);

        public async Task AddWasherAsync(User user, WasherProfile washer)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                washer.WasherId = user.UserId;
                _context.WasherProfiles.Add(washer);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateWasherProfileAsync(WasherProfile washer)
        {
            _context.WasherProfiles.Update(washer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserStatusAsync(long userId, bool isActive)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new Exception("User not found");
            user.IsActive = isActive;
            await _context.SaveChangesAsync();
        }

        // ORDERS

        public async Task<List<Order>> GetAllOrdersAsync(
            string? status, long? washerId, long? customerId,
            DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Orders.Include(o => o.AddOns).AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
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

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
