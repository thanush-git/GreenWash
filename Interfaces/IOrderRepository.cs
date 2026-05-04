using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.Models;

namespace GreenWash.Interfaces
{
    public interface IOrderRepository
    {
        //Customer and Admin
        Task<Order> AddAsync(Order order);

        Task<Order?> GetByIdAsync(long orderId);
        Task<Order?> GetOrderForCustomerAsync(long orderId, long customerId);

        Task<IEnumerable<Order>> GetByCustomerIdAsync(long customerId);
        Task UpdateAsync(Order order);
        Task<List<Order>> GetCurrentOrdersAsync(long customerId);
        Task<List<Order>> GetPastOrdersAsync(long customerId);

        //Helpers for Validating the orders
        Task<ServicePlan?> GetActiveServicePlanAsync(long servicePlanId);
        Task<List<AddOn>> GetAddOnsByIdsAsync(List<long> addOnIds);

        //Order total and Addon Builders
        decimal CalculateTotal(ServicePlan plan, List<AddOn> addOns);
        IEnumerable<OrderAddOn> BuildOrderAddOns(List<AddOn> addOns);
        
        //For Washer
        Task<List<Order>> GetAvailableOrdersAsync();
        Task<List<Order>> GetOrdersForWasherAsync(long washerId);
    }
}