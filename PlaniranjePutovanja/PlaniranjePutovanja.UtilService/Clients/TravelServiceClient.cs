using Microsoft.ServiceFabric.Services.Remoting.Client;
using PlaniranjePutovanja.Common.DTOs.Travel;
using PlaniranjePutovanja.Common.Interfaces.Travel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.UtilService.Clients
{
    public sealed class TravelServiceClient : ITravelServiceClient
    {
        private readonly Uri _travelServiceUri = new("fabric:/PlaniranjePutovanja/PlaniranjePutovanja.TravelService");

        private ITravelService GetProxy()
        {
            return ServiceProxy.Create<ITravelService>(_travelServiceUri);
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

        public async Task<IEnumerable<DestinationDto>> GetDestinationsByTravelIdAsync(string travelId)
        {
            try
            {
                var proxy = GetProxy();
                return await proxy.GetDestinationsByTravelIdAsync(travelId);
            }
            catch (Exception)
            {
                return Array.Empty<DestinationDto>();
            }
        }

        public async Task<IEnumerable<ActivityDto>> GetActivitiesByTravelIdAsync(string travelId)
        {
            try
            {
                var proxy = GetProxy();
                var destinations = await proxy.GetDestinationsByTravelIdAsync(travelId);
                var allActivities = new List<ActivityDto>();

                foreach (var dest in destinations)
                {
                    var activities = await proxy.GetActivitiesByDestinationIdAsync(travelId, dest.Id);
                    allActivities.AddRange(activities);
                }

                return allActivities;
            }
            catch (Exception)
            {
                return Array.Empty<ActivityDto>();
            }
        }
    }
}
