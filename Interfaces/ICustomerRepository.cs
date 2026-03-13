using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.Models;

namespace GreenWash.Interfaces
{
    public interface ICustomerRepository
    {
        Task<CustomerProfile?> GetByUserId(long userId);
        Task UpdateCustomerProfile(CustomerProfile profile);
    }
}