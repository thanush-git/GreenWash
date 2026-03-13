using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.Models;
using GreenWash.Data;

namespace GreenWash.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<Invoice> CreateInvoiceAsync(Invoice invoice);
        Task<Invoice?> GetInvoiceByOrderIdAsync(long orderId);
    }
}