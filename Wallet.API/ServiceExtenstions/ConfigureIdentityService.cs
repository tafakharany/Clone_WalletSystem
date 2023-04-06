using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Wallet.Domain.Entities;
using Wallet.Infrastructure.Persitstance;
using Wallet.Infrastructure.Utils;

namespace Wallet.ServiceExtenstions
{
    public static class JWtExtension
    {
        public static IServiceCollection AddJWtService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                        SaveSigninToken = true,
                    });

            return services;
        }
    }
    public static class IdentityServicesExtenstion
    {
        public static IServiceCollection AddIdentityService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<User, ApplicationRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddUserValidator<CustomUserValidator<User>>()
                    .AddDefaultTokenProviders();
            services.AddScoped<UserManager<User>>();
            services.AddAuthorization();
            return services;
        }
    }
}
