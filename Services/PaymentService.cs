using GreenWash.DTO;
using GreenWash.Exceptions;
using GreenWash.Interfaces;
using GreenWash.Models;

namespace GreenWash.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repository;
        private readonly ICustomerRepository _customer;
        private readonly IOrderRepository _orderRepository;
        private readonly IEmailService _email;

        public PaymentService(
            IPaymentRepository repository,
            ICustomerRepository customer,
            IOrderRepository orderRepository,
            IEmailService email)
        {
            _repository = repository;
            _customer = customer;
            _orderRepository = orderRepository;
            _email = email;
        }

        // PAYMENT METHODS

        public async Task<PaymentMethod> AddPaymentMethodAsync(long userId, AddPaymentMethod dto)
        {
            var profile = await _customer.GetByUserId(userId)
                ?? throw new NotFoundException("Customer profile not found");

            if (dto.CardNumber.Length < 16)
                throw new BadRequestException("Invalid card number");

            if (dto.IsDefault)
                await _repository.ClearDefaultPaymentMethodsAsync(profile.CustomerId);

            var payment = new PaymentMethod
            {
                CustomerId     = profile.CustomerId,
                CardHolderName = dto.CardHolderName,
                CardNumber     = dto.CardNumber,
                ExpiryMonth    = dto.ExpiryMonth,
                ExpiryYear     = dto.ExpiryYear,
                IsDefault      = dto.IsDefault,
                CreatedAt      = DateTime.UtcNow
            };

            return await _repository.AddPaymentMethodAsync(payment);
        }

        public async Task<List<PaymentMethod>> GetCustomerPaymentMethodsAsync(long userId)
        {
            var profile = await _customer.GetByUserId(userId)
                ?? throw new NotFoundException("Customer profile not found");

            return await _repository.GetPaymentMethodsByCustomerIdAsync(profile.CustomerId);
        }

        // PROCESS PAYMENT

        public async Task<string> ProcessPaymentAsync(ProcessPayment request, long userId)
        {
            var profile = await _customer.GetByUserId(userId)
                ?? throw new NotFoundException("Customer profile not found");

            var order = await _orderRepository.GetOrderForCustomerAsync(request.OrderId, profile.CustomerId)
                ?? throw new NotFoundException("Order not found");

            if (order.IsPaid)
                throw new BadRequestException("Order already paid");

            var paymentMethod = await _repository.GetPaymentMethodAsync(
                request.PaymentMethodId, profile.CustomerId)
                ?? throw new BadRequestException("Invalid payment method");

            if (!string.IsNullOrWhiteSpace(request.PromoCode) && order.PromoCodeId == null)
            {
                var promo = await _repository.GetActivePromoCodeAsync(request.PromoCode);

                if (promo == null || promo.ExpiryDate < DateTime.Now ||
                    promo.UsageCount >= promo.UsageLimit)
                    throw new BadRequestException(
                        "Promo code is invalid or expired. Please use a valid code or leave it blank.");

                if (promo.DiscountType.Equals("Percentage", StringComparison.OrdinalIgnoreCase))
                    order.TotalAmount = Math.Round(order.TotalAmount * (1 - promo.DiscountValue / 100), 2);
                else
                    order.TotalAmount = Math.Max(0, Math.Round(order.TotalAmount - promo.DiscountValue, 2));

                order.PromoCodeId = promo.PromoCodeId;
                promo.UsageCount++;
            }

            await Task.Delay(200); // simulated processing

            order.IsPaid = true;
            await _repository.SaveChangesAsync();

            var user = await _customer.GetUserById(profile.UserId);
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                var (subject, html) = EmailTemplates.PaymentConfirmed(
                    profile.FirstName, order.OrderId, order.TotalAmount);
                await _email.SendAsync(user.Email, profile.FirstName, subject, html);
            }

            return "Payment successful";
        }
    }
}
