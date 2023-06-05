using AutoMapper;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Wallet.API.Controllers;

[ApiController]
[Route("[controller]")]
public class APIBaseController<T> : ControllerBase
{
    private IMapper _mapper = null!;
    private ILogger<T> _logger = null!;
    protected IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetRequiredService<IMapper>();
    protected ILogger<T> Logger => _logger ??= HttpContext.RequestServices.GetRequiredService<ILogger<T>>();

    protected IEnumerable<string> SetErrorMessage(List<ValidationFailure> errors)
    {
        foreach (var error in errors)
            yield return error.ErrorMessage;
    }
}
