using GreenWash.Data;
using GreenWash.Interfaces;
using GreenWash.Models;
using Microsoft.EntityFrameworkCore;

namespace GreenWash.DAL
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly GreenWashDbContext _context;

        public PaymentRepository(GreenWashDbContext context)
        {
            _context = context;
        }

        //PAYMENT METHODS

        public async Task<PaymentMethod> AddPaymentMethodAsync(PaymentMethod payment)
        {
            _context.PaymentMethods.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<List<PaymentMethod>> GetPaymentMethodsByCustomerIdAsync(long customerId){
            return await _context.PaymentMethods.Where(p => p.CustomerId == customerId).ToListAsync();
        }
        public async Task<PaymentMethod?> GetPaymentMethodByIdAsync(long paymentMethodId){
            return await _context.PaymentMethods.FirstOrDefaultAsync(p => p.PaymentMethodId == paymentMethodId);
        }
        public async Task ClearDefaultPaymentMethodsAsync(long customerId)
        {
            var existing = await _context.PaymentMethods
                .Where(p => p.CustomerId == customerId && p.IsDefault)
                .ToListAsync();
            foreach (var card in existing)
                card.IsDefault = false;
            await _context.SaveChangesAsync();
        }

        // PAYMENTS / ORDERS

        public async Task<PaymentMethod?> GetPaymentMethodAsync(long paymentMethodId, long customerId){
            return await _context.PaymentMethods.FirstOrDefaultAsync(
                p => p.PaymentMethodId == paymentMethodId && p.CustomerId == customerId);
        }
        public async Task<PromoCode?> GetActivePromoCodeAsync(string code){
            return await _context.PromoCodes.FirstOrDefaultAsync(p => p.Code == code && p.IsActive);
        }
        public async Task SaveChangesAsync(){
            await _context.SaveChangesAsync();
        }
    }
}
