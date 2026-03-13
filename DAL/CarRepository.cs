using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.Data;
using GreenWash.Interfaces;
using GreenWash.Models;
using Microsoft.EntityFrameworkCore;

namespace GreenWash.DAL
{
    public class CarRepository : ICarRepository
    {
        private readonly GreenWashDbContext _context;

        public CarRepository(GreenWashDbContext context)
        {
            _context = context;
        }

        public async Task AddCar(Car car)
        {
            await _context.Cars.AddAsync(car);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Car>> GetCarsByCustomerId(long customerId)
        {
            return await _context.Cars
                .Where(c => c.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<Car?> GetCarById(long carId)
        {
            return await _context.Cars
                .FirstOrDefaultAsync(c => c.CarId == carId);
        }

        public async Task UpdateCar(Car car)
        {
            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCar(Car car)
        {
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
        }
    }
}