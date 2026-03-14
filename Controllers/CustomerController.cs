using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GreenWash.DTO;
using GreenWash.Interfaces;
using System.Security.Claims;

namespace GreenWash.Controllers
{
    [ApiController]
    [Route("api/customers")]
    [Authorize(Roles = "Customer")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ICarService _carService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IAddPaymentMethodService _paymentMethodService;
        private readonly IInvoiceService _invoiceService;
        private readonly IRatingService _ratingService;

        public CustomerController(
            ICustomerService customerService,
            ICarService carService,
            IOrderService orderService,
            IPaymentService paymentService,
            IAddPaymentMethodService paymentMethodService,
            IInvoiceService invoiceService,
            IRatingService ratingService)
        {
            _customerService = customerService;
            _carService = carService;
            _orderService = orderService;
            _paymentService = paymentService;
            _paymentMethodService = paymentMethodService;
            _invoiceService = invoiceService;
            _ratingService = ratingService;
        }

        private long GetUserId() =>
            long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // ── PROFILE ────────────────────────────────────────────────────────────

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var customer = await _customerService.GetProfile(GetUserId());
            return Ok(customer);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(UpdateCustomerProfileRequest request)
        {
            await _customerService.UpdateProfile(GetUserId(), request);
            return Ok("Profile updated successfully");
        }

        // ── CARS ───────────────────────────────────────────────────────────────

        [HttpPost("cars")]
        public async Task<IActionResult> AddCar(CreateCarRequest request)
        {
            await _carService.AddCar(GetUserId(), request);
            return Ok("Car added successfully");
        }

        // [HttpGet("cars")]
        // public async Task<IActionResult> GetMyCars()
        // {
        //     var cars = await _carService.GetMyCars(GetUserId());
        //     return Ok(cars);
        // }

        // [HttpDelete("cars/{id}")]
        // public async Task<IActionResult> DeleteCar(long id)
        // {
        //     await _carService.DeleteCar(id, GetUserId());
        //     return Ok("Car deleted successfully");
        // }

        // ── ORDERS ─────────────────────────────────────────────────────────────

        [HttpPost("orders/wash-now")]
        public async Task<IActionResult> WashNow(WashNowRequest request)
        {
            var order = await _orderService.CreateWashNowAsync(GetUserId(), request);
            return Ok(order);
        }

        [HttpPost("orders/schedule")]
        public async Task<IActionResult> ScheduleWash(ScheduleWashRequest request)
        {
            var order = await _orderService.ScheduleWashAsync(GetUserId(), request);
            return Ok(order);
        }

        [HttpGet("orders/current")]
        public async Task<IActionResult> GetCurrentOrders()
        {
            var orders = await _orderService.GetCurrentOrdersAsync(GetUserId());
            return Ok(orders);
        }

        [HttpGet("orders/past")]
        public async Task<IActionResult> GetPastOrders()
        {
            var orders = await _orderService.GetPastOrdersAsync(GetUserId());
            return Ok(orders);
        }

        // [HttpDelete("orders/{id}")]
        // public async Task<IActionResult> CancelOrder(long id)
        // {
        //     await _orderService.CancelOrderAsync(id, GetUserId());
        //     return Ok("Order cancelled");
        // }

        // ── PAYMENT METHODS ────────────────────────────────────────────────────

        [HttpPost("payment-methods")]
        public async Task<IActionResult> AddPaymentMethod(AddPaymentMethod dto)
        {
            var result = await _paymentMethodService.AddPaymentMethodAsync(GetUserId(), dto);
            return Ok(result);
        }

        // [HttpGet("payment-methods")]
        // public async Task<IActionResult> GetPaymentMethods()
        // {
        //     var result = await _paymentMethodService.GetCustomerPaymentMethodsAsync(GetUserId());
        //     return Ok(result);
        // }

        // ── PAYMENT ────────────────────────────────────────────────────────────
        // PromoCode is optional inside ProcessPayment — send it here to apply at checkout.
        // If the promo is invalid the payment will NOT be processed and an error is returned.

        [HttpPost("payments")]
        public async Task<IActionResult> Pay(ProcessPayment request)
        {
            var result = await _paymentService.ProcessPaymentAsync(request, GetUserId());
            return Ok(result);
        }

        // ── INVOICES ───────────────────────────────────────────────────────────

        [HttpGet("orders/{orderId}/invoice")]
        public async Task<IActionResult> GetInvoice(long orderId)
        {
            var invoice = await _invoiceService.GetInvoiceAsync(orderId);
            return Ok(invoice);
        }

        // ── RATINGS ────────────────────────────────────────────────────────────

        [HttpPost("ratings")]
        public async Task<IActionResult> SubmitRating(SubmitRatingRequest request)
        {
            var rating = await _ratingService.SubmitRatingAsync(GetUserId(), request);
            return Ok(rating);
        }
    }
}
