using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlaniranjePutovanja.Common.DTOs.Expense;
using PlaniranjePutovanja.ExpenseService.Clients;
using PlaniranjePutovanja.ExpenseService.Mappers;
using PlaniranjePutovanja.ExpenseService.Persistence;
using PlaniranjePutovanja.ExpenseService.Repositories;
using PlaniranjePutovanja.ExpenseService.Services;
using PlaniranjePutovanja.ExpenseService.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.ExpenseService.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddExpensePersistence(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ExpenseDbContext>(options =>
                options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

            return services;
        }

        public static IServiceCollection AddExpenseRepositories(this IServiceCollection services)
        {
            services.AddScoped<IExpenseRepository, ExpenseRepository>();
            return services;
        }

        public static IServiceCollection AddExpenseMappers(this IServiceCollection services)
        {
            services.AddSingleton<IExpenseMapper, ExpenseMapper>();
            return services;
        }

        public static IServiceCollection AddExpenseServices(this IServiceCollection services)
        {
            services.AddScoped<IBudgetBusinessService, BudgetBusinessService>();
            services.AddScoped<IActivityExpenseService, ActivityExpenseService>();
            return services;
        }

        public static IServiceCollection AddExpenseValidators(this IServiceCollection services)
        {
            services.AddSingleton<IValidator<CreateExpenseDto>, CreateExpenseDtoValidator>();
            return services;
        }

        public static IServiceCollection AddTravelClientService(this IServiceCollection services)
        {
            services.AddSingleton<ITravelServiceClient, TravelServiceClient>();
            return services;
        }


    }
}
