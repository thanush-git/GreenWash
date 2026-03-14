using GreenWash.DTO;
using GreenWash.Models;

namespace GreenWash.Interfaces
{
    public interface IWasherService
    {
        Task HandleOrderActionAsync(long orderId, long washerId, string action);
        Task<List<Order>> GetAvailableOrdersAsync();
        Task<List<Order>> GetWasherOrdersAsync(long washerId);
        Task<WasherProfile> UpdateWasherAsync(long washerId, UpdateWasher dto);
    }
}
