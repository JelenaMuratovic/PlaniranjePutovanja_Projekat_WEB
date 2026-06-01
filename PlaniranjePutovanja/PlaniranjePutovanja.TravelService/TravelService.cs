using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using PlaniranjePutovanja.Common.DTOs.Travel;
using PlaniranjePutovanja.Common.Interfaces.Travel;
using PlaniranjePutovanja.TravelService.Services;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class TravelService : StatelessService, ITravelService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public TravelService(StatelessServiceContext context, IServiceProvider serviceProvider)
            : base(context)
        {
            _scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        // Operacije vezane za putovanja
        public async Task<TravelDto> CreateTravelAsync(string userId, CreateTravelDto request)
        {
            using var scope = _scopeFactory.CreateScope();
            var travelService = scope.ServiceProvider.GetRequiredService<ITravelBusinessService>();
            return await travelService.CreateTravelAsync(userId, request, CancellationToken.None);
        }

        public async Task<TravelDto?> GetTravelByIdAsync(string id)
        {
            using var scope = _scopeFactory.CreateScope();
            var travelService = scope.ServiceProvider.GetRequiredService<ITravelBusinessService>();
            return await travelService.GetTravelByIdAsync(id, CancellationToken.None);
        }

        public async Task<IEnumerable<TravelDto>> GetTravelsByUserIdAsync(string userId)
        {
            using var scope = _scopeFactory.CreateScope();
            var travelService = scope.ServiceProvider.GetRequiredService<ITravelBusinessService>();
            return await travelService.GetTravelsByUserIdAsync(userId, CancellationToken.None);
        }
        public async Task<IEnumerable<TravelDto>> GetAllTravelsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var travelService = scope.ServiceProvider.GetRequiredService<ITravelBusinessService>();
            return await travelService.GetAllTravelsAsync(CancellationToken.None);
        }

        public async Task<bool> DeleteTravelAsync(string id)
        {
            using var scope = _scopeFactory.CreateScope();
            var travelService = scope.ServiceProvider.GetRequiredService<ITravelBusinessService>();
            return await travelService.DeleteTravelAsync(id, CancellationToken.None);
        }

        public async Task<TravelDto?> UpdateTravelAsync(string id, UpdateTravelDto request)
        {
            using var scope = _scopeFactory.CreateScope();
            var travelService = scope.ServiceProvider.GetRequiredService<ITravelBusinessService>();
            return await travelService.UpdateTravelAsync(id, request, CancellationToken.None);
        }

        // Operacije vezane za destinacije
        public async Task<DestinationDto> AddDestinationAsync(string travelId, CreateDestinationDto request)
        {
            using var scope = _scopeFactory.CreateScope();
            var destinationService = scope.ServiceProvider.GetRequiredService<IDestinationBusinessService>();
            return await destinationService.AddDestinationAsync(travelId, request, CancellationToken.None);
        }

        public async Task<DestinationDto?> GetDestinationByIdAsync(string travelId, string id)
        {
            using var scope = _scopeFactory.CreateScope();
            var destinationService = scope.ServiceProvider.GetRequiredService<IDestinationBusinessService>();
            return await destinationService.GetDestinationByIdAsync(travelId, id, CancellationToken.None);
        }

        public async Task<IEnumerable<DestinationDto>> GetDestinationsByTravelIdAsync(string travelId)
        {
            using var scope = _scopeFactory.CreateScope();
            var destinationService = scope.ServiceProvider.GetRequiredService<IDestinationBusinessService>();
            return await destinationService.GetDestinationsByTravelIdAsync(travelId, CancellationToken.None);
        }

        public async Task<bool> DeleteDestinationAsync(string travelId, string id)
        {
            using var scope = _scopeFactory.CreateScope();
            var destinationService = scope.ServiceProvider.GetRequiredService<IDestinationBusinessService>();
            return await destinationService.DeleteDestinationAsync(travelId, id, CancellationToken.None);
        }

        public async Task<DestinationDto?> UpdateDestinationAsync(string travelId, string id, UpdateDestinationDto request)
        {
            using var scope = _scopeFactory.CreateScope();
            var destinationService = scope.ServiceProvider.GetRequiredService<IDestinationBusinessService>();
            return await destinationService.UpdateDestinationAsync(travelId, id, request, CancellationToken.None);
        }

        // Operacije vezane za aktivnosti
        public async Task<ActivityDto> AddActivityAsync(string travelId, string destinationId, CreateActivityDto request)
        {
            using var scope = _scopeFactory.CreateScope();
            var activityService = scope.ServiceProvider.GetRequiredService<IActivityBusinessService>();
            return await activityService.AddActivityAsync(travelId, destinationId, request, CancellationToken.None);
        }

        public async Task<ActivityDto?> GetActivityByIdAsync(string travelId, string destinationId, string id)
        {
            using var scope = _scopeFactory.CreateScope();
            var activityService = scope.ServiceProvider.GetRequiredService<IActivityBusinessService>();
            return await activityService.GetActivityByIdAsync(travelId, destinationId, id, CancellationToken.None);
        }

        public async Task<IEnumerable<ActivityDto>> GetActivitiesByDestinationIdAsync(string travelId, string destinationId)
        {
            using var scope = _scopeFactory.CreateScope();
            var activityService = scope.ServiceProvider.GetRequiredService<IActivityBusinessService>();
            return await activityService.GetActivitiesByDestinationIdAsync(travelId, destinationId, CancellationToken.None);
        }

        public async Task<bool> DeleteActivityAsync(string travelId, string destinationId, string id)
        {
            using var scope = _scopeFactory.CreateScope();
            var activityService = scope.ServiceProvider.GetRequiredService<IActivityBusinessService>();
            return await activityService.DeleteActivityAsync(travelId, destinationId, id, CancellationToken.None);
        }

        public async Task<ActivityDto?> UpdateActivityAsync(string travelId, string destinationId, string id, UpdateActivityDto request)
        {
            using var scope = _scopeFactory.CreateScope();
            var activityService = scope.ServiceProvider.GetRequiredService<IActivityBusinessService>();
            return await activityService.UpdateActivityAsync(travelId, destinationId, id, request, CancellationToken.None);
        }

        // Operacije vezane za checkliste
        public async Task<ChecklistDto> AddChecklistAsync(string travelId, CreateChecklistDto request)
        {
            using var scope = _scopeFactory.CreateScope();
            var checklistService = scope.ServiceProvider.GetRequiredService<IChecklistBusinessService>();
            return await checklistService.AddChecklistAsync(travelId, request, CancellationToken.None);
        }

        public async Task<ChecklistDto?> GetChecklistByIdAsync(string travelId, string id)
        {
            using var scope = _scopeFactory.CreateScope();
            var checklistService = scope.ServiceProvider.GetRequiredService<IChecklistBusinessService>();
            return await checklistService.GetChecklistByIdAsync(travelId, id, CancellationToken.None);
        }

        public async Task<IEnumerable<ChecklistDto>> GetChecklistsByTravelIdAsync(string travelId)
        {
            using var scope = _scopeFactory.CreateScope();
            var checklistService = scope.ServiceProvider.GetRequiredService<IChecklistBusinessService>();
            return await checklistService.GetChecklistsByTravelIdAsync(travelId, CancellationToken.None);
        }

        public async Task<ChecklistDto?> ToggleChecklistAsync(string travelId, string id, bool isCompleted)
        {
            using var scope = _scopeFactory.CreateScope();
            var checklistService = scope.ServiceProvider.GetRequiredService<IChecklistBusinessService>();
            return await checklistService.ToggleChecklistAsync(travelId, id, isCompleted, CancellationToken.None);
        }

        public async Task<bool> DeleteChecklistAsync(string travelId, string id)
        {
            using var scope = _scopeFactory.CreateScope();
            var checklistService = scope.ServiceProvider.GetRequiredService<IChecklistBusinessService>();
            return await checklistService.DeleteChecklistAsync(travelId, id, CancellationToken.None);
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
