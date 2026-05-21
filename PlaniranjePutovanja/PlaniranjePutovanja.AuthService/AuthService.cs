using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using PlaniranjePutovanja.AuthService.Persistence;
using PlaniranjePutovanja.AuthService.Services;
using PlaniranjePutovanja.Common.DTOs.Auth;
using PlaniranjePutovanja.Common.Interfaces.Auth;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.AuthService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class AuthService : StatelessService, IAuthService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public AuthService(StatelessServiceContext context, IServiceProvider serviceProvider)
            : base(context)
        {
            _scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        }

        /// <summary>
        /// Registracija novog korisnika
        /// </summary>
        public async Task<AuthResponseDto> RegisterUserAsync(RegisterRequestDto request)
        {
            using var scope = _scopeFactory.CreateScope();
            var businessService = scope.ServiceProvider.GetRequiredService<IAuthBusinessService>();

            return await businessService.RegisterUserAsync(request);
        }

        /// <summary>
        /// Prijava korisnika
        /// </summary>
        public async Task<AuthResponseDto> LoginUserAsync(LoginRequestDto request)
        {
            using var scope = _scopeFactory.CreateScope();
            var businessService = scope.ServiceProvider.GetRequiredService<IAuthBusinessService>();

            return await businessService.LoginAsync(request);
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
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
