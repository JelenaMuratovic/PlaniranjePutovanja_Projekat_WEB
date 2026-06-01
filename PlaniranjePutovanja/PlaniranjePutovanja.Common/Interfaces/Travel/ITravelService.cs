using Microsoft.ServiceFabric.Services.Remoting;
using PlaniranjePutovanja.Common.DTOs.Travel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.Common.Interfaces.Travel
{
    public interface ITravelService : IService
    {
        // Operacije vezane za putovanja
        Task<TravelDto> CreateTravelAsync(string userId, CreateTravelDto request);

        Task<TravelDto?> GetTravelByIdAsync(string id);

        Task<IEnumerable<TravelDto>> GetTravelsByUserIdAsync(string userId);

        Task<IEnumerable<TravelDto>> GetAllTravelsAsync();

        Task<bool> DeleteTravelAsync(string id);

        Task<TravelDto?> UpdateTravelAsync(string id, UpdateTravelDto request);

        // Operacije vezane za destinacije
        Task<DestinationDto> AddDestinationAsync(string travelId, CreateDestinationDto request);

        Task<DestinationDto?> GetDestinationByIdAsync(string travelid, string id);

        Task<IEnumerable<DestinationDto>> GetDestinationsByTravelIdAsync(string travelId);

        Task<bool> DeleteDestinationAsync(string travelId, string id);

        Task<DestinationDto?> UpdateDestinationAsync(string travelId, string id, UpdateDestinationDto request);

        // Operacije vezane za aktivnosti
        Task<ActivityDto> AddActivityAsync(string travelId, string destinationId, CreateActivityDto request);

        Task<ActivityDto?> GetActivityByIdAsync(string travelId, string destinationId, string id);

        Task<IEnumerable<ActivityDto>> GetActivitiesByDestinationIdAsync(string travelId, string destinationId);

        Task<bool> DeleteActivityAsync(string travelId, string destinationId, string id);

        Task<ActivityDto?> UpdateActivityAsync(string travelId, string destinationId, string id, UpdateActivityDto request);

        // Operacije vezane za checkliste
        Task<ChecklistDto> AddChecklistAsync(string travelId, CreateChecklistDto request);

        Task<ChecklistDto?> GetChecklistByIdAsync(string travelId, string id);

        Task<IEnumerable<ChecklistDto>> GetChecklistsByTravelIdAsync(string travelId);

        Task<ChecklistDto?> ToggleChecklistAsync(string travelId, string id, bool isCompleted);

        Task<bool> DeleteChecklistAsync(string travelId, string id);
    }
}
