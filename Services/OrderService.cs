using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.DTO;
using GreenWash.Interfaces;
using GreenWash.Models;

namespace GreenWash.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;

        public OrderService(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<Order> CreateWashNowAsync(long customerId, WashNowRequest request)
        {
            var order = new Order
            {
                CustomerId = customerId,
                CarId = request.CarId,
                ServicePlanId = request.ServicePlanId,
                ServiceAddress = request.ServiceAddress,
                CustomerNotes = request.Notes,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            if (request.AddOnIds != null)
            {
                foreach (var addOnId in request.AddOnIds)
                {
                    order.AddOns.Add(new OrderAddOn
                    {
                        AddOnId = addOnId,
                        PriceSnapshot = 0
                    });
                }
            }

            return await _repository.AddAsync(order);
        }

        public async Task<Order> ScheduleWashAsync(long customerId, ScheduleWashRequest request)
        {
            if (request.ScheduledAt <= DateTime.UtcNow)
                throw new Exception("Scheduled date must be in the future");

            var order = new Order
            {
                CustomerId = customerId,
                CarId = request.CarId,
                ServicePlanId = request.ServicePlanId,
                ServiceAddress = request.ServiceAddress,
                CustomerNotes = request.Notes,
                ScheduledAt = request.ScheduledAt,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            if (request.AddOnIds != null)
            {
                foreach (var addOnId in request.AddOnIds)
                {
                    order.AddOns.Add(new OrderAddOn
                    {
                        AddOnId = addOnId,
                        PriceSnapshot = 0
                    });
                }
            }

            return await _repository.AddAsync(order);
        }

        public async Task<Order?> GetOrderAsync(long orderId)
        {
            return await _repository.GetByIdAsync(orderId);
        }

        public async Task<List<Order>> GetCurrentOrdersAsync(long customerId)
        {
            return await _repository.GetCurrentOrdersAsync(customerId);
        }

        public async Task<List<Order>> GetPastOrdersAsync(long customerId)
        {
            return await _repository.GetPastOrdersAsync(customerId);
        }

        public async Task CancelOrderAsync(long orderId)
        {
            var order = await _repository.GetByIdAsync(orderId);

            if (order == null)
                throw new Exception("Order not found");

            order.Status = OrderStatus.Cancelled;

            await _repository.UpdateAsync(order);
        }
    }
}