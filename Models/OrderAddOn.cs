using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GreenWash.Models
{
    public class OrderAddOn
    {
        public long OrderAddOnId { get; set; }

        public long OrderId { get; set; }

        public long AddOnId { get; set; }

        public decimal PriceSnapshot { get; set; }

        [JsonIgnore]
        public Order Order { get; set; }
    }
}