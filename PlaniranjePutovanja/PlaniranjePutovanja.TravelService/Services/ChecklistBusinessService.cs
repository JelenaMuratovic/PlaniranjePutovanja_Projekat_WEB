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
    public sealed class ChecklistBusinessService : IChecklistBusinessService
    {
        private readonly ITravelRepository _travelRepository;
        private readonly IChecklistRepository _checklistRepository;
        private readonly ITravelMapper _travelMapper;
        private readonly IValidator<CreateChecklistDto> _createChecklistValidator;

        public ChecklistBusinessService(
        ITravelRepository travelRepository,
        IChecklistRepository checklistRepository,
        ITravelMapper travelMapper,
        IValidator<CreateChecklistDto> createChecklistValidator)
        {
            _travelRepository = travelRepository;
            _checklistRepository = checklistRepository;
            _travelMapper = travelMapper;
            _createChecklistValidator = createChecklistValidator;
        }

        public async Task<ChecklistDto> AddChecklistAsync(string travelId, CreateChecklistDto dto, CancellationToken cancellationToken = default)
        {
            var validationResult = await _createChecklistValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var travel = await _travelRepository.GetByIdAsync(travelId, cancellationToken);
            if (travel == null)
            {
                throw new KeyNotFoundException($"Travel with id '{travelId}' was not found.");
            }

            var checklist = _travelMapper.ToChecklist(dto);
            checklist.TravelId = travelId;
            await _checklistRepository.AddAsync(checklist, cancellationToken);

            return _travelMapper.ToChecklistDto(checklist);
        }

        public async Task<ChecklistDto?> GetChecklistByIdAsync(string travelId, string id, CancellationToken cancellationToken = default)
        {
            var checklist = await _checklistRepository.GetByIdAsync(id, cancellationToken);
            if (checklist == null) return null;

            if (checklist.TravelId != travelId)
            {
                throw new UnauthorizedAccessException("The checklist doesn't belong to this travel.");
            }

            return _travelMapper.ToChecklistDto(checklist);
        }

        public async Task<IEnumerable<ChecklistDto>> GetChecklistsByTravelIdAsync(string travelId, CancellationToken cancellationToken = default)
        {
            var checklists = await _checklistRepository.GetByTravelIdAsync(travelId, cancellationToken);
            return checklists.Select(_travelMapper.ToChecklistDto).ToList();
        }

        public async Task<ChecklistDto?> ToggleChecklistAsync(string travelId, string id, bool isCompleted, CancellationToken cancellationToken = default)
        {
            var checklist = await _checklistRepository.GetByIdAsync(id, cancellationToken);
            if (checklist == null)
            {
                return null;
            }
            if (checklist.TravelId != travelId)
            {
                throw new UnauthorizedAccessException("This checklist item does not belong to the specified travel.");
            }

            checklist.IsCompleted = isCompleted;
            checklist.CompletedAt = isCompleted ? DateTime.UtcNow : default;

            await _checklistRepository.UpdateAsync(checklist, cancellationToken);

            return _travelMapper.ToChecklistDto(checklist);
        }

        public async Task<bool> DeleteChecklistAsync(string travelId, string id, CancellationToken cancellationToken = default)
        {
            var checklist = await _checklistRepository.GetByIdAsync(id, cancellationToken);
            if (checklist == null)
            {
                return false;
            }
            if (checklist.TravelId != travelId)
            {
                throw new UnauthorizedAccessException("This checklist item does not belong to the specified travel.");
            }

            await _checklistRepository.DeleteAsync(id, cancellationToken);
            return true;
        }
    }
}
