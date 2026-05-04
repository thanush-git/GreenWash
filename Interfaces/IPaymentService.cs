using GreenWash.DTO;
using GreenWash.Models;

namespace GreenWash.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentMethod> AddPaymentMethodAsync(long userId, AddPaymentMethod dto);
        Task<List<PaymentMethod>> GetCustomerPaymentMethodsAsync(long userId);
        Task<string> ProcessPaymentAsync(ProcessPayment request, long userId);
    }
}
