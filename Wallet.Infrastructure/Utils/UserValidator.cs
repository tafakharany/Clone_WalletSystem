using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Infrastructure.Utils;

public class CustomUserValidator<T> : UserValidator<T> where T : class
{
    public override async Task<IdentityResult> ValidateAsync(UserManager<T> manager, T user)
    {
        var result = await base.ValidateAsync(manager, user);

        // Remove all DuplicateEmail errors
        var errors = result.Errors.Where(error => error.Code != "DuplicateEmail").ToArray();
        result = errors.Any() ? IdentityResult.Failed(errors) : IdentityResult.Success;

        return result;
    }
}
