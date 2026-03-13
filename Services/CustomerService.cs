using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.Interfaces;
using GreenWash.DTO;
using GreenWash.Models;
using GreenWash.Exceptions;

namespace GreenWash.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerProfile> GetProfile(long userId)
        {
            var profile = await _customerRepository.GetByUserId(userId);

            if (profile == null)
                throw new NotFoundException("Customer profile not found");

            return profile;
        }

        public async Task UpdateProfile(long userId, UpdateCustomerProfileRequest request)
        {
            var profile = await _customerRepository.GetByUserId(userId);

            if (profile == null)
                throw new NotFoundException("Customer profile not found");

            profile.FirstName = request.FirstName;
            profile.LastName = request.LastName;
            profile.Phone = request.Phone;

            await _customerRepository.UpdateCustomerProfile(profile);
        }
    }
}