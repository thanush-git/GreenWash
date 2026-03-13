using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public WasherController(IWasherService washerService, IInvoiceService invoiceService)
        {
            _washerService = washerService;
            _invoiceService = invoiceService;
        }

        private int GetWasherId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
                throw new Exception("WasherId not found in token");

            return int.Parse(claim.Value);
        }

        [HttpGet("orders/available")]
        public async Task<IActionResult> GetAvailableOrders()
        {
            var orders = await _washerService.GetAvailableOrdersAsync();

            return Ok(orders);
        }

        [HttpPost("orders/{orderId}/accept")]
        public async Task<IActionResult> AcceptOrder(int orderId)
        {
            var washerId = GetWasherId();

            await _washerService.AcceptOrderAsync(orderId, washerId);

            return Ok(new
            {
                message = "Order accepted"
            });
        }

        [HttpPost("orders/{orderId}/decline")]
        public async Task<IActionResult> DeclineOrder(int orderId)
        {
            var washerId = GetWasherId();

            await _washerService.DeclineOrderAsync(orderId, washerId);

            return Ok(new
            {
                message = "Order declined"
            });
        }

        [HttpPatch("orders/{orderId}/start")]
        public async Task<IActionResult> StartWash(int orderId)
        {
            var washerId = GetWasherId();

            await _washerService.StartWashAsync(orderId, washerId);

            return Ok(new
            {
                message = "Wash started"
            });
        }

        [HttpPatch("orders/{orderId}/complete")]
        public async Task<IActionResult> CompleteWash(int orderId)
        {
            var washerId = GetWasherId();

            await _washerService.CompleteWashAsync(orderId, washerId);

            return Ok(new
            {
                message = "Wash completed"
            });
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var washerId = GetWasherId();

            var orders = await _washerService.GetWasherOrdersAsync(washerId);

            return Ok(orders);
        }

        //Invoice Generation


        // Invoice Generation

        [HttpPost("orders/{orderId}/invoice")]
        public async Task<IActionResult> GenerateInvoice(int orderId, GenerateInvoice dto)
        {
            dto.OrderId = orderId;

            var invoice = await _invoiceService.GenerateInvoiceAsync(dto);

            return Ok(invoice);
        }

        [HttpGet("orders/{orderId}/invoice")]
        public async Task<IActionResult> GetInvoice(int orderId)
        {
            var invoice = await _invoiceService.GetInvoiceAsync(orderId);

            if (invoice == null)
                return NotFound();

            return Ok(invoice);
        }
    }
}
