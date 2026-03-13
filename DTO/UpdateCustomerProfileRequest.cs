using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenWash.DTO
{
    public class UpdateCustomerProfileRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
    }
}