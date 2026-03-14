using System.ComponentModel.DataAnnotations;

namespace GreenWash.Models
{
    public class PromoCode
    {
        [Key]
        public long PromoCodeId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string DiscountType { get; set; } = string.Empty; // "Percentage" or "Fixed"
        public decimal DiscountValue { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
        public int UsageLimit { get; set; }
        public int UsageCount { get; set; } = 0;
    }
}
