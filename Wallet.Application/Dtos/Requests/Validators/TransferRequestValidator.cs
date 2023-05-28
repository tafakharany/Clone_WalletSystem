using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Application.Utils;
using Wallet.Resources;

namespace Wallet.Application.Dtos.Requests.Validators;

public class TransferRequestValidator :AbstractValidator<TransferRequestDto>
{
    public TransferRequestValidator()
    {
        RuleFor(x=>x.Amount).NotNull().GreaterThanOrEqualTo(5).LessThanOrEqualTo(50000);

        RuleFor(x=>x.SenderMobileNumber).MinimumLength(11).WithMessage(x => $"{{PropertyName}}: {Resource.MinLengthIs11}")
            .MaximumLength(15).WithMessage(x => $"{{PropertyName}}: {Resource.MaxLengthIs15}")
            .Matches(RegularExpressions.MobileNumber).WithMessage(x => $"{{PropertyName}}: {Resource.InvalidMobileNumber}"); 
        
        RuleFor(x=>x.RecipientMobileNumber).MinimumLength(11).WithMessage(x => $"{{PropertyName}}: {Resource.MinLengthIs11}")
            .MaximumLength(15).WithMessage(x => $"{{PropertyName}}: {Resource.MaxLengthIs15}")
            .Matches(RegularExpressions.MobileNumber).WithMessage(x => $"{{PropertyName}}: {Resource.InvalidMobileNumber}");
    }
}
