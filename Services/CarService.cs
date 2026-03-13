using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.DTO;
using GreenWash.Interfaces;
using GreenWash.Models;
using GreenWash.Exceptions;

namespace GreenWash.Services
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository;
        private readonly ICustomerRepository _customerRepository;

        public CarService(ICarRepository carRepository,
                          ICustomerRepository customerRepository)
        {
            _carRepository = carRepository;
            _customerRepository = customerRepository;
        }

        public async Task AddCar(long userId, CreateCarRequest request)
        {
            var customer = await _customerRepository.GetByUserId(userId);

            if (customer == null)
                throw new NotFoundException("Customer profile not found");

            var car = new Car
            {
                CustomerId = customer.CustomerId,
                Make = request.Make,
                Model = request.Model,
                Color = request.Color,
                PlateNumber = request.PlateNumber,
                ImageUrl = request.ImageUrl
            };

            await _carRepository.AddCar(car);
        }

        public async Task<List<Car>> GetMyCars(long userId)
        {
            var customer = await _customerRepository.GetByUserId(userId);

            if (customer == null)
                throw new NotFoundException("Customer profile not found");

            return await _carRepository.GetCarsByCustomerId(customer.CustomerId);
        }

        public async Task DeleteCar(long carId, long userId)
        {
            var car = await _carRepository.GetCarById(carId);

            if (car == null)
                throw new NotFoundException("Car not found");

            await _carRepository.DeleteCar(car);
        }
    }
}