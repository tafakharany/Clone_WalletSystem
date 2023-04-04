using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Application.Dtos.Requests;
using Wallet.Application.Dtos.Response;

namespace Wallet.Application.Contracts
{
    public interface IWalletManageService
    {
        Task<ResponseDto> TransferToOthers(TransferRequestDto transferRequest);
        Task<ResponseDto> CashIn(string mobileNumber);
    }
}
