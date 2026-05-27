using Microsoft.Extensions.DependencyInjection;
using PlaniranjePutovanja.UtilService.Clients;
using PlaniranjePutovanja.UtilService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.UtilService.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUtilGenerators(this IServiceCollection services)
        {
            services.AddScoped<IPdfGeneratorService, PdfGeneratorService>();
            services.AddScoped<IQrCodeGeneratorService, QrCodeGeneratorService>();
            return services;
        }

        public static IServiceCollection AddUtilTokenServices(this IServiceCollection services, string jwtSecret, string jwtIssuer, string jwtAudience)
        {
            services.AddSingleton<IShareTokenService>(provider =>
                new ShareTokenService(jwtSecret, jwtIssuer, jwtAudience));
            return services;
        }

        public static IServiceCollection AddUtilClients(this IServiceCollection services)
        {
            services.AddSingleton<ITravelServiceClient, TravelServiceClient>();
            services.AddSingleton<IExpenseServiceClient, ExpenseServiceClient>();
            return services;
        }

        public static IServiceCollection AddUtilBusiness(this IServiceCollection services)
        {
            services.AddScoped<IUtilBusinessService, UtilBusinessService>();
            return services;
        }

        public static IServiceCollection AddUtilServices(this IServiceCollection services, string jwtSecret, string jwtIssuer, string jwtAudience)
        {
            services.AddUtilGenerators();
            services.AddUtilTokenServices(jwtSecret, jwtIssuer, jwtAudience);
            services.AddUtilClients();
            services.AddUtilBusiness();
            return services;
        }
    }
}
