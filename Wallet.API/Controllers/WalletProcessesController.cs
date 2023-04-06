using Azure;
using Azure.Core;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Wallet.Application.Contracts;
using Wallet.Application.Dtos.Common;
using Wallet.Application.Dtos.Requests;
using Wallet.Application.Dtos.Response;
using Wallet.Resources;

namespace Wallet.Controllers
{

    public class WalletProcessesController : APIBaseController<WalletProcessesController>
    {
        private readonly IValidator<TransferRequestDto> _validator;
        private readonly IWalletManageService _service;

        public WalletProcessesController(IValidator<TransferRequestDto> validator, IWalletManageService service)
        {
            _validator = validator;
            _service = service;
        }
        [Authorize]
        [HttpPost("SendMoney")]
        public async Task<ActionResult<ResponseDto>> TransferAmountToOtherWalletAccount([FromBody] TransferRequestDto request)
        {
            ResponseDto response = new()
            {
                ResponseCode = ResponseCodes.FailedToProcess,
                ResponseMessage = Resource.Failed
            };
            try
            {
                var user = HttpContext.User as ClaimsPrincipal;
                var sender = user.FindFirstValue(ClaimTypes.MobilePhone);
                if (sender != null)
                {
                    request.SenderMobileNumber = sender;
                }

                var validationResult = _validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    response.ResponseCode = ResponseCodes.ValidationError;
                    response.ResponseMessage = string.Join(",", SetErrorMessage(validationResult.Errors));
                    return BadRequest(response);
                }

                response = await _service.TransferToOthers(request);

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
