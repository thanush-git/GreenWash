using GreenWash.DTO;
using GreenWash.Models;

namespace GreenWash.Interfaces
{
    public interface IAdminService
    {
        // Service Plans
        Task<ServicePlan> CreateServicePlanAsync(CreateServicePlanRequest request);
        Task<ServicePlan> UpdateServicePlanAsync(long id, CreateServicePlanRequest request);
        Task UpdateServicePlanStatusAsync(long id, bool isActive);
        Task<List<ServicePlan>> GetAllServicePlansAsync();

        // Add-Ons
        Task<AddOn> CreateAddOnAsync(CreateAddOnRequest request);
        Task<AddOn> UpdateAddOnAsync(long id, CreateAddOnRequest request);
        Task UpdateAddOnStatusAsync(long id, bool isActive);
        Task<List<AddOn>> GetAllAddOnsAsync();

        // Promo Codes
        Task<PromoCode> CreatePromoCodeAsync(CreatePromoCodeRequest request);
        Task<PromoCode> UpdatePromoCodeAsync(long id, CreatePromoCodeRequest request);
        Task<List<PromoCode>> GetAllPromoCodesAsync();

        // Customers
        Task<List<CustomerProfile>> GetAllCustomersAsync();
        Task UpdateCustomerStatusAsync(long customerId, bool isActive);
        Task<List<Rating>> GetCustomerRatingsAsync(long customerId);

        // Washers
        Task<WasherProfile> AddWasherAsync(CreateWasherRequest dto);
        Task<WasherProfile> UpdateWasherAsync(long washerId, UpdateWasherRequest dto);
        Task ToggleWasherStatusAsync(long washerId, bool isActive);
        Task<List<WasherProfile>> GetAllWashersAsync();
        Task<List<Rating>> GetWasherRatingsAsync(long washerId);

        // Orders
        Task<List<Order>> GetAllOrdersAsync(string? status, long? washerId, long? customerId, DateTime? startDate, DateTime? endDate);
        Task<Order> AssignWasherToOrderAsync(long orderId, long washerId);
        Task CancelOrderAsync(long orderId);

        // Leaderboard
        Task<List<object>> GetLeaderboardAsync();
    }
}
