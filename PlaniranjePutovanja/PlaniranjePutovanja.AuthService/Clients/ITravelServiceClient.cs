using PlaniranjePutovanja.Common.DTOs.Travel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.AuthService.Clients
{
    public interface ITravelServiceClient
    {
        Task<IEnumerable<TravelDto>> GetTravelsByUserIdAsync(string userId);
        Task<bool> DeleteTravelAsync(string travelId);
    }
}
