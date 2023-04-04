using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Application.Dtos.Requests
{
    public class TransferRequestDto
    {
        public string SenderMobileNumber { get; set; }
        public string RecipientMobileNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
