using GreenWash.DTO;
using GreenWash.Exceptions;
using GreenWash.Interfaces;
using GreenWash.Models;

namespace GreenWash.Services
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IAdminRepository _adminRepository;

        public RatingService(
            IRatingRepository ratingRepository,
            IOrderRepository orderRepository,
            ICustomerRepository customerRepository,
            IAdminRepository adminRepository)
        {
            _ratingRepository = ratingRepository;
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _adminRepository = adminRepository;
        }

        public async Task<Rating> SubmitRatingAsync(long reviewerId, SubmitRatingRequest request)
        {
            if (request.Score < 1 || request.Score > 5)
                throw new BadRequestException("Score must be between 1 and 5");

            var order = await _orderRepository.GetByIdAsync(request.OrderId)
                ?? throw new NotFoundException("Order not found");

            if (order.Status != OrderStatus.Completed)
                throw new BadRequestException("Ratings can only be submitted for completed orders");

            if (order.WasherId == null)
                throw new BadRequestException("This order has no assigned washer");

            var alreadyRated = await _ratingRepository.ExistsAsync(request.OrderId, reviewerId);
            if (alreadyRated)
                throw new BadRequestException("You have already rated this order");

            // check if reviewer is a customer or a washer
            var customerProfile = await _customerRepository.GetByUserId(reviewerId);
            var washerProfile = await _adminRepository.GetWasherProfileByIdAsync(reviewerId);

            if (customerProfile == null && washerProfile == null)
                throw new UnauthorizedException("You are not a party to this order");

            long revieweeId;

            if (customerProfile != null)
            {
                // customer rating the washer
                if (order.CustomerId != customerProfile.CustomerId)
                    throw new UnauthorizedException("You are not a party to this order");

                revieweeId = order.WasherId.Value;
            }
            else
            {
                // washer rating the customer
                if (order.WasherId != reviewerId)
                    throw new UnauthorizedException("You are not assigned to this order");

                revieweeId = order.CustomerId;
            }

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

            // update washer average rating only when a customer rates a washer
            if (customerProfile != null)
            {
                var washer = await _adminRepository.GetWasherProfileByIdAsync(revieweeId);
                if (washer != null)
                {
                    var allRatings = await _ratingRepository.GetByWasherIdAsync(washer.WasherId);
                    washer.AverageRating = Math.Round(allRatings.Average(r => r.Score), 2);
                    await _ratingRepository.SaveChangesAsync();
                }
            }

            return saved;
        }

        public async Task<List<Rating>> GetRatingsByOrderAsync(long orderId)
            => await _ratingRepository.GetByOrderIdAsync(orderId);

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
