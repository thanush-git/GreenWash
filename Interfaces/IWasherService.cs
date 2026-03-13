using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.DTO;
using GreenWash.Models;

namespace GreenWash.Interfaces
{
    public interface IWasherService
    {
        Task AcceptOrderAsync(int orderId, int washerId);

        Task DeclineOrderAsync(int orderId, int washerId);

        Task StartWashAsync(int orderId, int washerId);

        Task CompleteWashAsync(int orderId, int washerId);
        Task<List<Order>> GetAvailableOrdersAsync();

        Task<List<Order>> GetWasherOrdersAsync(int washerId);
    }
}