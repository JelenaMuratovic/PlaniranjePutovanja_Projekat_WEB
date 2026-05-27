using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Runtime;
using PlaniranjePutovanja.TravelService.DependencyInjection;
using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService
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

                ServiceRuntime.RegisterServiceAsync("PlaniranjePutovanja.TravelServiceType",
                    context => {
                        var configPackage = context.CodePackageActivationContext
                            .GetConfigurationPackageObject("Config");
                        var settings = configPackage.Settings;

                        // Database konfiguracija
                        var connectionString = settings.Sections["DatabaseConfiguration"]
                            .Parameters["ConnectionString"].Value;

                        IServiceCollection services = new ServiceCollection();
                        services.AddTravelPersistence(connectionString);
                        services.AddTravelRepositories();
                        services.AddTravelMappers();
                        services.AddTravelValidators();
                        services.AddTravelServices();

                        IServiceProvider serviceProvider = services.BuildServiceProvider();

                        return new TravelService(context, serviceProvider);
                    }).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(TravelService).Name);

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
