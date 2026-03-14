using System.ComponentModel.DataAnnotations;

namespace GreenWash.Models
{
    public class Rating
    {
        [Key]
        public long RatingId { get; set; }
        public long OrderId { get; set; }
        public long ReviewerId { get; set; }
        public long RevieweeId { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
