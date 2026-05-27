using PlaniranjePutovanja.Common.DTOs.Travel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Services
{
    public interface IChecklistBusinessService
    {
        Task<ChecklistDto> AddChecklistAsync(string travelId, CreateChecklistDto dto, CancellationToken cancellationToken = default);

        Task<ChecklistDto?> GetChecklistByIdAsync(string travelId, string id, CancellationToken cancellationToken = default);

        Task<IEnumerable<ChecklistDto>> GetChecklistsByTravelIdAsync(string travelId, CancellationToken cancellationToken = default);

        Task<ChecklistDto?> ToggleChecklistAsync(string travelId, string id, bool isCompleted, CancellationToken cancellationToken = default);

        Task<bool> DeleteChecklistAsync(string travelId, string id, CancellationToken cancellationToken = default);
    }
}
