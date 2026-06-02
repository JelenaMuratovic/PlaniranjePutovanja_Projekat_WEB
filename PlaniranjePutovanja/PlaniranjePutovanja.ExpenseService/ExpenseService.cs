using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using PlaniranjePutovanja.Common.DTOs.Expense;
using PlaniranjePutovanja.Common.Interfaces.Expense;
using PlaniranjePutovanja.Common.Interfaces.Travel;
using PlaniranjePutovanja.ExpenseService.Clients;
using PlaniranjePutovanja.ExpenseService.DependencyInjection;
using PlaniranjePutovanja.ExpenseService.Services;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.ExpenseService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class ExpenseService : StatefulService, IExpenseService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        //public ExpenseService(StatefulServiceContext context, IServiceProvider serviceProvider)
        //    : base(context)
        //{
        //    _scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        //}
        public ExpenseService(StatefulServiceContext context, string connectionString)
            : base(context)
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<Microsoft.ServiceFabric.Data.IReliableStateManager>(this.StateManager);

            services.AddExpensePersistence(connectionString);
            services.AddExpenseRepositories();
            services.AddExpenseMappers();
            services.AddExpenseValidators();
            services.AddTravelClientService();
            services.AddExpenseServices();

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            _scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        public async Task<ExpenseDto> AddExpenseAsync(string travelId, CreateExpenseDto request)
        {
            using var scope = _scopeFactory.CreateScope();
            var budgetService = scope.ServiceProvider.GetRequiredService<IBudgetBusinessService>();
            return await budgetService.AddExpenseAsync(travelId, request, CancellationToken.None);
        }

        public async Task<IEnumerable<ExpenseDto>> GetExpensesByTravelIdAsync(string travelId)
        {
            using var scope = _scopeFactory.CreateScope();
            var budgetService = scope.ServiceProvider.GetRequiredService<IBudgetBusinessService>();
            return await budgetService.GetExpensesByTravelIdAsync(travelId, CancellationToken.None);
        }

        public async Task<TravelBudgetSummaryDto?> GetBudgetSummaryAsync(string travelId)
        {
            using var scope = _scopeFactory.CreateScope();
            var budgetService = scope.ServiceProvider.GetRequiredService<IBudgetBusinessService>();
            return await budgetService.GetBudgetSummaryAsync(travelId, CancellationToken.None);
        }

        public async Task<bool> DeleteExpenseAsync(string travelId, string expenseId)
        {
            using var scope = _scopeFactory.CreateScope();
            var budgetService = scope.ServiceProvider.GetRequiredService<IBudgetBusinessService>();
            return await budgetService.DeleteExpenseAsync(travelId, expenseId, CancellationToken.None);
        }

        public async Task<bool> DeleteExpensesByTravelIdAsync(string travelId)
        {
            using var scope = _scopeFactory.CreateScope();
            var budgetService = scope.ServiceProvider.GetRequiredService<IBudgetBusinessService>();
            return await budgetService.DeleteExpensesByTravelIdAsync(travelId, CancellationToken.None);
        }

        public async Task<string> CreateActivityExpenseAsync(
        string travelId,
        string activityId,
        string activityName,
        decimal price,
        decimal plannedBudget)
        {
            using var scope = _scopeFactory.CreateScope();
            var activityExpenseService = scope.ServiceProvider.GetRequiredService<IActivityExpenseService>();
            return await activityExpenseService.CreateActivityExpenseAsync(travelId, activityId, activityName, price, plannedBudget, CancellationToken.None);
        }

        public async Task<bool> UpdateActivityExpenseAsync(
            string travelId,
            string activityId,
            decimal newPrice)
        {
            using var scope = _scopeFactory.CreateScope();
            var activityExpenseService = scope.ServiceProvider.GetRequiredService<IActivityExpenseService>();
            return await activityExpenseService.UpdateActivityExpenseAsync(travelId, activityId, newPrice, CancellationToken.None);
        }

        public async Task<bool> DeleteActivityExpenseAsync(
            string travelId,
            string activityId)
        {
            using var scope = _scopeFactory.CreateScope();
            var activityExpenseService = scope.ServiceProvider.GetRequiredService<IActivityExpenseService>();
            return await activityExpenseService.DeleteActivityExpenseAsync(travelId, activityId, CancellationToken.None);
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // Inicijalizacija
            using (var initScope = _scopeFactory.CreateScope())
            {
                var budgetService = initScope.ServiceProvider.GetRequiredService<IBudgetBusinessService>();
                var travelClient = initScope.ServiceProvider.GetRequiredService<ITravelServiceClient>();

                try
                {
                    var allTravels = await travelClient.GetAllTravelsAsync();

                    await budgetService.InitializeAsync(allTravels);

                    ServiceEventSource.Current.Message("ExpenseService: Budgets initialized successfully.");
                }
                catch (Exception ex)
                {
                    ServiceEventSource.Current.Message($"ExpenseService: Initialization failed: {ex.Message}");
                }
            }

            await Task.Delay(Timeout.Infinite, cancellationToken);

        }
    }
}
