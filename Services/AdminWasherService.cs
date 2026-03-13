using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.DTO;
using GreenWash.Interfaces;
using GreenWash.Models;
using GreenWash.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class AdminWasherService : IAdminWasherService
{
    private readonly IAdminWasherRepository _repository;

    public AdminWasherService(IAdminWasherRepository repository)
    {
        _repository = repository;
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

        return await _repository.AddWasherAsync(user, washer);
    }

    public async Task<WasherProfile> UpdateWasherAsync(long washerId, UpdateWasher dto)
    {
        var washer = await _repository.GetWasherByIdAsync(washerId);

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