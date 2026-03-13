using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.DAL;
using GreenWash.DTO;
using GreenWash.Interfaces;
using Microsoft.EntityFrameworkCore;
using GreenWash.Data;
using GreenWash.Models;
using GreenWash.Interfaces;

namespace GreenWash.Services
{
    public class WasherService : IWasherService
    {
        private readonly IOrderRepository _orderRepository;

        public WasherService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task AcceptOrderAsync(int orderId, int washerId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
                throw new Exception("Order not found");

            if (order.Status != OrderStatus.Pending)
                throw new Exception("Order cannot be accepted");

            order.WasherId = washerId;
            order.Status = OrderStatus.Accepted;

            await _orderRepository.UpdateAsync(order);
        }

        public async Task DeclineOrderAsync(int orderId, int washerId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
                throw new Exception("Order not found");

            if (order.WasherId != washerId)
                throw new Exception("Unauthorized washer");

            if (order.Status != OrderStatus.Pending)
                throw new Exception("Order cannot be declined");

            order.Status = OrderStatus.Declined;

            await _orderRepository.UpdateAsync(order);
        }

        public async Task StartWashAsync(int orderId, int washerId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
                throw new Exception("Order not found");

            if (order.WasherId != washerId)
                throw new Exception("Unauthorized washer");

            if (order.Status != OrderStatus.Accepted)
                throw new Exception("Order must be accepted first");

            order.Status = OrderStatus.InProgress;

            await _orderRepository.UpdateAsync(order);
        }

        public async Task<List<Order>> GetAvailableOrdersAsync()
        {
            return await _orderRepository.GetAvailableOrdersAsync();
        }

        public async Task CompleteWashAsync(int orderId, int washerId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
                throw new Exception("Order not found");

            if (order.WasherId != washerId)
                throw new Exception("Unauthorized washer");

            if (order.Status != OrderStatus.InProgress)
                throw new Exception("Wash has not started");

            order.Status = OrderStatus.Completed;

            await _orderRepository.UpdateAsync(order);
        }

        public async Task<List<Order>> GetWasherOrdersAsync(int washerId)
        {
            return await _orderRepository.GetOrdersForWasherAsync(washerId);
        }
    }
}