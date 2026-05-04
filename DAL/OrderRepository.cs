using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.Data;
using GreenWash.Interfaces;
using GreenWash.Models;
using Microsoft.EntityFrameworkCore;

namespace GreenWash.DAL
{
    public class OrderRepository : IOrderRepository
    {
        private readonly GreenWashDbContext _context;

        public OrderRepository(GreenWashDbContext context)
        {
            _context = context;
        }

        public async Task<Order> AddAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> GetByIdAsync(long orderId)
        {
            return await _context.Orders
                .Include(o => o.AddOns)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<Order?> GetOrderForCustomerAsync(long orderId, long customerId)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.CustomerId == customerId);
        }

        public async Task<IEnumerable<Order>> GetByCustomerIdAsync(long customerId)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.AddOns)
                .ToListAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Order>> GetCurrentOrdersAsync(long customerId)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == customerId &&
                       (o.Status == OrderStatus.Pending ||
                        o.Status == OrderStatus.Accepted ||
                        o.Status == OrderStatus.InProgress))
                .Include(o => o.AddOns) 
                .ToListAsync();
        }

        public async Task<List<Order>> GetPastOrdersAsync(long customerId)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == customerId &&
                       (o.Status == OrderStatus.Completed ||
                        o.Status == OrderStatus.Cancelled))
                .Include(o => o.AddOns) 
                .ToListAsync();
        }

        // Helpers
        public async Task<ServicePlan?> GetActiveServicePlanAsync(long servicePlanId)
        {
            return await _context.ServicePlans
                .FirstOrDefaultAsync(s => s.ServicePlanId == servicePlanId && s.IsActive);
        }

        public async Task<List<AddOn>> GetAddOnsByIdsAsync(List<long> addOnIds)
        {
            return await _context.AddOns
                .Where(a => addOnIds.Contains(a.AddOnId))
                .ToListAsync();
        }

        //Order total and addon-builders helpers
        public decimal CalculateTotal(ServicePlan plan, List<AddOn> addOns)
        {
            return plan.BasePrice + addOns.Sum(a => a.Price);
        }

        public IEnumerable<OrderAddOn> BuildOrderAddOns(List<AddOn> addOns)
        {
            return addOns.Select(a => new OrderAddOn
            {
                AddOnId = a.AddOnId,
                PriceSnapshot = a.Price
            });
        }

        //Washers
        public async Task<List<Order>> GetAvailableOrdersAsync()
        {
            return await _context.Orders
                .Where(o => o.Status == OrderStatus.Pending && o.WasherId == null)
                .Include(o => o.AddOns)
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersForWasherAsync(long washerId)
        {
            return await _context.Orders
                .Where(o => o.WasherId == washerId)
                .Include(o => o.AddOns)
                .ToListAsync();
        }
    }
}