using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenWash.DTO
{
    public class ProcessPayment
    {
        public int OrderId { get; set; }
        public int PaymentMethodId { get; set; }
    }
}