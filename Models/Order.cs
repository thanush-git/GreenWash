using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenWash.Models
{
    public class Order
    {
        public long OrderId { get; set; }
        public long CustomerId { get; set; }
        public long CarId { get; set; }
        public long ServicePlanId { get; set; }
        public long? WasherId { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public string ServiceAddress { get; set; }
        public string? CustomerNotes { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPaid { get; set; } = false;
        public long? PromoCodeId { get; set; }
        public ICollection<OrderAddOn> AddOns { get; set; } = new List<OrderAddOn>();
    }
}