using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.Interfaces;
using GreenWash.Models;
using GreenWash.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenWash.DAL
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly GreenWashDbContext _context;

        public InvoiceRepository(GreenWashDbContext context)
        {
            _context = context;
        }

        public async Task<Invoice> CreateInvoiceAsync(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return invoice;
        }

        public async Task<Invoice?> GetInvoiceByOrderIdAsync(long orderId)
        {
            return await _context.Invoices
                .FirstOrDefaultAsync(i => i.OrderId == orderId);
        }
    }
}