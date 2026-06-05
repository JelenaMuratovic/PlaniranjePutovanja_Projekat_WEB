using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.Extensions.DependencyInjection;
using PlaniranjePutovanja.UtilService.DependencyInjection;
using QuestPDF.Infrastructure;

namespace PlaniranjePutovanja.UtilService
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
                QuestPDF.Settings.License = LicenseType.Community;

                ServiceRuntime.RegisterServiceAsync("PlaniranjePutovanja.UtilServiceType",
                    context => 
                    {
                        var configPackage = context.CodePackageActivationContext
                            .GetConfigurationPackageObject("Config");
                        var settings = configPackage.Settings;

                        // JWT konfiguracija za share tokene
                        var jwtSecretKey = settings.Sections["SecurityConfiguration"]
                            .Parameters["JwtSecretKey"].Value;
                        var jwtIssuer = settings.Sections["SecurityConfiguration"]
                            .Parameters["JwtIssuer"].Value;
                        var jwtAudience = settings.Sections["SecurityConfiguration"]
                            .Parameters["JwtAudience"].Value;

                        IServiceCollection services = new ServiceCollection();

                        // Registracije servisa i klijenata
                        services.AddUtilServices(jwtSecretKey, jwtIssuer, jwtAudience);

                        IServiceProvider serviceProvider = services.BuildServiceProvider();

                        return new UtilService(context, serviceProvider);
                    }).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(UtilService).Name);

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
