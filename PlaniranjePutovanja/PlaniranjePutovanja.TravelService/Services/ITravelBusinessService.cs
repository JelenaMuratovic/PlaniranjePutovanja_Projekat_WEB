using PlaniranjePutovanja.Common.DTOs.Travel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Services
{
    public interface ITravelBusinessService
    {
        Task<TravelDto> CreateTravelAsync(string userId, CreateTravelDto dto, CancellationToken cancellationToken = default);

        Task<TravelDto?> GetTravelByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<IEnumerable<TravelDto>> GetTravelsByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<IEnumerable<TravelDto>> GetAllTravelsAsync(CancellationToken cancellationToken = default);

        Task<bool> DeleteTravelAsync(string id, CancellationToken cancellationToken = default);
    }
}
