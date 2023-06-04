using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Wallet.Application.Contracts;
using Wallet.Application.Dtos.Common;
using Wallet.Application.Dtos.Requests;
using Wallet.Application.Dtos.Response;
using Wallet.Domain.Entities;
using Wallet.Infrastructure.Persitstance;
using Wallet.Resources;

namespace Wallet.Infrastructure.Services;

public class IdentityService : BaseService<IdentityService>, IIdentityService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IWalletManageService _walletManageService;
    
    public IdentityService(UserManager<User> userManager,
        SignInManager<User> signInManager,
        IMapper mapper,
        ILogger<IdentityService> logger,
        IConfiguration configuration, ApplicationDbContext context, 
        IWalletManageService walletManageService) : base(mapper, logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _context = context;
        _walletManageService = walletManageService;
    }

    public async Task<bool> IsMobileRegistered(string mobileNumber)
    {
        var result = false;
        try
        {
            var isMobileExists =await _context.Users.FirstOrDefaultAsync(x => x.MobileNumber.Equals(mobileNumber));
            if (isMobileExists == null)
            {
                return result;
            }
            result = true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message, ex.StackTrace);
        }
        return result;
    }

    public async Task<LoginResponseDto> Login(LoginRequestDto request)
    {
        LoginResponseDto response = new()
        {
            ResponseCode = ResponseCodes.FailedToProcess,
            ResponseMessage = Resource.Failed
        };
        try
        {
            User user = await _userManager.FindByNameAsync(request.MobileNumber);

            if (user == null)
            {
                response.ResponseMessage = string.Format(Resource.NotFound,"user") ;
                response.ResponseCode = ResponseCodes.NotFound;
                return response;
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded)
            {
                response.ResponseMessage = Resource.WrongData;
                response.ResponseCode = ResponseCodes.FailedToProcess;
                serializedResponse = JsonConvert.SerializeObject(response);
                Logger.LogError(serializedResponse);
            }
            else
            {
                var token = GenerateToken(request);
                response.ExpiresIn = 3600;
                response.AccessToken = token;
                response.ResponseCode = ResponseCodes.ProcessedSuccessfully;
                response.ResponseMessage = Resource.Sucess;
            }
        }
        catch (Exception ex)
        {

            Logger.LogError(ex.Message, ex.StackTrace);
            response.ResponseMessage = Resource.GeneralError;
            response.ResponseCode = ResponseCodes.GeneralError;
            serializedResponse = JsonConvert.SerializeObject(response);
            Logger.LogError(serializedResponse);
        }
        return response;
    }

    public async Task<RegisterResponseDto> RegisterUser(RegisterRequestDto request)
    {
        RegisterResponseDto response = new ()
        {
            ResponseMessage = Resource.Failed,
            ResponseCode = ResponseCodes.FailedToProcess
        };
        
        try
        {
            using var transaction = _context.Database.BeginTransaction();
            
            var isAlreadyUserWithSameMobileNumber = await IsMobileRegistered(request.MobileNumber);
            if (!isAlreadyUserWithSameMobileNumber)
            {
                var user = Mapper.Map<User>(request);
                var registered = await _userManager.CreateAsync(user, request.Password);

                if (!registered.Succeeded)
                {
                    await transaction.RollbackAsync();
                    response.ResponseMessage ="Error While Registring the user" + string.Join(',', SetErrorMessage(registered.Errors));
                    serializedResponse = JsonConvert.SerializeObject(response);
                    Logger.LogError(serializedResponse);
                    return response;
                }
                var cashInResponse = await _walletManageService.CashIn(request.MobileNumber);
                if(!cashInResponse.ResponseCode.Equals(ResponseCodes.ProcessedSuccessfully))
                {
                    await transaction.RollbackAsync();
                    response.ResponseMessage = cashInResponse.ResponseMessage;
                    response.ResponseCode = cashInResponse.ResponseCode;
                    serializedResponse = JsonConvert.SerializeObject(cashInResponse);
                    Logger.LogError(serializedResponse);
                    return response;
                }

                response.ResponseMessage = Resource.Sucess;
                response.ResponseCode = ResponseCodes.ProcessedSuccessfully;
                await transaction.CommitAsync();
            }
            else
            {
                response.ResponseMessage = Resource.DuplicateMobileNumber;
                response.ResponseCode = ResponseCodes.DuplicatedNumber;
                serializedResponse = JsonConvert.SerializeObject(response);
                Logger.LogError(serializedResponse);
            }

        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message, ex.StackTrace);
            response.ResponseMessage = Resource.GeneralError;
            response.ResponseCode = ResponseCodes.GeneralError;
            serializedResponse = JsonConvert.SerializeObject(response);
            Logger.LogError(serializedResponse);
        }

        return response;
    }

    private IEnumerable<string> SetErrorMessage(IEnumerable<IdentityError> errors)
    {
        foreach (var error in errors)
            yield return error.Description;
    }

    private string GenerateToken(LoginRequestDto request)
    {
        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.MobilePhone,request.MobileNumber)
        };
        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
