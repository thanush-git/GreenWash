using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenWash.DTO;

namespace GreenWash.Interfaces
{
    public interface IAuthService
    {
        Task<bool> Register(RegisterRequest request);
        Task<AuthResponse> Login(LoginRequest request);
    }
}