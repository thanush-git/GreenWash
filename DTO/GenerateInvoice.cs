using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenWash.DTO
{
    public class GenerateInvoice
    {
        public int OrderId { get; set; }

        public string AfterWashImageUrl { get; set; }
    }
}