using GreenWash.Data;
using GreenWash.DTO;
using GreenWash.Exceptions;
using GreenWash.Interfaces;
using GreenWash.Models;
using Microsoft.EntityFrameworkCore;

namespace GreenWash.Services
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly GreenWashDbContext _context;

        public RatingService(IRatingRepository ratingRepository, GreenWashDbContext context)
        {
            _ratingRepository = ratingRepository;
            _context = context;
        }

        public async Task<Rating> SubmitRatingAsync(long reviewerId, SubmitRatingRequest request)
        {
            // 1. Validate score range
            if (request.Score < 1 || request.Score > 5)
                throw new BadRequestException("Score must be between 1 and 5");

            // 2. Fetch the order
            var order = await _context.Orders.FindAsync(request.OrderId)
                ?? throw new NotFoundException("Order not found");

            // 3. Order must be completed
            if (order.Status != OrderStatus.Completed)
                throw new BadRequestException("Ratings can only be submitted for completed orders");

            // 4. Order must have an assigned washer
            if (order.WasherId == null)
                throw new BadRequestException("This order has no assigned washer");

            // 5. Only the customer on this order can submit a rating
            var customerProfile = await _context.CustomerProfiles
                .FirstOrDefaultAsync(c => c.UserId == reviewerId)
                ?? throw new UnauthorizedException("Only customers can submit ratings");

            if (order.CustomerId != customerProfile.CustomerId)
                throw new UnauthorizedException("You are not a party to this order");

            // 6. Reviewee is always the washer
            long revieweeId = order.WasherId.Value;

            // 7. Duplicate check — one rating per customer per order
            var alreadyRated = await _ratingRepository.ExistsAsync(request.OrderId, reviewerId);
            if (alreadyRated)
                throw new BadRequestException("You have already rated this order");

            // 8. Save the rating
            var rating = new Rating
            {
                OrderId = request.OrderId,
                ReviewerId = reviewerId,
                RevieweeId = revieweeId,
                Score = request.Score,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            var saved = await _ratingRepository.AddAsync(rating);

            // 9. Recalculate washer average rating
            var washer = await _context.WasherProfiles.FindAsync(revieweeId);
            if (washer != null)
            {
                var allRatings = await _ratingRepository.GetByWasherIdAsync(washer.WasherId);
                washer.AverageRating = Math.Round(allRatings.Average(r => r.Score), 2);
                await _context.SaveChangesAsync();
            }

            return saved;
        }

        public async Task<List<Rating>> GetRatingsByOrderAsync(long orderId)
        {
            return await _ratingRepository.GetByOrderIdAsync(orderId);
        }

        public async Task<object> GetRatingsByWasherAsync(long washerId)
        {
            var ratings = await _ratingRepository.GetByWasherIdAsync(washerId);
            var average = ratings.Count > 0 ? ratings.Average(r => r.Score) : 0;

            return new
            {
                WasherId = washerId,
                AverageRating = Math.Round(average, 2),
                Ratings = ratings
            };
        }
    }
}