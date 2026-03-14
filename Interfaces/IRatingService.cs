using GreenWash.DTO;
using GreenWash.Models;

namespace GreenWash.Interfaces
{
    public interface IRatingService
    {
        Task<Rating> SubmitRatingAsync(long reviewerId, SubmitRatingRequest request);
        Task<List<Rating>> GetRatingsByOrderAsync(long orderId);
        Task<object> GetRatingsByWasherAsync(long washerId);
    }
}
