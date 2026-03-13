using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GreenWash.DTO;

namespace GreenWash.Interfaces
{
    public interface IPaymentService
    {
        Task<string> ProcessPaymentAsync(ProcessPayment request, long userId);
    }
}