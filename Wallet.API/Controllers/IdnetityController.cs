using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Wallet.Application.Contracts;
using Wallet.Application.Dtos.Common;
using Wallet.Application.Dtos.Requests;
using Wallet.Application.Dtos.Response;
using Wallet.Resources;

namespace Wallet.API.Controllers;


public class IdentityController : APIBaseController<IdentityController>
{
    private readonly IValidator<RegisterRequestDto> _registerationValidator;
    private readonly IValidator<LoginRequestDto> _loginValidator;
    private readonly IIdentityService _service;
    public IdentityController(IValidator<LoginRequestDto> loginValidator,
        IValidator<RegisterRequestDto> registerationValidator,
        IIdentityService service)
    {
        _loginValidator = loginValidator;
        _registerationValidator = registerationValidator;
        _service = service;
    }

    [HttpPost("registerNewUser")]
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

        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message, ex.StackTrace);
            response.ResponseCode = ResponseCodes.GeneralError;
            response.ResponseMessage = Resource.GeneralError;
        }

        return Ok(response);
    }

    [HttpPost("login")]
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
