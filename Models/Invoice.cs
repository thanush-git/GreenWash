using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenWash.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }

        public long OrderId { get; set; }

        public string AfterWashImageUrl { get; set; }

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}