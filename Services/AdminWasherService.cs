using GreenWash.DTO;
using GreenWash.Interfaces;
using GreenWash.Models;
using GreenWash.Exceptions;
namespace GreenWash.Services
{
    public class AdminWasherService : IAdminWasherService
    {
        private readonly IAdminWasherRepository _repository;
        private readonly IEmailService _email;

        public AdminWasherService(IAdminWasherRepository repository, IEmailService email)
        {
            _repository = repository;
            _email = email;
        }

        public async Task<WasherProfile> AddWasherAsync(CreateWasher dto)
        {
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.Washer,
                IsActive = true
            };

            var washer = new WasherProfile
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Phone = dto.Phone
            };

            var created = await _repository.AddWasherAsync(user, washer);

            // ── Washer welcome email with login credentials ────────────────────────
            var (subject, html) = EmailTemplates.WasherWelcome(dto.FirstName, dto.Email, dto.Password);
            await _email.SendAsync(dto.Email, dto.FirstName, subject, html);

            return created;
        }

        public async Task<WasherProfile> UpdateWasherAsync(long washerId, UpdateWasher dto)
        {
            var washer = await _repository.GetWasherByIdAsync(washerId)
                ?? throw new NotFoundException("Washer not found");

            washer.FirstName = dto.FirstName;
            washer.LastName = dto.LastName;
            washer.Phone = dto.Phone;

            return await _repository.UpdateWasherAsync(washer);
        }

        public async Task ToggleWasherStatusAsync(long washerId, bool isActive)
        {
            await _repository.UpdateStatusAsync(washerId, isActive);
        }
    }
}