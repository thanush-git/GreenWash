using GreenWash.Data;
using GreenWash.DTO;
using GreenWash.Exceptions;
using GreenWash.Interfaces;
using GreenWash.Models;
using Microsoft.EntityFrameworkCore;

namespace GreenWash.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IOrderRepository   _orderRepository;
        private readonly GreenWashDbContext  _context;
        private readonly IEmailService       _email;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            IOrderRepository orderRepository,
            GreenWashDbContext context,
            IEmailService email)
        {
            _invoiceRepository = invoiceRepository;
            _orderRepository   = orderRepository;
            _context           = context;
            _email             = email;
        }

        public async Task<Invoice> GenerateInvoiceAsync(GenerateInvoice dto)
        {
            var order = await _orderRepository.GetByIdAsync(dto.OrderId)
                ?? throw new NotFoundException("Order not found");

            if (order.Status != OrderStatus.Completed)
                throw new BadRequestException("Invoice can only be generated after wash completion");

            var invoice = new Invoice
            {
                OrderId           = order.OrderId,
                AfterWashImageUrl = dto.AfterWashImageUrl,
                GeneratedAt       = DateTime.UtcNow
            };

            var saved = await _invoiceRepository.CreateInvoiceAsync(invoice);

            // ── Invoice email to customer ──────────────────────────────────────
            var profile = await _context.CustomerProfiles
                .FirstOrDefaultAsync(c => c.CustomerId == order.CustomerId);

            if (profile != null)
            {
                var user = await _context.Users.FindAsync(profile.UserId);
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    var (subject, html) = EmailTemplates.InvoiceGenerated(
                        profile.FirstName, order.OrderId, saved.InvoiceId,
                        order.TotalAmount, dto.AfterWashImageUrl, saved.GeneratedAt);
                    await _email.SendAsync(user.Email, profile.FirstName, subject, html);
                }
            }

            return saved;
        }

        public async Task<object> GetInvoiceAsync(long orderId)
        {
            var invoice = await _invoiceRepository.GetInvoiceByOrderIdAsync(orderId)
                ?? throw new NotFoundException("Invoice not found");

            var order = await _orderRepository.GetByIdAsync(orderId)
                ?? throw new NotFoundException("Order not found");

            return new
            {
                invoice.InvoiceId,
                invoice.GeneratedAt,
                invoice.AfterWashImageUrl,
                order.OrderId,
                order.CustomerId,
                order.CarId,
                order.TotalAmount
            };
        }
    }
}
