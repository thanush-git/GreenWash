using GreenWash.Data;
using GreenWash.DTO;
using GreenWash.Exceptions;
using GreenWash.Interfaces;
using GreenWash.Models;
using Microsoft.EntityFrameworkCore;

namespace GreenWash.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly GreenWashDbContext _context;
        private readonly IEmailService _email;

        public PaymentService(GreenWashDbContext context, IEmailService email)
        {
            _context = context;
            _email = email;
        }

        public async Task<string> ProcessPaymentAsync(ProcessPayment request, long userId)
        {
            // Resolve userId → customer profile
            var profile = await _context.CustomerProfiles
                .FirstOrDefaultAsync(c => c.UserId == userId)
                ?? throw new NotFoundException("Customer profile not found");

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == request.OrderId && o.CustomerId == profile.CustomerId)
                ?? throw new NotFoundException("Order not found");

            if (order.IsPaid)
                throw new BadRequestException("Order already paid");

            var paymentMethod = await _context.PaymentMethods
                .FirstOrDefaultAsync(p => p.PaymentMethodId == request.PaymentMethodId
                                       && p.CustomerId == profile.CustomerId)
                ?? throw new BadRequestException("Invalid payment method");

            // ── Optional promo code applied at payment time ────────────────────
            if (!string.IsNullOrWhiteSpace(request.PromoCode))
            {
                // Already has a promo applied from a previous attempt — skip re-applying
                if (order.PromoCodeId == null)
                {
                    var promo = await _context.PromoCodes
                        .FirstOrDefaultAsync(p => p.Code == request.PromoCode && p.IsActive);

                    if (promo == null || promo.ExpiryDate < DateTime.UtcNow || promo.UsageCount >= promo.UsageLimit)
                        throw new BadRequestException(
                            "Promo code is invalid or expired. Please use a valid code or leave it blank to pay the full amount.");

                    // Apply discount
                    if (promo.DiscountType.Equals("Percentage", StringComparison.OrdinalIgnoreCase))
                        order.TotalAmount = Math.Round(order.TotalAmount * (1 - promo.DiscountValue / 100), 2);
                    else
                        order.TotalAmount = Math.Max(0, Math.Round(order.TotalAmount - promo.DiscountValue, 2));

                    order.PromoCodeId = promo.PromoCodeId;
                    promo.UsageCount++;
                }
            }

            // ── Process payment ────────────────────────────────────────────────
            await Task.Delay(200); // simulate gateway

            order.IsPaid = true;
            await _context.SaveChangesAsync();

            // ── Confirmation email ─────────────────────────────────────────────
            var user = await _context.Users.FindAsync(profile.UserId);
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
