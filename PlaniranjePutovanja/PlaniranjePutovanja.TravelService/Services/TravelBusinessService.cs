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
    public sealed class TravelBusinessService : ITravelBusinessService
    {
        private readonly ITravelRepository _travelRepository;
        IDestinationRepository _destinationRepository;
        private readonly ITravelMapper _travelMapper;
        private readonly IValidator<CreateTravelDto> _createTravelValidator;
        private readonly IValidator<UpdateTravelDto> _updateTravelValidator;

        public TravelBusinessService(
        ITravelRepository travelRepository,
        IDestinationRepository destinationRepository,
        ITravelMapper travelMapper,
        IValidator<CreateTravelDto> createTravelValidator,
        IValidator<UpdateTravelDto> updateTravelValidator)
        {
            _travelRepository = travelRepository;
            _destinationRepository = destinationRepository;
            _travelMapper = travelMapper;
            _createTravelValidator = createTravelValidator;
            _updateTravelValidator = updateTravelValidator;
        }

        public async Task<TravelDto> CreateTravelAsync(string userId, CreateTravelDto dto, CancellationToken cancellationToken = default)
        {
            var validationResult = await _createTravelValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var travel = _travelMapper.ToTravel(dto);
            travel.UserId = userId;
            await _travelRepository.AddAsync(travel, cancellationToken);

            return _travelMapper.ToTravelDto(travel);
        }

        public async Task<TravelDto?> GetTravelByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var travel = await _travelRepository.GetByIdAsync(id, cancellationToken);
            return travel == null ? null : _travelMapper.ToTravelDto(travel);
        }

        public async Task<IEnumerable<TravelDto>> GetTravelsByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var travels = await _travelRepository.GetByUserIdAsync(userId, cancellationToken);
            return travels.Select(_travelMapper.ToTravelDto).ToList();
        }

        public async Task<IEnumerable<TravelDto>> GetAllTravelsAsync(CancellationToken cancellationToken = default)
        {
            var travels = await _travelRepository.GetAllAsync(cancellationToken);
            return travels.Select(_travelMapper.ToTravelDto).ToList();
        }

        public async Task<bool> DeleteTravelAsync(string id, CancellationToken cancellationToken = default)
        {
            var travel = await _travelRepository.GetByIdAsync(id, cancellationToken);
            if (travel == null)
            {
                return false;
            }

            await _travelRepository.DeleteAsync(id, cancellationToken);
            return true;
        }

        public async Task<TravelDto?> UpdateTravelAsync(string id, UpdateTravelDto dto, CancellationToken cancellationToken = default)
        {
            var travel = await _travelRepository.GetByIdAsync(id, cancellationToken);
            if (travel == null)
            {
                return null;
            }

            var validationResult = await _updateTravelValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var destinations = await _destinationRepository.GetByTravelIdAsync(id, cancellationToken);
            EnsureTravelCanFitDestinations(dto.StartDate, dto.EndDate, destinations);

            travel.Name = dto.Name;
            travel.Description = dto.Description;
            travel.StartDate = dto.StartDate;
            travel.EndDate = dto.EndDate;
            travel.Budget = dto.Budget;
            travel.Notes = dto.Notes;

            await _travelRepository.UpdateAsync(travel, cancellationToken);

            return _travelMapper.ToTravelDto(travel);
        }

        private static void EnsureTravelCanFitDestinations(
            DateTime startDate,
            DateTime endDate,
            IEnumerable<Destination> destinations)
        {
            var travelDurationDays = GetTravelDurationDays(startDate, endDate);
            var assignedDays = destinations.Sum(destination => destination.DaysSpent);

            if (assignedDays > travelDurationDays)
            {
                throw new ValidationException(
                    $"Travel duration is too short for the existing destinations. Travel allows {travelDurationDays} day(s), " +
                    $"but destinations currently use {assignedDays} day(s).");
            }
        }

        private static int GetTravelDurationDays(DateTime startDate, DateTime endDate)
        {
            return (endDate.Date - startDate.Date).Days + 1;
        }
    }
}
