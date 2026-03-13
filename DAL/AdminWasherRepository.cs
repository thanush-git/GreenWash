using GreenWash.DAL;
using GreenWash.Interfaces;
using GreenWash.Models;
using Microsoft.EntityFrameworkCore;
using GreenWash.Data;

namespace GreenWash.DAL
{
    public class AdminWasherRepository : IAdminWasherRepository
    {
        private readonly GreenWashDbContext _context;

        public AdminWasherRepository(GreenWashDbContext context)
        {
            _context = context;
        }

        public async Task<WasherProfile> AddWasherAsync(User user, WasherProfile washer)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Add user first
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Link washer profile to user
                washer.WasherId = user.UserId;

                _context.WasherProfiles.Add(washer);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return washer;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<WasherProfile> UpdateWasherAsync(WasherProfile washer)
        {
            _context.WasherProfiles.Update(washer);
            await _context.SaveChangesAsync();
            return washer;
        }

        public async Task<WasherProfile> GetWasherByIdAsync(long washerId)
        {
            return await _context.WasherProfiles
                .FirstOrDefaultAsync(w => w.WasherId == washerId);
        }

        public async Task UpdateStatusAsync(long washerId, bool isActive)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == washerId);

            if (user == null)
                throw new Exception("Washer not found");

            user.IsActive = isActive;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}