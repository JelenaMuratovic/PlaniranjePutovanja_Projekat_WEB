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
            return ServiceProxy.Create<ITravelService>(_travelServiceUri, new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(0));
        }

        public async Task<IEnumerable<TravelDto>> GetAllTravelsAsync()
        {
            try
            {
                var proxy = GetProxy();
                return await proxy.GetAllTravelsAsync();
            }
            catch (Exception)
            {
                // Fallback ako servis ne radi
                return Array.Empty<TravelDto>();
            }
        }

        public async Task<TravelDto?> GetTravelByIdAsync(string travelId)
        {
            try
            {
                var proxy = GetProxy();
                return await proxy.GetTravelByIdAsync(travelId);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
