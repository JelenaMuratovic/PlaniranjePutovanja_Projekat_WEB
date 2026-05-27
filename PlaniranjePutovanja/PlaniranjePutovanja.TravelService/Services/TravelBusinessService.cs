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
    public sealed class TravelBusinessService : ITravelBusinessService
    {
        private readonly ITravelRepository _travelRepository;
        private readonly ITravelMapper _travelMapper;
        private readonly IValidator<CreateTravelDto> _createTravelValidator;

        public TravelBusinessService(
        ITravelRepository travelRepository,
        ITravelMapper travelMapper,
        IValidator<CreateTravelDto> createTravelValidator)
        {
            _travelRepository = travelRepository;
            _travelMapper = travelMapper;
            _createTravelValidator = createTravelValidator;
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
    }
}
