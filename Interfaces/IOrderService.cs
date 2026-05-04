using GreenWash.DTO;
using GreenWash.Models;

namespace GreenWash.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateWashNowAsync(long userId, WashNowRequest request);
        Task<Order> ScheduleWashAsync(long userId, ScheduleWashRequest request);
        Task<List<Order>> GetCurrentOrdersAsync(long userId);
        Task<List<Order>> GetPastOrdersAsync(long userId);
    }
}
