using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.DTO;
using GreenWash.Models;

namespace GreenWash.Interfaces
{
    public interface IAddPaymentMethodService
    {
        Task<PaymentMethod> AddPaymentMethodAsync(long customerId, AddPaymentMethod dto);

        Task<List<PaymentMethod>> GetCustomerPaymentMethodsAsync(long customerId);
    }
}