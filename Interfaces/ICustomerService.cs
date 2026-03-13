using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.DTO;
using GreenWash.Models;

namespace GreenWash.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerProfile> GetProfile(long userId);

        Task UpdateProfile(long userId, UpdateCustomerProfileRequest request);
    }
}