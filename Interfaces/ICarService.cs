using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.DTO;
using GreenWash.Models;

namespace GreenWash.Interfaces
{
    public interface ICarService
    {
        Task AddCar(long userId, CreateCarRequest request);

        Task<List<Car>> GetMyCars(long userId);

        Task DeleteCar(long carId, long userId);
    }
}