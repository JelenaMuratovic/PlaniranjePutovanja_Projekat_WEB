using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Runtime;
using PlaniranjePutovanja.ExpenseService.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.ExpenseService
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

                ServiceRuntime.RegisterServiceAsync("PlaniranjePutovanja.ExpenseServiceType",
                    context => {
                        var configPackage = context.CodePackageActivationContext
                            .GetConfigurationPackageObject("Config");
                        var settings = configPackage.Settings;

                        var connectionString = settings.Sections["DatabaseConfiguration"]
                            .Parameters["ConnectionString"].Value;

                        //IServiceCollection services = new ServiceCollection();

                        //services.AddExpensePersistence(connectionString);
                        //services.AddExpenseRepositories();
                        //services.AddExpenseMappers();
                        //services.AddExpenseValidators();
                        //services.AddTravelClientService();
                        //services.AddExpenseServices();

                        //IServiceProvider serviceProvider = services.BuildServiceProvider();

                        //return new ExpenseService(context, serviceProvider);
                        // Task.FromResult<StatefulService>(new ExpenseService(context, connectionString));

                        // Prevarimo kompajler da je ovo sigurno StatefulContext
                        var statefulContext = (System.Fabric.StatefulServiceContext)context;

                        // Vracamo direktno instancu, bez Task.FromResult!
                        return new ExpenseService(statefulContext, connectionString);
                    }).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(ExpenseService).Name);

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
