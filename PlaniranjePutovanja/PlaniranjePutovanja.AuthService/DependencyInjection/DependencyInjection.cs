using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlaniranjePutovanja.AuthService.Helpers.Passwords;
using PlaniranjePutovanja.AuthService.Helpers.Tokens;
using PlaniranjePutovanja.AuthService.Mappers;
using PlaniranjePutovanja.AuthService.Persistence;
using PlaniranjePutovanja.AuthService.Repositories;
using PlaniranjePutovanja.AuthService.Services;
using PlaniranjePutovanja.AuthService.Validators;
using PlaniranjePutovanja.Common.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.AuthService.DependencyInjection
{
    public static class DependencyInjection
    {
       public static IServiceCollection AddAuthPersistence(this IServiceCollection services, string connectionString)
       {
           services.AddDbContext<AppDbContext>(options =>
               options.UseSqlServer(connectionString));

           return services;
       }

       public static IServiceCollection AddAuthRepositories(this IServiceCollection services)
       {
           services.AddScoped<IUserRepository, UserRepository>();
           return services;
       }

       public static IServiceCollection AddAuthServices(this IServiceCollection services)
       {
           services.AddScoped<IAuthBusinessService, AuthBusinessService>();
           return services;
       }

       public static IServiceCollection AddAuthMappings(this IServiceCollection services)
       {
           services.AddSingleton<IAuthMapper, AuthMapper>();
           return services;
       }

       public static IServiceCollection AddAuthPasswords(
       this IServiceCollection services,
       int passwordHashingIterations = 100000,
       int passwordSaltSize = 16,
       int passwordHashSize = 32)
       {
           services.AddSingleton<IPasswordHasher>(provider =>
               new PasswordHasher(passwordHashingIterations, passwordSaltSize, passwordHashSize));

           return services;
       }

       public static IServiceCollection AddAuthJwt(
           this IServiceCollection services,
           string secretKey,
           string issuer,
           string audience,
           int expirationMinutes = 60)
       {
           services.AddSingleton<IJwtTokenService>(provider =>
               new JwtTokenService(secretKey, issuer, audience, expirationMinutes));

           return services;
       }

        public static IServiceCollection AddAuthValidators(this IServiceCollection services)
        {
            services.AddSingleton<IValidator<RegisterRequestDto>, RegisterRequestDtoValidator>();
            services.AddSingleton<IValidator<LoginRequestDto>, LoginRequestDtoValidator>();
            return services;
        }

        public static IServiceCollection AddAuthClients(this IServiceCollection services)
        {
            services.AddSingleton<Clients.ITravelServiceClient, Clients.TravelServiceClient>();
            services.AddSingleton<Clients.IExpenseServiceClient, Clients.ExpenseServiceClient>();
            return services;
        }
    }
}
