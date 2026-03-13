using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenWash.Models
{
    public enum OrderStatus
    {
        Pending,
        Accepted,
        Declined,
        InProgress,
        Completed,
        Cancelled
    }
}