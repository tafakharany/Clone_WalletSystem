using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Wallet.Filters
{
    public class ValidateAntiforgeryTokenAuthorizationFilter : IAuthorizationFilter
    {
        private readonly string _antiforgeryHeaderName;
        private readonly string _antiforgeryCookieName;

        public ValidateAntiforgeryTokenAuthorizationFilter()
        {
            _antiforgeryHeaderName = "X-CSRF-TOKEN"; // The name of the header that includes the antiforgery token.
            _antiforgeryCookieName = ".AspNetCore.Antiforgery"; // The name of the cookie that includes the antiforgery token.
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var antiforgery = context.HttpContext.RequestServices.GetRequiredService<IAntiforgery>();

            var headers = context.HttpContext.Request.Headers;
            var cookies = context.HttpContext.Request.Cookies;

            if (headers.ContainsKey(_antiforgeryHeaderName) && cookies.ContainsKey(_antiforgeryCookieName))
            {
                var headerToken = headers[_antiforgeryHeaderName];
                var cookieToken = cookies[_antiforgeryCookieName];

                antiforgery.ValidateRequestAsync(context.HttpContext).Wait();
            }
            else
            {
                context.Result = new StatusCodeResult(403);
            }
        }
    }
}
