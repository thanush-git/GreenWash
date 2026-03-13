using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.DTO;
using GreenWash.Models;

namespace GreenWash.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateWashNowAsync(long customerId, WashNowRequest request);

        Task<Order> ScheduleWashAsync(long customerId, ScheduleWashRequest request);

        Task<Order?> GetOrderAsync(long orderId);

        Task CancelOrderAsync(long orderId);

        Task<List<Order>> GetCurrentOrdersAsync(long customerId);
        Task<List<Order>> GetPastOrdersAsync(long customerId);
    }
}