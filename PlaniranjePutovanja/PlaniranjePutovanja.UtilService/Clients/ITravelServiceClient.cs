using PlaniranjePutovanja.Common.DTOs.Travel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.UtilService.Clients
{
    public interface ITravelServiceClient
    {
        Task<TravelDto?> GetTravelByIdAsync(string travelId);
        Task<IEnumerable<DestinationDto>> GetDestinationsByTravelIdAsync(string travelId);
        Task<IEnumerable<ActivityDto>> GetActivitiesByTravelIdAsync(string travelId);
    }
}
