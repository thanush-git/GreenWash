using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.DTO;
using GreenWash.Interfaces;
using GreenWash.Models;

namespace GreenWash.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IOrderRepository _orderRepository;

        public InvoiceService(IInvoiceRepository invoiceRepository, IOrderRepository orderRepository)
        {
            _invoiceRepository = invoiceRepository;
            _orderRepository = orderRepository;
        }

        public async Task<Invoice> GenerateInvoiceAsync(GenerateInvoice dto)
        {
            var order = await _orderRepository.GetByIdAsync(dto.OrderId);

            if (order == null)
                throw new Exception("Order not found");

            if (order.Status != OrderStatus.Completed)
                throw new Exception("Invoice can only be generated after wash completion");

            var invoice = new Invoice
            {
                OrderId = order.OrderId,
                AfterWashImageUrl = dto.AfterWashImageUrl
            };

            return await _invoiceRepository.CreateInvoiceAsync(invoice);
        }

        public async Task<object> GetInvoiceAsync(long orderId)
        {
            var invoice = await _invoiceRepository.GetInvoiceByOrderIdAsync(orderId);

            if (invoice == null)
                throw new Exception("Invoice not found");

            var order = await _orderRepository.GetByIdAsync(orderId);

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