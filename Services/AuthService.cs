using GreenWash.DTO;
using GreenWash.Interfaces;
using GreenWash.Models;
using GreenWash.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace GreenWash.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }

        public async Task<bool> Register(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new BadRequestException("Email is required");

            if (string.IsNullOrWhiteSpace(request.FirstName))
                throw new BadRequestException("First name is required");

            if (string.IsNullOrWhiteSpace(request.LastName))
                throw new BadRequestException("Last name is required");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new BadRequestException("Password is required");

            if (string.IsNullOrWhiteSpace(request.Phone))
                throw new BadRequestException("Phone number is required");

            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            if (!emailRegex.IsMatch(request.Email))
                throw new BadRequestException("Invalid email format");

            if (request.Password.Length < 8)
                throw new BadRequestException("Password must be at least 8 characters long");

            if (request.Phone.Length != 10)
                throw new BadRequestException("Phone number must be exactly 10 digits");

            var existingUser = await _authRepository.GetUserByEmail(request.Email);

            if (existingUser != null)
                throw new BadRequestException("User already exists");

            var user = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = UserRole.Customer,
                CreatedAt = DateTime.UtcNow
            };

            await _authRepository.AddUser(user);

            var profile = new CustomerProfile
            {
                UserId = user.UserId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone
            };

            await _authRepository.AddCustomerProfile(profile);

            return true;
        }

        public async Task<AuthResponse> Login(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new BadRequestException("Email is required");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new BadRequestException("Password is required");

            var user = await _authRepository.GetUserByEmail(request.Email);

            if (user == null)
                throw new NotFoundException("User not found");

            bool validPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!validPassword)
                throw new UnauthorizedException("Invalid password");

            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = token,
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}