using GreenWash.DTO;
using GreenWash.Exceptions;
using GreenWash.Interfaces;
using GreenWash.Models;

namespace GreenWash.Services
{
    public class CarService : ICarService
    {
        private readonly ICarRepository      _carRepository;
        private readonly ICustomerRepository _customerRepository;

        public CarService(ICarRepository carRepository, ICustomerRepository customerRepository)
        {
            _carRepository      = carRepository;
            _customerRepository = customerRepository;
        }

        public async Task AddCar(long userId, CreateCarRequest request)
        {
            var customer = await _customerRepository.GetByUserId(userId)
                ?? throw new NotFoundException("Customer profile not found");

            var car = new Car
            {
                CustomerId  = customer.CustomerId,
                Make        = request.Make,
                Model       = request.Model,
                Color       = request.Color,
                PlateNumber = request.PlateNumber,
                ImageUrl    = request.ImageUrl ?? string.Empty,
                IsActive    = true,
                CreatedAt   = DateTime.UtcNow
            };

            await _carRepository.AddCar(car);
        }

        public async Task<List<Car>> GetMyCars(long userId)
        {
            var customer = await _customerRepository.GetByUserId(userId)
                ?? throw new NotFoundException("Customer profile not found");

            var cars = await _carRepository.GetCarsByCustomerId(customer.CustomerId);
            return cars.Where(c => c.IsActive).ToList();   // only return active cars
        }

        public async Task DeleteCar(long carId, long userId)
        {
            var customer = await _customerRepository.GetByUserId(userId)
                ?? throw new NotFoundException("Customer profile not found");

            var car = await _carRepository.GetCarById(carId)
                ?? throw new NotFoundException("Car not found");

            // Ownership check — a customer can only delete their own car
            if (car.CustomerId != customer.CustomerId)
                throw new UnauthorizedException("This car does not belong to you");

            // Soft-delete: mark inactive rather than hard-delete
            // (preserves historical order data that references this car)
            car.IsActive = false;
            await _carRepository.UpdateCar(car);
        }
    }
}
