using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using PlaniranjePutovanja.Common.DTOs.Util;
using PlaniranjePutovanja.Common.Interfaces.Util;
using PlaniranjePutovanja.UtilService.Services;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.UtilService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class UtilService : StatelessService, IUtilService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public UtilService(StatelessServiceContext context, IServiceProvider serviceProvider)
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

        public async Task<GeneratedFileDto> GenerateTravelPlanPdfAsync(string travelId)
        {
            using var scope = _scopeFactory.CreateScope();
            var util = scope.ServiceProvider.GetRequiredService<IUtilBusinessService>();
            return await util.GenerateTravelPlanPdfAsync(travelId);
        }

        public async Task<ShareLinkResponseDto> GenerateShareQrCodeAsync(string travelId, GenerateShareLinkRequestDto request)
        {
            using var scope = _scopeFactory.CreateScope();
            var util = scope.ServiceProvider.GetRequiredService<IUtilBusinessService>();
            return await util.GenerateShareQrCodeAsync(travelId, request);
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // Drzimo servis zivim
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
    }
}
