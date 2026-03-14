using System.ComponentModel.DataAnnotations;

namespace GreenWash.Models
{
    public class AddOn
    {
        [Key]
        public long AddOnId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
