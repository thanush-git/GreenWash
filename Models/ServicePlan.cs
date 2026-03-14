using System.ComponentModel.DataAnnotations;

namespace GreenWash.Models
{
    public class ServicePlan
    {
        [Key]
        public long ServicePlanId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
