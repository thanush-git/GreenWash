using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GreenWash.Data;
using GreenWash.Models;
using GreenWash.Interfaces;

namespace GreenWash.DAL
{
    public class AuthRepository : IAuthRepository
    {
        private readonly GreenWashDbContext _context;

        public AuthRepository(GreenWashDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddUser(User user)
        {
            _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        
        public async Task AddCustomerProfile(CustomerProfile profile)
        {
            await _context.CustomerProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();
        }
    }
}