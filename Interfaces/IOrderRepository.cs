using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.Models;

namespace GreenWash.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> AddAsync(Order order);

        Task<Order?> GetByIdAsync(long orderId);

        Task<IEnumerable<Order>> GetByCustomerIdAsync(long customerId);
        Task UpdateAsync(Order order);
        Task<List<Order>> GetCurrentOrdersAsync(long customerId);
        Task<List<Order>> GetPastOrdersAsync(long customerId);

        //For Washer
        Task<List<Order>> GetAvailableOrdersAsync();
        Task<List<Order>> GetOrdersForWasherAsync(long washerId);
    }
}