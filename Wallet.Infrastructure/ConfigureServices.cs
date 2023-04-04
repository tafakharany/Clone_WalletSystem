using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Application.Contracts;
using Wallet.Infrastructure.Persitstance;

namespace Wallet.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            //services.AddScoped<IPaymentInstitutionService, PaymentInstitutionService>();
            //services.AddScoped<IBudgetCategoryService, BudgetCategoryService>();

            //services.AddHttpClient();
            //services.AddScoped<IIdentityService, IdentityService>();

            //Add DBcontext using SQL server 
            var connectionString = configuration.GetConnectionString("Default");
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IApplicationDbContext>(
                provider => provider.GetRequiredService<ApplicationDbContext>());

            return services;
        }
    }
}
