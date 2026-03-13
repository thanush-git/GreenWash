using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.DAL;
using GreenWash.DTO;
using GreenWash.Interfaces;
using Microsoft.EntityFrameworkCore;
using GreenWash.Data;

namespace GreenWash.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly GreenWashDbContext _context;

        public PaymentService(GreenWashDbContext context)
        {
            _context = context;
        }

        public async Task<string> ProcessPaymentAsync(ProcessPayment request, long userId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == request.OrderId && o.CustomerId == userId);

            if (order == null)
                throw new Exception("Order not found");

            if (order.IsPaid)
                throw new Exception("Order already paid");

            var paymentMethod = await _context.PaymentMethods
                .FirstOrDefaultAsync(p => p.PaymentMethodId == request.PaymentMethodId && p.CustomerId == userId);

            if (paymentMethod == null)
                throw new Exception("Invalid payment method");

            // Dummy payment simulation
            await Task.Delay(500);

            order.IsPaid = true;

            await _context.SaveChangesAsync();

            return "Payment Successful";
        }
    }
}