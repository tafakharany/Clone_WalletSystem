using Microsoft.OpenApi.Models;
using Wallet.API.Filters;

namespace Wallet.API.ServiceExtenstions;

public static class SwaggerExtenstions
{
    public static IServiceCollection AddSwaggerWithAuthorization(this IServiceCollection services)
    {
        services.AddSwaggerGen(option =>
        {
            option.OperationFilter<ReApplyOptionalRouteParameterOperationFilter>();
            option.UseInlineDefinitionsForEnums();
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
        });
        return services;
    }
}
