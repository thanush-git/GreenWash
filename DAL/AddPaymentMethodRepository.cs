using Microsoft.EntityFrameworkCore;
using GreenWash.Data;
using GreenWash.Models;
using GreenWash.Interfaces;

namespace GreenWash.DAL
{
    public class AddPaymentMethodRepository : IAddPaymentMethodRepository
    {
        private readonly GreenWashDbContext _context;

        public AddPaymentMethodRepository(GreenWashDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentMethod> AddAsync(PaymentMethod payment)
        {
            _context.PaymentMethods.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<List<PaymentMethod>> GetByCustomerIdAsync(long customerId)
        {
            return await _context.PaymentMethods
                .Where(p => p.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<PaymentMethod?> GetByIdAsync(int id)
        {
            return await _context.PaymentMethods
                .FirstOrDefaultAsync(p => p.PaymentMethodId == id);
        }
    }
}