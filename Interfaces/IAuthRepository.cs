using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.Models;

namespace GreenWash.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> GetUserByEmail(string email);
        Task AddUser(User user);
        Task AddCustomerProfile(CustomerProfile profile);
    }
}