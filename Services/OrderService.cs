using GreenWash.DTO;
using GreenWash.Exceptions;
using GreenWash.Interfaces;
using GreenWash.Models;

namespace GreenWash.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly ICustomerRepository _customer;
        private readonly ICarRepository _car;
        private readonly IEmailService _email;

        public OrderService
        (IOrderRepository repository, ICustomerRepository customer, ICarRepository car, IEmailService email)
        {
            _repository = repository;
            _customer = customer;
            _car = car;
            _email = email;
        }

        // Helper methods
        private async Task<long> ResolveCustomerIdAsync(long userId)
        {
            var profile = await _customer.GetByUserId(userId)
                ?? throw new NotFoundException("Customer profile not found");

            return profile.CustomerId;
        }

        private async Task<(CustomerProfile profile, ServicePlan plan, List<AddOn> addOns)>
        ValidateAndFetchAsync(long userId, long carId, long servicePlanId, List<long>? addOnIds)
        {
            var profile = await _customer.GetByUserId(userId)
                ?? throw new NotFoundException("Customer profile not found");

            var car = await _car.GetCarById(carId)
                ?? throw new NotFoundException("Car not found");

            if (!car.IsActive)
                throw new BadRequestException("This car has been removed from your account");

            if (car.CustomerId != profile.CustomerId)
                throw new UnauthorizedException("This car does not belong to you");

            var plan = await _repository.GetActiveServicePlanAsync(servicePlanId)
                ?? throw new BadRequestException("Service plan not found or is no longer available");

            var addOns = new List<AddOn>();

            if (addOnIds != null && addOnIds.Count > 0)
            {
                var uniqueIds = addOnIds.Distinct().ToList();
                addOns = await _repository.GetAddOnsByIdsAsync(uniqueIds);

                var missing = uniqueIds.Except(addOns.Select(a => a.AddOnId)).ToList();

                if (missing.Any())
                    throw new BadRequestException($"Add-on not found: {string.Join(", ", missing)}");

                var inactive = addOns.Where(a => !a.IsActive).ToList();

                if (inactive.Any())
                    throw new BadRequestException($"Add-on not available: {string.Join(", ", inactive.Select(a => a.Name))}");
            }

            return (profile, plan, addOns);
        }

        // CreateWashNowAsync 
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
                TotalAmount = _repository.CalculateTotal(plan, addOns),
                CreatedAt = DateTime.UtcNow
            };

            foreach (var a in _repository.BuildOrderAddOns(addOns))
                order.AddOns.Add(a);

            return await _repository.AddAsync(order);
        }

        // ScheduleWashAsync
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
                TotalAmount = _repository.CalculateTotal(plan, addOns),
                CreatedAt = DateTime.UtcNow
            };

            foreach (var a in _repository.BuildOrderAddOns(addOns))
                order.AddOns.Add(a);

            return await _repository.AddAsync(order);
        }

        // GetCurrentOrdersAsync / GetPastOrdersAsync for customers

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

        // CancelOrderAsync for Admins
        public async Task CancelOrderAsync(long orderId, long userId)
        {
            var order = await _repository.GetByIdAsync(orderId)
                ?? throw new NotFoundException("Order not found");

            if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled)
                throw new BadRequestException("Order cannot be cancelled in its current state");

            order.Status = OrderStatus.Cancelled;
            await _repository.UpdateAsync(order);

            var (customerEmail, customerName) = await _email.GetCustomerContactAsync(order.CustomerId);
            if (!string.IsNullOrEmpty(customerEmail))
            {
                var (subject, html) = EmailTemplates.OrderCancelled(customerName, order.OrderId);
                await _email.SendAsync(customerEmail, customerName, subject, html);
            }
        }
    }
}
