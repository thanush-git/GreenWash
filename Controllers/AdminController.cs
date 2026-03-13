using Microsoft.AspNetCore.Mvc;
using GreenWash.DTO;
using GreenWash.Interfaces;

namespace GreenWash.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminWasherService _service;

        public AdminController(IAdminWasherService service)
        {
            _service = service;
        }

        [HttpPost("washers")]
        public async Task<IActionResult> AddWasher(CreateWasher dto)
        {
            var washer = await _service.AddWasherAsync(dto);
            return Ok(washer);
        }

        [HttpPut("washers/{id}")]
        public async Task<IActionResult> UpdateWasher(long id, UpdateWasher dto)
        {
            var washer = await _service.UpdateWasherAsync(id, dto);
            return Ok(washer);
        }

        [HttpPatch("washers/{id}/status")]
        public async Task<IActionResult> ToggleStatus(long id, WasherStatus dto)
        {
            await _service.ToggleWasherStatusAsync(id, dto.IsActive);
            return Ok("Washer status updated");
        }
    }
}