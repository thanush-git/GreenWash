using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.Models;

namespace GreenWash.Interfaces
{
    public interface IAddPaymentMethodRepository
    {
        Task<PaymentMethod> AddAsync(PaymentMethod payment);
        Task<List<PaymentMethod>> GetByCustomerIdAsync(long customerId);
        Task<PaymentMethod?> GetByIdAsync(int id);
    }
}