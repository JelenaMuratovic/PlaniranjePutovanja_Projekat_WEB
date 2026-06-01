using FluentValidation;
using PlaniranjePutovanja.Common.DTOs.Travel;
using PlaniranjePutovanja.TravelService.Mappers;
using PlaniranjePutovanja.TravelService.Models;
using PlaniranjePutovanja.TravelService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Services
{
    public sealed class DestinationBusinessService : IDestinationBusinessService
    {
        private readonly ITravelRepository _travelRepository;
        private readonly IDestinationRepository _destinationRepository;
        private readonly ITravelMapper _travelMapper;
        private readonly IValidator<CreateDestinationDto> _createDestinationValidator;
        private readonly IValidator<UpdateDestinationDto> _updateDestinationValidator;

        public DestinationBusinessService(
        ITravelRepository travelRepository,
        IDestinationRepository destinationRepository,
        ITravelMapper travelMapper,
        IValidator<CreateDestinationDto> createDestinationValidator,
        IValidator<UpdateDestinationDto> updateDestinationValidator)
        {
            _travelRepository = travelRepository;
            _destinationRepository = destinationRepository;
            _travelMapper = travelMapper;
            _createDestinationValidator = createDestinationValidator;
            _updateDestinationValidator = updateDestinationValidator;
        }

        public async Task<DestinationDto> AddDestinationAsync(string travelId, CreateDestinationDto dto, CancellationToken cancellationToken = default)
        {
            var validationResult = await _createDestinationValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
            var travel = await _travelRepository.GetByIdAsync(travelId, cancellationToken);
            if (travel == null)
            {
                throw new KeyNotFoundException($"Travel with id '{travelId}' was not found.");
            }

            var existingDestinations = await _destinationRepository.GetByTravelIdAsync(travelId, cancellationToken);
            EnsureDestinationFitsTravel(travel, existingDestinations, dto.DaysSpent);


            var destination = _travelMapper.ToDestination(dto);
            destination.TravelId = travelId;
            await _destinationRepository.AddAsync(destination, cancellationToken);

            return _travelMapper.ToDestinationDto(destination);
        }

        public async Task<DestinationDto?> GetDestinationByIdAsync(string travelId, string id, CancellationToken cancellationToken = default)
        {
            var destination = await _destinationRepository.GetByIdAsync(id, cancellationToken);
            if (destination == null)
            {
                return null;
            }

            if (destination.TravelId != travelId)
            {
                throw new UnauthorizedAccessException("You don't have access to this destination.");
            }

            return _travelMapper.ToDestinationDto(destination);
        }

        public async Task<IEnumerable<DestinationDto>> GetDestinationsByTravelIdAsync(string travelId, CancellationToken cancellationToken = default)
        {
            var destinations = await _destinationRepository.GetByTravelIdAsync(travelId, cancellationToken);
            return destinations.Select(_travelMapper.ToDestinationDto).ToList();
        }

        public async Task<bool> DeleteDestinationAsync(string travelId, string id, CancellationToken cancellationToken = default)
        {
            var destination = await _destinationRepository.GetByIdAsync(id, cancellationToken);
            if (destination == null)
            {
                return false;
            }
            if (destination.TravelId != travelId)
            {
                throw new UnauthorizedAccessException("You don't have access to this destination.");
            }

            await _destinationRepository.DeleteAsync(id, cancellationToken);
            return true;
        }

        public async Task<DestinationDto?> UpdateDestinationAsync(string travelId, string id, UpdateDestinationDto dto, CancellationToken cancellationToken = default)
        {
            var result = await _updateDestinationValidator.ValidateAsync(dto, cancellationToken);
            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
            var destination = await _destinationRepository.GetByIdAsync(id, cancellationToken);
            if (destination == null)
            {
                return null;
            }
            if (destination.TravelId != travelId)
            {
                throw new UnauthorizedAccessException("You don't have access to this destination.");
            }

            var travel = await _travelRepository.GetByIdAsync(travelId, cancellationToken);
            if (travel == null)
            {
                throw new KeyNotFoundException($"Travel with id '{travelId}' was not found.");
            }

            var existingDestinations = await _destinationRepository.GetByTravelIdAsync(travelId, cancellationToken);
            EnsureDestinationFitsTravel(travel, existingDestinations, dto.DaysSpent, id);

            destination.Name = dto.Name;
            destination.Country = dto.Country;
            destination.City = dto.City;
            destination.Latitude = dto.Latitude;
            destination.Longitude = dto.Longitude;
            destination.DaysSpent = dto.DaysSpent;
            destination.Description = dto.Description;
            destination.Notes = dto.Notes;

            await _destinationRepository.UpdateAsync(destination, cancellationToken);
            return _travelMapper.ToDestinationDto(destination);
        }

        private static void EnsureDestinationFitsTravel(
            Travel travel,
            IEnumerable<Destination> destinations,
            int daysSpent,
            string? excludedDestinationId = null)
        {
            var travelDurationDays = GetTravelDurationDays(travel);

            var allocatedDays = destinations
                .Where(destination => destination.Id != excludedDestinationId)
                .Sum(destination => destination.DaysSpent);

            var totalDays = allocatedDays + daysSpent;
            if (totalDays > travelDurationDays)
            {
                throw new ValidationException(
                    $"Destination duration exceeds the travel duration. Travel allows {travelDurationDays} day(s), " +
                    $"but {allocatedDays} day(s) are already assigned and {daysSpent} more day(s) were requested.");
            }
        }

        private static int GetTravelDurationDays(Travel travel)
        {
            return (travel.EndDate.Date - travel.StartDate.Date).Days + 1;
        }
    }
}
