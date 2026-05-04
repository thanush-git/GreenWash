using GreenWash.Models;

namespace GreenWash.Interfaces
{
    public interface IRatingRepository
    {
        Task<Rating> AddAsync(Rating rating);
        Task<List<Rating>> GetByOrderIdAsync(long orderId);
        Task<List<Rating>> GetByWasherIdAsync(long washerId);
        Task<bool> ExistsAsync(long orderId, long reviewerId);
        Task SaveChangesAsync();
    }
}
