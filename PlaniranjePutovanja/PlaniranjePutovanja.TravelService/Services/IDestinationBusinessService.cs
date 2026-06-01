using PlaniranjePutovanja.Common.DTOs.Travel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Services
{
    public interface IDestinationBusinessService
    {
        Task<DestinationDto> AddDestinationAsync(string travelId, CreateDestinationDto dto, CancellationToken cancellationToken = default);

        Task<DestinationDto?> GetDestinationByIdAsync(string travelId, string id, CancellationToken cancellationToken = default);

        Task<IEnumerable<DestinationDto>> GetDestinationsByTravelIdAsync(string travelId, CancellationToken cancellationToken = default);

        Task<bool> DeleteDestinationAsync(string travelId, string id, CancellationToken cancellationToken = default);

        Task<DestinationDto?> UpdateDestinationAsync(string travelId, string id, UpdateDestinationDto dto, CancellationToken cancellationToken = default);
    }
}
