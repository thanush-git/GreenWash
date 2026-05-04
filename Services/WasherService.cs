using GreenWash.DTO;
using GreenWash.Exceptions;
using GreenWash.Interfaces;
using GreenWash.Models;

namespace GreenWash.Services
{
    public class WasherService : IWasherService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IAdminRepository _washerRepository;
        private readonly IEmailService _email;

        public WasherService(
            IOrderRepository orderRepository,
            IAdminRepository washerRepository,
            IEmailService email)
        {
            _orderRepository = orderRepository;
            _washerRepository = washerRepository;
            _email = email;
        }

        public async Task HandleOrderActionAsync(long orderId, long washerId, string action)
        {
            var order = await _orderRepository.GetByIdAsync(orderId)
                ?? throw new NotFoundException("Order not found");

            switch (action.ToLower())
            {
                case "accept":
                    if (order.Status != OrderStatus.Pending)
                        throw new BadRequestException("Only pending orders can be accepted");

                    order.WasherId = washerId;
                    order.Status = OrderStatus.Accepted;
                    await _orderRepository.UpdateAsync(order);

                    var washer = await _washerRepository.GetWasherProfileByIdAsync(washerId);
                    var washerName = washer != null ? $"{washer.FirstName} {washer.LastName}" : "your washer";
                    var (acceptEmail, acceptName) = await _email.GetCustomerContactAsync(order.CustomerId);
                    if (!string.IsNullOrEmpty(acceptEmail))
                    {
                        var (s, h) = EmailTemplates.OrderAccepted(acceptName, order.OrderId, washerName, order.ServiceAddress);
                        await _email.SendAsync(acceptEmail, acceptName, s, h);
                    }
                    break;

                case "decline":
                    if (order.WasherId != washerId)
                        throw new UnauthorizedException("You are not assigned to this order");
                    if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Accepted)
                        throw new BadRequestException("This order cannot be declined");

                    order.Status = OrderStatus.Declined;
                    order.WasherId = null;
                    await _orderRepository.UpdateAsync(order);

                    var (declineEmail, declineName) = await _email.GetCustomerContactAsync(order.CustomerId);
                    if (!string.IsNullOrEmpty(declineEmail))
                    {
                        var (s, h) = EmailTemplates.OrderDeclined(declineName, order.OrderId);
                        await _email.SendAsync(declineEmail, declineName, s, h);
                    }
                    break;

                case "start":
                    if (order.WasherId != washerId)
                        throw new UnauthorizedException("You are not assigned to this order");
                    if (order.Status != OrderStatus.Accepted)
                        throw new BadRequestException("Order must be accepted before starting");

                    order.Status = OrderStatus.InProgress;
                    await _orderRepository.UpdateAsync(order);

                    var (startEmail, startName) = await _email.GetCustomerContactAsync(order.CustomerId);
                    if (!string.IsNullOrEmpty(startEmail))
                    {
                        var (s, h) = EmailTemplates.WashStarted(startName, order.OrderId, order.ServiceAddress);
                        await _email.SendAsync(startEmail, startName, s, h);
                    }
                    break;

                case "complete":
                    if (order.WasherId != washerId)
                        throw new UnauthorizedException("You are not assigned to this order");
                    if (order.Status != OrderStatus.InProgress)
                        throw new BadRequestException("Wash has not started yet");

                    order.Status = OrderStatus.Completed;
                    await _orderRepository.UpdateAsync(order);

                    var washerProfile = await _washerRepository.GetWasherProfileByIdAsync(washerId);
                    if (washerProfile != null)
                    {
                        washerProfile.TotalWashes++;
                        await _washerRepository.UpdateWasherProfileAsync(washerProfile);
                    }

                    var (completeEmail, completeName) = await _email.GetCustomerContactAsync(order.CustomerId);
                    if (!string.IsNullOrEmpty(completeEmail))
                    {
                        var (s, h) = EmailTemplates.WashCompleted(completeName, order.OrderId, order.TotalAmount);
                        await _email.SendAsync(completeEmail, completeName, s, h);
                    }
                    break;

                default:
                    throw new BadRequestException("Invalid action. Must be one of: accept, decline, start, complete");
            }
        }

        public async Task<List<Order>> GetAvailableOrdersAsync()
            => await _orderRepository.GetAvailableOrdersAsync();

        public async Task<List<Order>> GetWasherOrdersAsync(long washerId)
            => await _orderRepository.GetOrdersForWasherAsync(washerId);

        public async Task<WasherProfile> UpdateWasherAsync(long washerId, UpdateWasherRequest dto)
        {
            var washer = await _washerRepository.GetWasherProfileByIdAsync(washerId)
                ?? throw new NotFoundException("Washer not found");

            washer.FirstName = dto.FirstName;
            washer.LastName = dto.LastName;
            washer.Phone = dto.Phone;

            await _washerRepository.UpdateWasherProfileAsync(washer);
            return washer;
        }
    }
}
