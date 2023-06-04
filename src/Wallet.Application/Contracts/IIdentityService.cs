using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Application.Dtos.Requests;
using Wallet.Application.Dtos.Response;

namespace Wallet.Application.Contracts;

public interface IIdentityService
{
    Task<RegisterResponseDto> RegisterUser(RegisterRequestDto request);
    Task<LoginResponseDto> Login(LoginRequestDto request);
    Task<bool> IsMobileRegistered(string mobileNumber);
}
