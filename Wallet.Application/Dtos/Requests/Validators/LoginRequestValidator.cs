using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Application.Utils;
using Wallet.Resources;

namespace Wallet.Application.Dtos.Requests.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {

        RuleFor(x => x.MobileNumber)
            .MinimumLength(11).WithMessage(x => $"{{PropertyName}}: {Resource.MinLengthIs11}")
            .MaximumLength(15).WithMessage(x => $"{{PropertyName}}: {Resource.MaxLengthIs15}")
            .Matches(RegularExpressions.MobileNumber).WithMessage(x => $"{{PropertyName}}: {Resource.InvalidMobileNumber}");
    }
}

