using PlaniranjePutovanja.Common.DTOs.Travel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.ExpenseService.Clients
{
    public interface ITravelServiceClient
    {
        Task<IEnumerable<TravelDto>> GetAllTravelsAsync();
        Task<TravelDto?> GetTravelByIdAsync(string travelId);
    }
}
