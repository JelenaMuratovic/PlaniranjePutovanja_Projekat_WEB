using PlaniranjePutovanja.Common.DTOs.Travel;
using PlaniranjePutovanja.TravelService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Mappers
{
    public sealed class TravelMapper : ITravelMapper
    {
        public Travel ToTravel(CreateTravelDto dto)
        {
            return new Travel
            {
                Name = dto.Name,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Budget = dto.Budget,
                Notes = dto.Notes
            };
        }

        public TravelDto ToTravelDto(Travel travel)
        {
            return new TravelDto
            {
                Id = travel.Id,
                UserId = travel.UserId,
                Name = travel.Name,
                Description = travel.Description,
                StartDate = travel.StartDate,
                EndDate = travel.EndDate,
                Budget = travel.Budget,
                Notes = travel.Notes,
                CreatedAt = travel.CreatedAt,
                DestinationCount = travel.Destinations?.Count ?? 0
            };
        }

        public Destination ToDestination(CreateDestinationDto dto)
        {
            return new Destination
            {
                Name = dto.Name,
                Country = dto.Country,
                City = dto.City,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                DaysSpent = dto.DaysSpent,
                Description = dto.Description,
                Notes = dto.Notes
            };
        }

        public DestinationDto ToDestinationDto(Destination destination)
        {
            return new DestinationDto
            {
                Id = destination.Id,
                TravelId = destination.TravelId,
                Name = destination.Name,
                Country = destination.Country,
                City = destination.City,
                Latitude = destination.Latitude,
                Longitude = destination.Longitude,
                DaysSpent = destination.DaysSpent,
                Description = destination.Description,
                Notes = destination.Notes,
                ActivityCount = destination.Activities?.Count ?? 0
            };
        }

        public Activity ToActivity(CreateActivityDto dto)
        {
            return new Activity
            {
                Name = dto.Name,
                Description = dto.Description,
                ActivityDate = dto.ActivityDate,
                StartTime = dto.StartTime,
                Price = dto.Price,
                Status = dto.Status
            };
        }

        public ActivityDto ToActivityDto(Activity activity)
        {
            return new ActivityDto
            {
                Id = activity.Id,
                DestinationId = activity.DestinationId,
                Name = activity.Name,
                Description = activity.Description,
                ActivityDate = activity.ActivityDate,
                StartTime = activity.StartTime,
                Price = activity.Price,
                Status = activity.Status.ToString(),
                CreatedAt = activity.CreatedAt
            };
        }

        public Checklist ToChecklist(CreateChecklistDto dto)
        {
            return new Checklist
            {
                Item = dto.Item
            };
        }

        public ChecklistDto ToChecklistDto(Checklist checklist)
        {
            return new ChecklistDto
            {
                Id = checklist.Id,
                TravelId = checklist.TravelId,
                Item = checklist.Item,
                IsCompleted = checklist.IsCompleted,
                CreatedAt = checklist.CreatedAt
            };
        }
    }
}
