using GreenWash.Data;
using GreenWash.DTO;
using GreenWash.Exceptions;
using GreenWash.Interfaces;
using GreenWash.Models;
using Microsoft.EntityFrameworkCore;

namespace GreenWash.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly GreenWashDbContext _context;
        private readonly IEmailService _email;

        public OrderService(IOrderRepository repository, GreenWashDbContext context, IEmailService email)
        {
            _repository = repository;
            _context = context;
            _email = email;
        }

        // ── Helpers ────────────────────────────────────────────────────────────────

        private async Task<long> ResolveCustomerIdAsync(long userId)
        {
            var profile = await _context.CustomerProfiles
                .FirstOrDefaultAsync(c => c.UserId == userId)
                ?? throw new NotFoundException("Customer profile not found");
            return profile.CustomerId;
        }

        private async Task<(string email, string firstName)> GetCustomerContactAsync(long customerId)
        {
            var profile = await _context.CustomerProfiles.FindAsync(customerId);
            if (profile == null) return ("", "Customer");
            var user = await _context.Users.FindAsync(profile.UserId);
            return (user?.Email ?? "", profile.FirstName);
        }

        private async Task<(CustomerProfile profile, ServicePlan plan, List<AddOn> addOns)>
            ValidateAndFetchAsync(long userId, long carId, long servicePlanId, List<long>? addOnIds)
        {
            var profile = await _context.CustomerProfiles
                .FirstOrDefaultAsync(c => c.UserId == userId)
                ?? throw new NotFoundException("Customer profile not found");

            var car = await _context.Cars.FirstOrDefaultAsync(c => c.CarId == carId)
                ?? throw new NotFoundException("Car not found");

            if (!car.IsActive)
                throw new BadRequestException("This car has been removed from your account");

            if (car.CustomerId != profile.CustomerId)
                throw new UnauthorizedException("This car does not belong to you");

            var plan = await _context.ServicePlans
                .FirstOrDefaultAsync(s => s.ServicePlanId == servicePlanId && s.IsActive)
                ?? throw new BadRequestException("Service plan not found or is no longer available");

            var addOns = new List<AddOn>();
            if (addOnIds != null && addOnIds.Count > 0)
            {
                var uniqueIds = addOnIds.Distinct().ToList();
                addOns = await _context.AddOns.Where(a => uniqueIds.Contains(a.AddOnId)).ToListAsync();

                var missing = uniqueIds.Except(addOns.Select(a => a.AddOnId)).ToList();
                if (missing.Any())
                    throw new BadRequestException($"Add-on(s) not found: {string.Join(", ", missing)}");

                var inactive = addOns.Where(a => !a.IsActive).ToList();
                if (inactive.Any())
                    throw new BadRequestException($"Add-on(s) not available: {string.Join(", ", inactive.Select(a => a.Name))}");
            }

            return (profile, plan, addOns);
        }

        private static decimal CalculateTotal(ServicePlan plan, List<AddOn> addOns)
            => plan.BasePrice + addOns.Sum(a => a.Price);

        private static IEnumerable<OrderAddOn> BuildOrderAddOns(List<AddOn> addOns)
            => addOns.Select(a => new OrderAddOn { AddOnId = a.AddOnId, PriceSnapshot = a.Price });

        // ── CreateWashNowAsync ─────────────────────────────────────────────────────

        public async Task<Order> CreateWashNowAsync(long userId, WashNowRequest request)
        {
            var (profile, plan, addOns) = await ValidateAndFetchAsync(
                userId, request.CarId, request.ServicePlanId, request.AddOnIds);

            var order = new Order
            {
                CustomerId = profile.CustomerId,
                CarId = request.CarId,
                ServicePlanId = request.ServicePlanId,
                ServiceAddress = request.ServiceAddress,
                CustomerNotes = request.Notes,
                Status = OrderStatus.Pending,
                TotalAmount = CalculateTotal(plan, addOns),
                CreatedAt = DateTime.UtcNow
            };

            foreach (var a in BuildOrderAddOns(addOns)) order.AddOns.Add(a);
            return await _repository.AddAsync(order);
        }

        // ── ScheduleWashAsync ──────────────────────────────────────────────────────

        public async Task<Order> ScheduleWashAsync(long userId, ScheduleWashRequest request)
        {
            if (request.ScheduledAt <= DateTime.UtcNow)
                throw new BadRequestException("Scheduled date must be in the future");

            var (profile, plan, addOns) = await ValidateAndFetchAsync(
                userId, request.CarId, request.ServicePlanId, request.AddOnIds);

            var order = new Order
            {
                CustomerId = profile.CustomerId,
                CarId = request.CarId,
                ServicePlanId = request.ServicePlanId,
                ServiceAddress = request.ServiceAddress,
                CustomerNotes = request.Notes,
                ScheduledAt = request.ScheduledAt,
                Status = OrderStatus.Pending,
                TotalAmount = CalculateTotal(plan, addOns),
                CreatedAt = DateTime.UtcNow
            };

            foreach (var a in BuildOrderAddOns(addOns)) order.AddOns.Add(a);
            return await _repository.AddAsync(order);
        }

        // ── GetCurrentOrdersAsync / GetPastOrdersAsync ─────────────────────────────

        public async Task<List<Order>> GetCurrentOrdersAsync(long userId)
        {
            var customerId = await ResolveCustomerIdAsync(userId);
            return await _repository.GetCurrentOrdersAsync(customerId);
        }

        public async Task<List<Order>> GetPastOrdersAsync(long userId)
        {
            var customerId = await ResolveCustomerIdAsync(userId);
            return await _repository.GetPastOrdersAsync(customerId);
        }

        // ── CancelOrderAsync ───────────────────────────────────────────────────────

        public async Task CancelOrderAsync(long orderId, long userId)
        {
            var order = await _repository.GetByIdAsync(orderId)
                ?? throw new NotFoundException("Order not found");

            var customerId = await ResolveCustomerIdAsync(userId);
            if (order.CustomerId != customerId)
                throw new UnauthorizedException("You are not authorized to cancel this order");

            if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled)
                throw new BadRequestException("Order cannot be cancelled in its current state");

            order.Status = OrderStatus.Cancelled;
            await _repository.UpdateAsync(order);

            var (customerEmail, customerName) = await GetCustomerContactAsync(order.CustomerId);
            if (!string.IsNullOrEmpty(customerEmail))
            {
                var (subject, html) = EmailTemplates.OrderCancelled(customerName, order.OrderId);
                await _email.SendAsync(customerEmail, customerName, subject, html);
            }
        }
    }
}
