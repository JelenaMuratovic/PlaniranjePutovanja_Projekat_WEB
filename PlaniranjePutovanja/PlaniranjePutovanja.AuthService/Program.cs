using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Runtime;
using PlaniranjePutovanja.AuthService.DependencyInjection;
using PlaniranjePutovanja.AuthService.Persistence;
using PlaniranjePutovanja.AuthService.Services;
using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.AuthService
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                ServiceRuntime.RegisterServiceAsync("PlaniranjePutovanja.AuthServiceType",
                    context => {
                        var configPackage = context.CodePackageActivationContext
                        .GetConfigurationPackageObject("Config");
                        var settings = configPackage.Settings;

                        // Database konfiguracija
                        var connectionString = settings.Sections["DatabaseConfiguration"]
                            .Parameters["ConnectionString"].Value;

                        // Passwords konfiguracija
                        var passwordHashingIterations = int.Parse(
                            settings.Sections["SecurityConfiguration"]
                            .Parameters["PasswordHashingIterations"].Value);

                        var passwordSaltSize = int.Parse(
                            settings.Sections["SecurityConfiguration"]
                            .Parameters["PasswordSaltSize"].Value);

                        var passwordHashSize = int.Parse(
                            settings.Sections["SecurityConfiguration"]
                            .Parameters["PasswordHashSize"].Value);

                        // JWT konfiguracija
                        var jwtSecretKey = settings.Sections["SecurityConfiguration"]
                            .Parameters["JwtSecretKey"].Value;

                        var jwtIssuer = settings.Sections["SecurityConfiguration"]
                            .Parameters["JwtIssuer"].Value;

                        var jwtAudience = settings.Sections["SecurityConfiguration"]
                            .Parameters["JwtAudience"].Value;

                        var jwtExpirationMinutes = int.Parse(
                            settings.Sections["SecurityConfiguration"]
                            .Parameters["JwtExpirationMinutes"].Value);

                        IServiceCollection services = new ServiceCollection();
                        services.AddAuthPersistence(connectionString);
                        services.AddAuthRepositories();
                        services.AddAuthMappings();
                        services.AddAuthPasswords(passwordHashingIterations, passwordSaltSize, passwordHashSize);
                        services.AddAuthJwt(jwtSecretKey, jwtIssuer, jwtAudience, jwtExpirationMinutes);
                        services.AddAuthValidators();
                        services.AddAuthServices();

                        IServiceProvider serviceProvider = services.BuildServiceProvider();

                        return new AuthService(context, serviceProvider);
                    }).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(AuthService).Name);

                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
