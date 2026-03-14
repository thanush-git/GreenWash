using GreenWash.Data;
using GreenWash.DTO;
using GreenWash.Exceptions;
using GreenWash.Interfaces;
using GreenWash.Models;
using Microsoft.EntityFrameworkCore;

namespace GreenWash.Services
{
    public class AddPaymentMethodService : IAddPaymentMethodService
    {
        private readonly IAddPaymentMethodRepository _repository;
        private readonly GreenWashDbContext _context;

        public AddPaymentMethodService(IAddPaymentMethodRepository repository, GreenWashDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        // Resolves JWT userId → CustomerProfile.CustomerId (the FK stored on PaymentMethod)
        private async Task<long> ResolveCustomerIdAsync(long userId)
        {
            var profile = await _context.CustomerProfiles
                .FirstOrDefaultAsync(c => c.UserId == userId)
                ?? throw new NotFoundException("Customer profile not found");
            return profile.CustomerId;
        }

        public async Task<PaymentMethod> AddPaymentMethodAsync(long userId, AddPaymentMethod dto)
        {
            var customerId = await ResolveCustomerIdAsync(userId);

            // Validate card number (basic: 13-19 digits)
            var digits = new string(dto.CardNumber.Where(char.IsDigit).ToArray());
            if (digits.Length < 13 || digits.Length > 19)
                throw new BadRequestException("Invalid card number");

            var payment = new PaymentMethod
            {
                CustomerId     = customerId,
                CardHolderName = dto.CardHolderName,
                CardNumber     = $"**** **** **** {digits[^4..]}",  // store only last 4 digits
                ExpiryMonth    = dto.ExpiryMonth,
                ExpiryYear     = dto.ExpiryYear,
                IsDefault      = dto.IsDefault,
                CreatedAt      = DateTime.UtcNow
            };

            // If this card is being set as default, clear existing defaults first
            if (dto.IsDefault)
            {
                var existing = await _context.PaymentMethods
                    .Where(p => p.CustomerId == customerId && p.IsDefault)
                    .ToListAsync();
                foreach (var card in existing)
                    card.IsDefault = false;
                await _context.SaveChangesAsync();
            }

            return await _repository.AddAsync(payment);
        }

        public async Task<List<PaymentMethod>> GetCustomerPaymentMethodsAsync(long userId)
        {
            var customerId = await ResolveCustomerIdAsync(userId);
            return await _repository.GetByCustomerIdAsync(customerId);
        }
    }
}
