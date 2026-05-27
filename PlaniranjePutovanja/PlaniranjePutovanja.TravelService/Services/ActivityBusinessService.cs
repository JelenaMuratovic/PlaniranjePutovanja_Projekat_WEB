using FluentValidation;
using PlaniranjePutovanja.Common.DTOs.Travel;
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

        public ActivityBusinessService(
        IDestinationRepository destinationRepository,
        IActivityRepository activityRepository,
        ITravelMapper travelMapper,
        IValidator<CreateActivityDto> createActivityValidator)
        {
            _destinationRepository = destinationRepository;
            _activityRepository = activityRepository;
            _travelMapper = travelMapper;
            _createActivityValidator = createActivityValidator;
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
                throw new InvalidOperationException($"Destination with id '{destinationId}' was not found.");
            }
            if (destination.TravelId != travelId)
            {
                throw new UnauthorizedAccessException("The destination does not belong to the specified travel.");
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
            if (destination == null || destination.TravelId != travelId)
            {
                throw new UnauthorizedAccessException("The activity does not belong to the specified travel.");
            }
            return _travelMapper.ToActivityDto(activity);
        }

        public async Task<IEnumerable<ActivityDto>> GetActivitiesByDestinationIdAsync(string travelId, string destinationId, CancellationToken cancellationToken = default)
        {
            var destination = await _destinationRepository.GetByIdAsync(destinationId, cancellationToken);
            if (destination == null || destination.TravelId != travelId)
            {
                throw new UnauthorizedAccessException("The destination does not belong to the specified travel.");
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
            if (destination == null || destination.TravelId != travelId)
            {
                throw new UnauthorizedAccessException("The activity does not belong to the specified travel.");
            }

            await _activityRepository.DeleteAsync(id, cancellationToken);
            return true;
        }
    }
}
