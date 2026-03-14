using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GreenWash.DTO;
using GreenWash.Interfaces;

namespace GreenWash.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminWasherService _washerService;
        private readonly IAdminService _adminService;

        public AdminController(IAdminWasherService washerService, IAdminService adminService)
        {
            _washerService = washerService;
            _adminService = adminService;
        }

        // ── WASHER MANAGEMENT ──────────────────────────────────────────────────────

        [HttpPost("washers")]
        public async Task<IActionResult> AddWasher(CreateWasher dto)
        {
            var washer = await _washerService.AddWasherAsync(dto);
            return Ok(washer);
        }

        // [HttpGet("washers")]
        // public async Task<IActionResult> GetAllWashers()
        // {
        //     var washers = await _adminService.GetAllWashersAsync();
        //     return Ok(washers);
        // }

        [HttpPut("washers/{id}")]
        public async Task<IActionResult> UpdateWasher(long id, UpdateWasher dto)
        {
            var washer = await _washerService.UpdateWasherAsync(id, dto);
            return Ok(washer);
        }

        [HttpPatch("washers/{id}/status")]
        public async Task<IActionResult> ToggleWasherStatus(long id, WasherStatus dto)
        {
            await _washerService.ToggleWasherStatusAsync(id, dto.IsActive);
            return Ok("Washer status updated");
        }

        [HttpGet("washers/{id}/ratings")]
        public async Task<IActionResult> GetWasherRatings(long id)
        {
            var ratings = await _adminService.GetWasherRatingsAsync(id);
            return Ok(ratings);
        }

        // ── CUSTOMER MANAGEMENT ────────────────────────────────────────────────────

        // [HttpGet("customers")]
        // public async Task<IActionResult> GetAllCustomers()
        // {
        //     var customers = await _adminService.GetAllCustomersAsync();
        //     return Ok(customers);
        // }

        [HttpPatch("customers/{id}/status")]
        public async Task<IActionResult> ToggleCustomerStatus(long id, ToggleStatusRequest dto)
        {
            await _adminService.UpdateCustomerStatusAsync(id, dto.IsActive);
            return Ok("Customer status updated");
        }

        // ── SERVICE PLANS ──────────────────────────────────────────────────────────

        [HttpPost("service-plans")]
        public async Task<IActionResult> CreateServicePlan(CreateServicePlanRequest request)
        {
            var plan = await _adminService.CreateServicePlanAsync(request);
            return Ok(plan);
        }

        // [HttpGet("service-plans")]
        // public async Task<IActionResult> GetServicePlans()
        // {
        //     var plans = await _adminService.GetAllServicePlansAsync();
        //     return Ok(plans);
        // }

        [HttpPut("service-plans/{id}")]
        public async Task<IActionResult> UpdateServicePlan(long id, CreateServicePlanRequest request)
        {
            var plan = await _adminService.UpdateServicePlanAsync(id, request);
            return Ok(plan);
        }

        [HttpPatch("service-plans/{id}/status")]
        public async Task<IActionResult> ToggleServicePlanStatus(long id, ToggleStatusRequest dto)
        {
            await _adminService.UpdateServicePlanStatusAsync(id, dto.IsActive);
            return Ok("Status updated");
        }

        // ── ADD-ONS ────────────────────────────────────────────────────────────────

        [HttpPost("addons")]
        public async Task<IActionResult> CreateAddOn(CreateAddOnRequest request)
        {
            var addOn = await _adminService.CreateAddOnAsync(request);
            return Ok(addOn);
        }

        // [HttpGet("addons")]
        // public async Task<IActionResult> GetAddOns()
        // {
        //     var addOns = await _adminService.GetAllAddOnsAsync();
        //     return Ok(addOns);
        // }

        [HttpPut("addons/{id}")]
        public async Task<IActionResult> UpdateAddOn(long id, CreateAddOnRequest request)
        {
            var addOn = await _adminService.UpdateAddOnAsync(id, request);
            return Ok(addOn);
        }

        [HttpPatch("addons/{id}/status")]
        public async Task<IActionResult> ToggleAddOnStatus(long id, ToggleStatusRequest dto)
        {
            await _adminService.UpdateAddOnStatusAsync(id, dto.IsActive);
            return Ok("Status updated");
        }

        // ── PROMO CODES ────────────────────────────────────────────────────────────

        [HttpPost("promo-codes")]
        public async Task<IActionResult> CreatePromoCode(CreatePromoCodeRequest request)
        {
            var promo = await _adminService.CreatePromoCodeAsync(request);
            return Ok(promo);
        }

        // [HttpGet("promo-codes")]
        // public async Task<IActionResult> GetPromoCodes()
        // {
        //     var promos = await _adminService.GetAllPromoCodesAsync();
        //     return Ok(promos);
        // }

        [HttpPut("promo-codes/{id}")]
        public async Task<IActionResult> UpdatePromoCode(long id, CreatePromoCodeRequest request)
        {
            var promo = await _adminService.UpdatePromoCodeAsync(id, request);
            return Ok(promo);
        }

        // ── ORDER MANAGEMENT ───────────────────────────────────────────────────────

        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrders(
            [FromQuery] string? status,
            [FromQuery] long? washerId,
            [FromQuery] long? customerId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var orders = await _adminService.GetAllOrdersAsync(status, washerId, customerId, startDate, endDate);
            return Ok(orders);
        }

        [HttpPost("orders/{orderId}/assign")]
        public async Task<IActionResult> AssignWasher(long orderId, AssignWasherRequest request)
        {
            var order = await _adminService.AssignWasherToOrderAsync(orderId, request.WasherId);
            return Ok(order);
        }

        [HttpDelete("orders/{orderId}")]
        public async Task<IActionResult> CancelOrder(long orderId)
        {
            await _adminService.CancelOrderAsync(orderId);
            return Ok("Order cancelled");
        }
    }
}
