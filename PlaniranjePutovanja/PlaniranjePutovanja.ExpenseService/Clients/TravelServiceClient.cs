using Microsoft.ServiceFabric.Services.Remoting.Client;
using PlaniranjePutovanja.Common.DTOs.Travel;
using PlaniranjePutovanja.Common.Interfaces.Travel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.ExpenseService.Clients
{
    public sealed class TravelServiceClient : ITravelServiceClient
    {
        private readonly Uri _travelServiceUri = new("fabric:/PlaniranjePutovanja/PlaniranjePutovanja.TravelService");

        private ITravelService GetProxy()
        {
            return ServiceProxy.Create<ITravelService>(_travelServiceUri);
        }

        public async Task<IEnumerable<TravelDto>> GetAllTravelsAsync()
        {
            var proxy = GetProxy();
            return await proxy.GetAllTravelsAsync();
        }

        public async Task<TravelDto?> GetTravelByIdAsync(string travelId)
        {
            var proxy = GetProxy();
            return await proxy.GetTravelByIdAsync(travelId);
        }
    }
}
