using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.DTO;
using GreenWash.Interfaces;
using GreenWash.Models;

namespace GreenWash.Services
{
    public class AddPaymentMethodService : IAddPaymentMethodService
    {
        private readonly IAddPaymentMethodRepository _repository;

        public AddPaymentMethodService(IAddPaymentMethodRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaymentMethod> AddPaymentMethodAsync(long customerId, AddPaymentMethod dto)
        {
            var payment = new PaymentMethod
            {
                CustomerId = customerId,
                CardHolderName = dto.CardHolderName,
                CardNumber = dto.CardNumber,
                ExpiryMonth = dto.ExpiryMonth,
                ExpiryYear = dto.ExpiryYear,
                IsDefault = dto.IsDefault
            };

            return await _repository.AddAsync(payment);
        }

        public async Task<List<PaymentMethod>> GetCustomerPaymentMethodsAsync(long customerId)
        {
            return await _repository.GetByCustomerIdAsync(customerId);
        }
    }
}