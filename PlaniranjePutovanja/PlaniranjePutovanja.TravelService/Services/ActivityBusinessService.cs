using FluentValidation;
using PlaniranjePutovanja.Common.DTOs.Travel;
using PlaniranjePutovanja.Common.Enums;
using PlaniranjePutovanja.TravelService.Clients;
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
        //dodah
        private readonly ITravelRepository _travelRepository;
        private readonly ITravelMapper _travelMapper;
        private readonly IValidator<CreateActivityDto> _createActivityValidator;
        private readonly IValidator<UpdateActivityDto> _updateActivityValidator;
        private readonly IActivityExpenseClient _activityExpenseClient;

        public ActivityBusinessService(
        IDestinationRepository destinationRepository,
        IActivityRepository activityRepository,
        ITravelRepository travelRepository,
        ITravelMapper travelMapper,
        IValidator<CreateActivityDto> createActivityValidator,
        IValidator<UpdateActivityDto> updateActivityValidator,
        IActivityExpenseClient activityExpenseClient)
        {
            _destinationRepository = destinationRepository;
            _activityRepository = activityRepository;
            _travelRepository = travelRepository;
            _travelMapper = travelMapper;
            _createActivityValidator = createActivityValidator;
            _updateActivityValidator = updateActivityValidator;
            _activityExpenseClient = activityExpenseClient;
        }

        public async Task<ActivityDto> AddActivityAsync(string travelId, string destinationId, CreateActivityDto dto, CancellationToken cancellationToken = default)
        {
            var validationResult = await _createActivityValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            //dodah
            var travel = await _travelRepository.GetByIdAsync(travelId, cancellationToken);
            if (travel == null)
            {
                throw new KeyNotFoundException($"Travel with id '{travelId}' was not found.");
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

            // Ako aktivnost ima cenu, kreiramo sistemski trosak
            if (activity.Price != 0 && activity.Price > 0)
            {
                try
                {
                    await _activityExpenseClient.CreateActivityExpenseAsync(
                        travelId,
                        activity.Id,
                        activity.Name,
                        activity.Price,
                        travel.Budget);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Activity created but failed to create related expense.", ex);
                }
            }

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

            // Obrisemo povezani sistemski trosak
            if (activity.Price != 0 && activity.Price > 0)
            {
                try
                {
                    await _activityExpenseClient.DeleteActivityExpenseAsync(travelId, id);
                }
                catch (Exception ex)
                {
                    // Ne prekidamo brisanje — aktivnost je vec obrisana
                    throw new InvalidOperationException(
                        $"Activity deleted but failed to delete related expense.", ex);
                }
            }
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

            var oldPrice = activity.Price;
            activity.Name = dto.Name;
            activity.Description = dto.Description;
            activity.ActivityDate = dto.ActivityDate;
            activity.StartTime = dto.StartTime;
            activity.Price = dto.Price;
            activity.Status = dto.Status;

            await _activityRepository.UpdateAsync(activity, cancellationToken);

            // Sinhronizujemo cenu sa sistemskim troskom
            if (oldPrice != dto.Price)
            {
                try
                {
                    if (dto.Price != 0 && dto.Price > 0)
                    {
                        // Ako nova cena postoji, azuriramo trosak
                        await _activityExpenseClient.UpdateActivityExpenseAsync(
                            travelId,
                            id,
                            dto.Price);
                    }
                    else if (oldPrice != 0 && oldPrice > 0 && (dto.Price == 0))
                    {
                        // Ako je stara cena bila > 0 a nova je 0, obrisemo trosak
                        await _activityExpenseClient.DeleteActivityExpenseAsync(travelId, id);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Activity updated but failed to sync expense.", ex);
                }
            }
            return _travelMapper.ToActivityDto(activity);
        }
    }
}
