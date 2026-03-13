using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.Models;

namespace GreenWash.Interfaces
{
    public interface IAdminWasherRepository
    {
        Task<WasherProfile> AddWasherAsync(User user, WasherProfile washer);
        Task<WasherProfile> UpdateWasherAsync(WasherProfile washer);
        Task<WasherProfile> GetWasherByIdAsync(long washerId);
        Task UpdateStatusAsync(long washerId, bool isActive);
    }
}