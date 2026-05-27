using PlaniranjePutovanja.Common.DTOs.Travel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Services
{
    public interface IActivityBusinessService
    {
        Task<ActivityDto> AddActivityAsync(string travelId, string destinationId, CreateActivityDto dto, CancellationToken cancellationToken = default);

        Task<ActivityDto?> GetActivityByIdAsync(string travelId, string destinationId, string id, CancellationToken cancellationToken = default);

        Task<IEnumerable<ActivityDto>> GetActivitiesByDestinationIdAsync(string travelId, string destinationId, CancellationToken cancellationToken = default);
        Task<bool> DeleteActivityAsync(string travelId, string destinationId, string id, CancellationToken cancellationToken = default);
    }
}
