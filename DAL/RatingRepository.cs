using GreenWash.Data;
using GreenWash.Interfaces;
using GreenWash.Models;
using Microsoft.EntityFrameworkCore;

namespace GreenWash.DAL
{
    public class RatingRepository : IRatingRepository
    {
        private readonly GreenWashDbContext _context;

        public RatingRepository(GreenWashDbContext context)
        {
            _context = context;
        }

        public async Task<Rating> AddAsync(Rating rating)
        {
            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();
            return rating;
        }

        public async Task<List<Rating>> GetByOrderIdAsync(long orderId)
        {
            return await _context.Ratings
                .Where(r => r.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<List<Rating>> GetByWasherIdAsync(long washerId)
        {
            return await _context.Ratings
                .Where(r => r.RevieweeId == washerId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(long orderId, long reviewerId)
        {
            return await _context.Ratings
                .AnyAsync(r => r.OrderId == orderId && r.ReviewerId == reviewerId);
        }
    }
}
