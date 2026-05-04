using GreenWash.Models;

namespace GreenWash.Interfaces
{
    public interface IAdminRepository
    {
        // Service Plans
        Task<ServicePlan> AddServicePlanAsync(ServicePlan plan);
        Task<ServicePlan?> GetServicePlanByIdAsync(long id);
        Task<List<ServicePlan>> GetAllServicePlansAsync();
        Task SaveChangesAsync();

        // Add-Ons
        Task<AddOn> AddAddOnAsync(AddOn addOn);
        Task<AddOn?> GetAddOnByIdAsync(long id);
        Task<List<AddOn>> GetAllAddOnsAsync();

        // Promo Codes
        Task<PromoCode> AddPromoCodeAsync(PromoCode promo);
        Task<PromoCode?> GetPromoCodeByIdAsync(long id);
        Task<List<PromoCode>> GetAllPromoCodesAsync();

        // Customers
        Task<List<CustomerProfile>> GetAllCustomersAsync();
        Task<List<Rating>> GetRatingsByRevieweeIdAsync(long revieweeId);

        // Washers
        Task<List<WasherProfile>> GetAllWasherProfilesAsync();
        Task<WasherProfile?> GetWasherProfileByIdAsync(long washerId);
        Task AddWasherAsync(User user, WasherProfile washer);
        Task UpdateWasherProfileAsync(WasherProfile washer);
        Task UpdateUserStatusAsync(long userId, bool isActive);

        // Orders
        Task<List<Order>> GetAllOrdersAsync(string? status, long? washerId, long? customerId, DateTime? startDate, DateTime? endDate);
    }
}
