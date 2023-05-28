using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Application.Dtos.Common;

namespace Wallet.Application.Dtos.Response;

public class ResponseDto
{
    public ResponseDto()
    {

    }
    public ResponseDto(string responseCode, string responseMessage)
    {
        ResponseCode = responseCode;
        ResponseMessage = responseMessage;
    }
    public ResponseDto(string responseMessage)
    {
        ResponseCode = ResponseCodes.ProcessedSuccessfully;
        ResponseMessage = responseMessage;
    }

    public string ResponseCode { get; set; }
    public string ResponseMessage { get; set; }
}
