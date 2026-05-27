using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlaniranjePutovanja.Common.DTOs.Travel;
using PlaniranjePutovanja.TravelService.Mappers;
using PlaniranjePutovanja.TravelService.Persistence;
using PlaniranjePutovanja.TravelService.Repositories;
using PlaniranjePutovanja.TravelService.Services;
using PlaniranjePutovanja.TravelService.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTravelPersistence(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<TravelDbContext>(options =>
                options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

            return services;
        }

        public static IServiceCollection AddTravelRepositories(this IServiceCollection services)
        {
            services.AddScoped<ITravelRepository, TravelRepository>();
            services.AddScoped<IDestinationRepository, DestinationRepository>();
            services.AddScoped<IActivityRepository, ActivityRepository>();
            services.AddScoped<IChecklistRepository, ChecklistRepository>();
            return services;
        }

        public static IServiceCollection AddTravelMappers(this IServiceCollection services)
        {
            services.AddSingleton<ITravelMapper, TravelMapper>();
            return services;
        }

        public static IServiceCollection AddTravelServices(this IServiceCollection services)
        {
            services.AddScoped<ITravelBusinessService, TravelBusinessService>();
            services.AddScoped<IDestinationBusinessService, DestinationBusinessService>();
            services.AddScoped<IActivityBusinessService, ActivityBusinessService>();
            services.AddScoped<IChecklistBusinessService, ChecklistBusinessService>();
            return services;
        }

        public static IServiceCollection AddTravelValidators(this IServiceCollection services)
        {
            services.AddSingleton<IValidator<CreateTravelDto>, CreateTravelDtoValidator>();
            services.AddSingleton<IValidator<CreateDestinationDto>, CreateDestinationDtoValidator>();
            services.AddSingleton<IValidator<CreateActivityDto>, CreateActivityDtoValidator>();
            services.AddSingleton<IValidator<CreateChecklistDto>, CreateChecklistDtoValidator>();
            return services;
        }
    }
}
