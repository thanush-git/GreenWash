using GreenWash.DTO;
using GreenWash.Exceptions;
using GreenWash.Interfaces;
using GreenWash.Models;

namespace GreenWash.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEmailService _email;

        public AdminService(
            IAdminRepository adminRepository,
            IOrderRepository orderRepository,
            ICustomerRepository customerRepository,
            IEmailService email)
        {
            _adminRepository = adminRepository;
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _email = email;
        }

        // SERVICE PLANS

        public async Task<ServicePlan> CreateServicePlanAsync(CreateServicePlanRequest request)
        {
            var plan = new ServicePlan
            {
                Name = request.Name,
                Description = request.Description,
                BasePrice = request.BasePrice
            };
            return await _adminRepository.AddServicePlanAsync(plan);
        }

        public async Task<ServicePlan> UpdateServicePlanAsync(long id, CreateServicePlanRequest request)
        {
            var plan = await _adminRepository.GetServicePlanByIdAsync(id)
                ?? throw new NotFoundException("Service plan not found");

            plan.Name = request.Name;
            plan.Description = request.Description;
            plan.BasePrice = request.BasePrice;

            await _adminRepository.SaveChangesAsync();
            return plan;
        }

        public async Task UpdateServicePlanStatusAsync(long id, bool isActive)
        {
            var plan = await _adminRepository.GetServicePlanByIdAsync(id)
                ?? throw new NotFoundException("Service plan not found");

            plan.IsActive = isActive;
            await _adminRepository.SaveChangesAsync();
        }

        public async Task<List<ServicePlan>> GetAllServicePlansAsync()
            => await _adminRepository.GetAllServicePlansAsync();

        // ADD-ONS

        public async Task<AddOn> CreateAddOnAsync(CreateAddOnRequest request)
        {
            var addOn = new AddOn
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price
            };
            return await _adminRepository.AddAddOnAsync(addOn);
        }

        public async Task<AddOn> UpdateAddOnAsync(long id, CreateAddOnRequest request)
        {
            var addOn = await _adminRepository.GetAddOnByIdAsync(id)
                ?? throw new NotFoundException("Add-on not found");

            addOn.Name = request.Name;
            addOn.Description = request.Description;
            addOn.Price = request.Price;

            await _adminRepository.SaveChangesAsync();
            return addOn;
        }

        public async Task UpdateAddOnStatusAsync(long id, bool isActive)
        {
            var addOn = await _adminRepository.GetAddOnByIdAsync(id)
                ?? throw new NotFoundException("Add-on not found");

            addOn.IsActive = isActive;
            await _adminRepository.SaveChangesAsync();
        }

        public async Task<List<AddOn>> GetAllAddOnsAsync()
            => await _adminRepository.GetAllAddOnsAsync();

        // PROMO CODES

        public async Task<PromoCode> CreatePromoCodeAsync(CreatePromoCodeRequest request)
        {
            var promo = new PromoCode
            {
                Code = request.Code,
                DiscountType = request.DiscountType,
                DiscountValue = request.DiscountValue,
                ExpiryDate = request.ExpiryDate,
                UsageLimit = request.UsageLimit
            };
            return await _adminRepository.AddPromoCodeAsync(promo);
        }

        public async Task<PromoCode> UpdatePromoCodeAsync(long id, CreatePromoCodeRequest request)
        {
            var promo = await _adminRepository.GetPromoCodeByIdAsync(id)
                ?? throw new NotFoundException("Promo code not found");

            promo.Code = request.Code;
            promo.DiscountType = request.DiscountType;
            promo.DiscountValue = request.DiscountValue;
            promo.ExpiryDate = request.ExpiryDate;
            promo.UsageLimit = request.UsageLimit;

            await _adminRepository.SaveChangesAsync();
            return promo;
        }

        public async Task<List<PromoCode>> GetAllPromoCodesAsync()
            => await _adminRepository.GetAllPromoCodesAsync();

        // CUSTOMERS 

        public async Task<List<CustomerProfile>> GetAllCustomersAsync()
            => await _adminRepository.GetAllCustomersAsync();

        public async Task UpdateCustomerStatusAsync(long customerId, bool isActive)
        {
            var profile = await _customerRepository.GetByCustomerId(customerId)
                ?? throw new NotFoundException("Customer not found");

            var user = await _customerRepository.GetUserById(profile.UserId)
                ?? throw new NotFoundException("User not found");

            await _adminRepository.UpdateUserStatusAsync(user.UserId, isActive);
        }

        public async Task<List<Rating>> GetCustomerRatingsAsync(long customerId)
            => await _adminRepository.GetRatingsByRevieweeIdAsync(customerId);

        // WASHERS 

        public async Task<WasherProfile> AddWasherAsync(CreateWasherRequest dto)
        {
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.Washer,
                IsActive = true
            };

            var washer = new WasherProfile
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Phone = dto.Phone
            };

            await _adminRepository.AddWasherAsync(user, washer);

            var (subject, html) = EmailTemplates.WasherWelcome(dto.FirstName, dto.Email, dto.Password);
            await _email.SendAsync(dto.Email, dto.FirstName, subject, html);

            return washer;
        }

        public async Task<WasherProfile> UpdateWasherAsync(long washerId, UpdateWasherRequest dto)
        {
            var washer = await _adminRepository.GetWasherProfileByIdAsync(washerId)
                ?? throw new NotFoundException("Washer not found");

            washer.FirstName = dto.FirstName;
            washer.LastName = dto.LastName;
            washer.Phone = dto.Phone;

            await _adminRepository.UpdateWasherProfileAsync(washer);
            return washer;
        }

        public async Task ToggleWasherStatusAsync(long washerId, bool isActive)
            => await _adminRepository.UpdateUserStatusAsync(washerId, isActive);

        public async Task<List<WasherProfile>> GetAllWashersAsync()
            => await _adminRepository.GetAllWasherProfilesAsync();

        public async Task<List<Rating>> GetWasherRatingsAsync(long washerId)
            => await _adminRepository.GetRatingsByRevieweeIdAsync(washerId);

        // ORDERS

        public async Task<List<Order>> GetAllOrdersAsync(
            string? status, long? washerId, long? customerId,
            DateTime? startDate, DateTime? endDate)
            => await _adminRepository.GetAllOrdersAsync(status, washerId, customerId, startDate, endDate);

        public async Task<Order> AssignWasherToOrderAsync(long orderId, long washerId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId)
                ?? throw new NotFoundException("Order not found");

            if (order.Status != OrderStatus.Pending)
                throw new BadRequestException("Only pending orders can be assigned");

            var washer = await _adminRepository.GetWasherProfileByIdAsync(washerId)
                ?? throw new NotFoundException("Washer not found");

            order.WasherId = washerId;
            order.Status = OrderStatus.Accepted;
            await _orderRepository.UpdateAsync(order);

            var profile = await _customerRepository.GetByCustomerId(order.CustomerId);
            if (profile != null)
            {
                var user = await _customerRepository.GetUserById(profile.UserId);
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    var washerName = $"{washer.FirstName} {washer.LastName}";
                    var (subject, html) = EmailTemplates.OrderAccepted(
                        profile.FirstName, order.OrderId, washerName, order.ServiceAddress);
                    await _email.SendAsync(user.Email, profile.FirstName, subject, html);
                }
            }

            return order;
        }

        public async Task CancelOrderAsync(long orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId)
                ?? throw new NotFoundException("Order not found");

            order.Status = OrderStatus.Cancelled;
            await _orderRepository.UpdateAsync(order);

            var profile = await _customerRepository.GetByCustomerId(order.CustomerId);
            if (profile != null)
            {
                var user = await _customerRepository.GetUserById(profile.UserId);
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    var (subject, html) = EmailTemplates.OrderCancelled(profile.FirstName, order.OrderId);
                    await _email.SendAsync(user.Email, profile.FirstName, subject, html);
                }
            }
        }

        // LEADERBOARD

        private const double GallonsPerWash = 3.5;

        public async Task<List<object>> GetLeaderboardAsync()
        {
            var washers = await _adminRepository.GetAllWasherProfilesAsync();

            return washers
                .OrderByDescending(w => w.TotalWashes)
                .Select(w => (object)new
                {
                    w.WasherId,
                    w.FirstName,
                    w.LastName,
                    w.TotalWashes,
                    w.AverageRating,
                    GallonsSaved = Math.Round(w.TotalWashes * GallonsPerWash, 2)
                })
                .ToList();
        }
    }
}
