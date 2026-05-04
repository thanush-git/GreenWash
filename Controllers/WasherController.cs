using GreenWash.DTO;
using GreenWash.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GreenWash.Controllers
{
    [ApiController]
    [Route("api/washers")]
    [Authorize(Roles = "Washer")]
    public class WasherController : ControllerBase
    {
        private readonly IWasherService _washerService;
        private readonly IInvoiceService _invoiceService;
        private readonly IRatingService _ratingService;
        public WasherController(
            IWasherService washerService,
            IInvoiceService invoiceService,
            IRatingService ratingService
            )
        {
            _washerService = washerService;
            _invoiceService = invoiceService;
            _ratingService = ratingService;
        }

        private long GetWasherId() =>
            long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // PROFILE

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(UpdateWasherRequest dto)
        {
            var result = await _washerService.UpdateWasherAsync(GetWasherId(), dto);
            return Ok(result);
        }

        // ORDERS

        [HttpGet("orders/available")]
        public async Task<IActionResult> GetAvailableOrders()
        {
            var orders = await _washerService.GetAvailableOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var orders = await _washerService.GetWasherOrdersAsync(GetWasherId());
            return Ok(orders);
        }

        /// Unified order action endpoint.
        /// Body: { "action": "accept" | "decline" | "start" | "complete" }
        [HttpPatch("orders/{orderId}")]
        public async Task<IActionResult> HandleOrderAction(long orderId, WasherOrderAction dto)
        {
            await _washerService.HandleOrderActionAsync(orderId, GetWasherId(), dto.Action);
            return Ok(new { message = $"Order {dto.Action}ed successfully" });
        }

        // INVOICES 

        [HttpPost("orders/{orderId}/invoice")]
        public async Task<IActionResult> GenerateInvoice(long orderId, GenerateInvoice dto)
        {
            dto.OrderId = orderId;
            var invoice = await _invoiceService.GenerateInvoiceAsync(dto);
            return Ok(invoice);
        }

        // Rate Customers
        [HttpPost("rate-customer")]
        public async Task<IActionResult> RateCustomer(SubmitRatingRequest request)
        {
            var washerId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _ratingService.SubmitRatingAsync(washerId, request);
            return Ok(result);
        }
    }
}
