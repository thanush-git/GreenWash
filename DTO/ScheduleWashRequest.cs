using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenWash.DTO
{
    public class ScheduleWashRequest
    {
        public long CarId { get; set; }

        public long ServicePlanId { get; set; }

        public List<long>? AddOnIds { get; set; }

        public DateTime ScheduledAt { get; set; }

        public string ServiceAddress { get; set; } = string.Empty;

        public string? Notes { get; set; }
    }
}