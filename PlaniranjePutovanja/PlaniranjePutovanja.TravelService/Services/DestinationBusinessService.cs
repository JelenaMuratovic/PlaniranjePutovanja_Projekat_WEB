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
    public sealed class DestinationBusinessService : IDestinationBusinessService
    {
        private readonly ITravelRepository _travelRepository;
        private readonly IDestinationRepository _destinationRepository;
        private readonly ITravelMapper _travelMapper;
        private readonly IValidator<CreateDestinationDto> _createDestinationValidator;

        public DestinationBusinessService(
        ITravelRepository travelRepository,
        IDestinationRepository destinationRepository,
        ITravelMapper travelMapper,
        IValidator<CreateDestinationDto> createDestinationValidator)
        {
            _travelRepository = travelRepository;
            _destinationRepository = destinationRepository;
            _travelMapper = travelMapper;
            _createDestinationValidator = createDestinationValidator;
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
                throw new InvalidOperationException($"Travel with id '{travelId}' was not found.");
            }

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
    }
}
