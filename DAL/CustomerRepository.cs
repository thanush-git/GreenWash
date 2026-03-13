using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.Models;
using GreenWash.Data;
using Microsoft.EntityFrameworkCore;
using GreenWash.Interfaces;

namespace GreenWash.DAL
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly GreenWashDbContext _context;

        public CustomerRepository(GreenWashDbContext context)
        {
            _context = context;
        }

        public async Task<CustomerProfile?> GetByUserId(long userId)
        {
            return await _context.CustomerProfiles
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task UpdateCustomerProfile(CustomerProfile profile)
        {
            _context.CustomerProfiles.Update(profile);
            await _context.SaveChangesAsync();
        }
    }
}