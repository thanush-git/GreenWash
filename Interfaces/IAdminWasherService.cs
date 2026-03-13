using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.DTO;
using GreenWash.Models;

namespace GreenWash.Interfaces
{
    public interface IAdminWasherService
    {
        Task<WasherProfile> AddWasherAsync(CreateWasher dto);
        Task<WasherProfile> UpdateWasherAsync(long washerId, UpdateWasher dto);
        Task ToggleWasherStatusAsync(long washerId, bool isActive);
    }
}