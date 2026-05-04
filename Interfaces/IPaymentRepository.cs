using GreenWash.Models;

namespace GreenWash.Interfaces
{
    public interface IPaymentRepository
    {
        // Payment methods
        Task<PaymentMethod> AddPaymentMethodAsync(PaymentMethod payment);
        Task<List<PaymentMethod>> GetPaymentMethodsByCustomerIdAsync(long customerId);
        Task<PaymentMethod?> GetPaymentMethodByIdAsync(long paymentMethodId);
        Task ClearDefaultPaymentMethodsAsync(long customerId);

        // Payments / orders
        Task<PaymentMethod?> GetPaymentMethodAsync(long paymentMethodId, long customerId);
        Task<PromoCode?> GetActivePromoCodeAsync(string code);
        Task SaveChangesAsync();
    }
}
