using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.Models;
using GreenWash.DTO;

namespace GreenWash.Interfaces
{
    public interface IInvoiceService
    {
        Task<Invoice> GenerateInvoiceAsync(GenerateInvoice dto);
        Task<object> GetInvoiceAsync(long orderId);
    }
}