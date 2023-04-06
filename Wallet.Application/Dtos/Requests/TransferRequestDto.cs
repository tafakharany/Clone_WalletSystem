using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Wallet.Application.Dtos.Requests
{
    public class TransferRequestDto
    {
        [JsonIgnore]
        public string? SenderMobileNumber { get; set; }
        public string RecipientMobileNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
