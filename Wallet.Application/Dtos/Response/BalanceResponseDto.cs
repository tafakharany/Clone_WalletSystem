using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Application.Dtos.Response
{
    public class BalanceResponseDto : ResponseDto
    {
          public decimal Balance { get; set; }
    }
}
