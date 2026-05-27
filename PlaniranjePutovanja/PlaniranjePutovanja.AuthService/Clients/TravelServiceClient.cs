using Microsoft.ServiceFabric.Services.Remoting.Client;
using PlaniranjePutovanja.Common.DTOs.Travel;
using PlaniranjePutovanja.Common.Interfaces.Travel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.AuthService.Clients
{
    public sealed class TravelServiceClient : ITravelServiceClient
    {
        private readonly Uri _travelServiceUri = new("fabric:/PlaniranjePutovanja/PlaniranjePutovanja.TravelService");

        public async Task<IEnumerable<TravelDto>> GetTravelsByUserIdAsync(string userId)
        {
            var proxy = ServiceProxy.Create<ITravelService>(_travelServiceUri);
            return await proxy.GetTravelsByUserIdAsync(userId);
        }

        public async Task<bool> DeleteTravelAsync(string travelId)
        {
            var proxy = ServiceProxy.Create<ITravelService>(_travelServiceUri);
            return await proxy.DeleteTravelAsync(travelId);
        }
    }
}
