using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenWash.Models
{
    public class PaymentTransaction
    {
        public int PaymentTransactionId { get; set; }
        public int OrderId { get; set; }
        public int PaymentMethodId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Order Order { get; set; }
    }
}