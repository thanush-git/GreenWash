using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenWash.DTO
{
    public class AddPaymentMethod
    {
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; } 
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public bool IsDefault { get; set; }
    }
}