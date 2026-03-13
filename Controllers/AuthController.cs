using Microsoft.AspNetCore.Mvc;
using GreenWash.DTO;
using GreenWash.Interfaces;

namespace GreenWash.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authservice;

        public AuthController(IAuthService authService)
        {
            _authservice = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            await _authservice.Register(request);
            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var token = await _authservice.Login(request);

            if (token == null)
                return Unauthorized();

            return Ok(token);
        }
    }
}