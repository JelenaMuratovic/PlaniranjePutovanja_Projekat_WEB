using PlaniranjePutovanja.TravelService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Repositories
{
    public interface IChecklistRepository
    {
        Task<Checklist?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<IEnumerable<Checklist>> GetByTravelIdAsync(string travelId, CancellationToken cancellationToken = default);

        Task AddAsync(Checklist checklist, CancellationToken cancellationToken = default);

        Task UpdateAsync(Checklist checklist, CancellationToken cancellationToken = default);

        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    }
}
