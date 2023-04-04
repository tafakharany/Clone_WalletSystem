using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Contracts;
using Wallet.Application.Dtos.Common;
using Wallet.Application.Dtos.Requests;
using Wallet.Application.Dtos.Response;
using Wallet.Application.Dtos.Response.Response;
using Wallet.Infrastructure.Services;
using Wallet.Resources;

namespace Wallet.Controllers
{
    public class IdnetityController : APIBaseController<IdnetityController>
    {
        private readonly IValidator<RegisterRequestDto> _registerationValidator;
        private readonly IValidator<LoginRequestDto> _loginValidator;
        private readonly IIdentityService _service;
        private readonly IWalletManageService _walletServices;
        public IdnetityController(IValidator<LoginRequestDto> loginValidator,
            IValidator<RegisterRequestDto> registerationValidator,
            IIdentityService service,
            IWalletManageService walletService)
        {
            _loginValidator = loginValidator;
            _registerationValidator = registerationValidator;
            _service = service;
            _walletServices = walletService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<RegisterResponseDto>> SignUp([FromBody] RegisterRequestDto request)
        {
            RegisterResponseDto response = new()
            {
                ResponseMessage = Resource.Failed,
                ResponseCode = ResponseCodes.FailedToProcess
            };

            try
            {
                var validationResult = _registerationValidator.Validate(request);
                if (!validationResult.IsValid)
                {
                    response.ResponseCode = ResponseCodes.ValidationError;
                    response.ResponseMessage = string.Join(",", SetErrorMessage(validationResult.Errors));
                    return BadRequest(response);
                }

                response = await _service.RegisterUser(request);
                if (response.ResponseCode.Equals(ResponseCodes.ProcessedSuccessfully))
                {
                    var result = await _walletServices.CashIn(request.MobileNumber);
                    response.ResponseMessage = result.ResponseMessage;
                    response.ResponseCode = result.ResponseCode;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, ex.StackTrace);
                response.ResponseCode = ResponseCodes.GeneralError;
                response.ResponseMessage = Resource.GeneralError;
            }

            return Ok(response);
        } 
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<RegisterResponseDto>> SignIn([FromBody] LoginRequestDto request)
        {
            LoginResponseDto response = new()
            {
                ResponseMessage = Resource.Failed,
                ResponseCode = ResponseCodes.FailedToProcess
            };

            try
            {
                var validationResult = _loginValidator.Validate(request);
                if (!validationResult.IsValid)
                {
                    response.ResponseCode = ResponseCodes.ValidationError;
                    response.ResponseMessage = string.Join(",", SetErrorMessage(validationResult.Errors));
                    return BadRequest(response);
                }

                response = await _service.Login(request);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message, ex.StackTrace);
                response.ResponseCode = ResponseCodes.GeneralError;
                response.ResponseMessage = Resource.GeneralError;
            }

            return Ok(response);
        }

    }
}
