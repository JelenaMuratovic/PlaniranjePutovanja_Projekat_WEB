using FluentValidation;
using PlaniranjePutovanja.Common.DTOs.Travel;
using PlaniranjePutovanja.Common.Enums;
using PlaniranjePutovanja.TravelService.Mappers;
using PlaniranjePutovanja.TravelService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Services
{
    public sealed class ActivityBusinessService : IActivityBusinessService
    {
        private readonly IDestinationRepository _destinationRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly ITravelMapper _travelMapper;
        private readonly IValidator<CreateActivityDto> _createActivityValidator;
        private readonly IValidator<UpdateActivityDto> _updateActivityValidator;

        public ActivityBusinessService(
        IDestinationRepository destinationRepository,
        IActivityRepository activityRepository,
        ITravelMapper travelMapper,
        IValidator<CreateActivityDto> createActivityValidator,
        IValidator<UpdateActivityDto> updateActivityValidator)
        {
            _destinationRepository = destinationRepository;
            _activityRepository = activityRepository;
            _travelMapper = travelMapper;
            _createActivityValidator = createActivityValidator;
            _updateActivityValidator = updateActivityValidator;
        }

        public async Task<ActivityDto> AddActivityAsync(string travelId, string destinationId, CreateActivityDto dto, CancellationToken cancellationToken = default)
        {
            var validationResult = await _createActivityValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var destination = await _destinationRepository.GetByIdAsync(destinationId, cancellationToken);
            if (destination == null)
            {
                throw new KeyNotFoundException($"Destination with id '{destinationId}' was not found, so you cannot add an activity to it.");
            }
            if (destination.TravelId != travelId)
            {
                throw new UnauthorizedAccessException("The destination does not belong to the specified travel, so you cannot add an activity to it.");
            }

            var activity = _travelMapper.ToActivity(dto);
            activity.DestinationId = destinationId;
            await _activityRepository.AddAsync(activity, cancellationToken);

            return _travelMapper.ToActivityDto(activity);
        }

        public async Task<ActivityDto?> GetActivityByIdAsync(string travelId, string destinationId, string id, CancellationToken cancellationToken = default)
        {
            var activity = await _activityRepository.GetByIdAsync(id, cancellationToken);
            if (activity == null)
            {
                return null;
            }
            if (activity.DestinationId != destinationId)
            {
                throw new UnauthorizedAccessException("The activity does not belong to the specified destination.");
            }
            var destination = await _destinationRepository.GetByIdAsync(destinationId, cancellationToken);
            if (destination == null)
            {
                throw new KeyNotFoundException($"Destination with id '{destinationId}' was not found, so you cannot view the activity for it.");
            }
            if (destination.TravelId != travelId)
            {
                throw new UnauthorizedAccessException("The destination does not belong to the specified travel, so you cannot view the activity for it.");
            }

            return _travelMapper.ToActivityDto(activity);
        }

        public async Task<IEnumerable<ActivityDto>> GetActivitiesByDestinationIdAsync(string travelId, string destinationId, CancellationToken cancellationToken = default)
        {
            var destination = await _destinationRepository.GetByIdAsync(destinationId, cancellationToken);
            if (destination == null)
            {
                throw new KeyNotFoundException($"Destination with id '{destinationId}' was not found, so you cannot view the activities for it.");
            }

            if (destination.TravelId != travelId)
            {
                throw new UnauthorizedAccessException("The destination does not belong to the specified travel, so you cannot view the activities for it.");
            }

            var activities = await _activityRepository.GetByDestinationIdAsync(destinationId, cancellationToken);
            return activities.Select(_travelMapper.ToActivityDto).ToList();
        }

        public async Task<bool> DeleteActivityAsync(string travelId, string destinationId, string id, CancellationToken cancellationToken = default)
        {
            var activity = await _activityRepository.GetByIdAsync(id, cancellationToken);
            if (activity == null)
            {
                return false;
            }
            if (activity.DestinationId != destinationId)
            {
                throw new UnauthorizedAccessException("The activity does not belong to the specified destination.");
            }
            var destination = await _destinationRepository.GetByIdAsync(destinationId, cancellationToken);
            if (destination == null)
            {
                throw new KeyNotFoundException($"Destination with id '{destinationId}' was not found, so you cannot delete the activity for it.");
            }
            if (destination.TravelId != travelId)
            {
                throw new UnauthorizedAccessException("The destination does not belong to the specified travel, so you cannot delete the activity for it.");
            }

            await _activityRepository.DeleteAsync(id, cancellationToken);
            return true;
        }

        public async Task<ActivityDto?> UpdateActivityAsync(string travelId, string destinationId, string id, UpdateActivityDto dto, CancellationToken cancellationToken = default)
        {
            var validationResult = await _updateActivityValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
            var activity = await _activityRepository.GetByIdAsync(id, cancellationToken);
            if (activity == null)
            {
                return null;
            }
            if (activity.DestinationId != destinationId)
            {
                throw new UnauthorizedAccessException("The activity does not belong to the specified destination.");
            }
            var destination = await _destinationRepository.GetByIdAsync(destinationId, cancellationToken);
            if (destination == null)
            {
                throw new KeyNotFoundException($"Destination with id '{destinationId}' was not found, so you can not update the activity for it.");
            }
            if (destination.TravelId != travelId)
            {
                throw new UnauthorizedAccessException("The destination does not belong to the specified travel, so you can not update the activity for it.");
            }
            activity.Name = dto.Name;
            activity.Description = dto.Description;
            activity.ActivityDate = dto.ActivityDate;
            activity.StartTime = dto.StartTime;
            activity.Price = dto.Price;
            activity.Status = dto.Status;

            await _activityRepository.UpdateAsync(activity, cancellationToken);
            return _travelMapper.ToActivityDto(activity);
        }
    }
}
