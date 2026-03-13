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
                .ToListAsync();
        }

        public async Task<List<Order>> GetPastOrdersAsync(long customerId)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == customerId &&
                       (o.Status == OrderStatus.Completed ||
                        o.Status == OrderStatus.Cancelled))
                .ToListAsync();
        }

        //Washers
        public async Task<List<Order>> GetAvailableOrdersAsync()
        {
            return await _context.Orders
                .Where(o => o.Status == OrderStatus.Pending && o.WasherId == null)
                .ToListAsync();
        }
        
        public async Task<List<Order>> GetOrdersForWasherAsync(int washerId)
        {
            return await _context.Orders
                .Where(o => o.WasherId == washerId)
                .ToListAsync();
        }
    }
}