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


        public CustomerController(
            ICustomerService customerService,
            ICarService carService,
            IOrderService orderService,
            IPaymentService paymentService,
            IAddPaymentMethodService paymentMethodService,
            IInvoiceService invoiceService
            )
        {
            _customerService = customerService;
            _carService = carService;
            _orderService = orderService;
            _paymentService = paymentService;
            _paymentMethodService = paymentMethodService;
            _invoiceService = invoiceService;
        }

        private long GetUserId()
        {
            return long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        // PROFILE

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();
            var customer = await _customerService.GetProfile(userId);
            return Ok(customer);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(UpdateCustomerProfileRequest request)
        {
            var userId = GetUserId();
            await _customerService.UpdateProfile(userId, request);
            return Ok("Profile updated successfully");
        }

        // CAR MANAGEMENT

        [HttpPost("cars")]
        public async Task<IActionResult> AddCar(CreateCarRequest request)
        {
            var userId = GetUserId();
            await _carService.AddCar(userId, request);
            return Ok("Car added successfully");
        }

        // [HttpGet("cars")]
        // public async Task<IActionResult> GetMyCars()
        // {
        //     var userId = GetUserId();
        //     var cars = await _carService.GetMyCars(userId);
        //     return Ok(cars);
        // }

        // [HttpDelete("cars/{id}")]
        // public async Task<IActionResult> DeleteCar(long id)
        // {
        //     var userId = GetUserId();
        //     await _carService.DeleteCar(id, userId);
        //     return Ok("Car deleted successfully");
        // }

        // ORDERS

        [HttpPost("orders/wash-now")]
        public async Task<IActionResult> WashNow(WashNowRequest request)
        {
            var userId = GetUserId();
            var order = await _orderService.CreateWashNowAsync(userId, request);
            return Ok(order);
        }

        [HttpPost("orders/schedule")]
        public async Task<IActionResult> ScheduleWash(ScheduleWashRequest request)
        {
            var userId = GetUserId();
            var order = await _orderService.ScheduleWashAsync(userId, request);
            return Ok(order);
        }

        [HttpGet("orders/current")]
        public async Task<IActionResult> GetCurrentOrders()
        {
            var userId = GetUserId();
            var orders = await _orderService.GetCurrentOrdersAsync(userId);
            return Ok(orders);
        }

        [HttpGet("orders/past")]
        public async Task<IActionResult> GetPastOrders()
        {
            var userId = GetUserId();
            var orders = await _orderService.GetPastOrdersAsync(userId);
            return Ok(orders);
        }

        // [HttpDelete("orders/{id}")]
        // public async Task<IActionResult> CancelOrder(long id)
        // {
        //     await _orderService.CancelOrderAsync(id);
        //     return Ok("Order cancelled");
        // }

        // PAYMENTS

        [HttpPost("payments")]
        public async Task<IActionResult> Pay(ProcessPayment request)
        {
            var userId = GetUserId();
            var result = await _paymentService.ProcessPaymentAsync(request, userId);
            return Ok(result);
        }

        // PAYMENT METHODS

        [HttpPost("payment-methods")]
        public async Task<IActionResult> AddPaymentMethod(AddPaymentMethod dto)
        {
            var userId = GetUserId();
            var result = await _paymentMethodService.AddPaymentMethodAsync(userId, dto);
            return Ok(result);
        }

        // [HttpGet("payment-methods")]
        // public async Task<IActionResult> GetPaymentMethods()
        // {
        //     var userId = GetUserId();
        //     var result = await _paymentMethodService.GetCustomerPaymentMethodsAsync(userId);
        //     return Ok(result);
        // }

        //Invoice

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