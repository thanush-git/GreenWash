using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.Models;

namespace GreenWash.Interfaces
{
    public interface ICarRepository
    {
        Task AddCar(Car car);
        Task<List<Car>> GetCarsByCustomerId(long customerId);
        Task<Car?> GetCarById(long carId);
        Task UpdateCar(Car car);
        Task DeleteCar(Car car);
    }
}