using PlaniranjePutovanja.TravelService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Repositories
{
    public interface IDestinationRepository
    {
        Task<Destination?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<IEnumerable<Destination>> GetByTravelIdAsync(string travelId, CancellationToken cancellationToken = default);

        Task AddAsync(Destination destination, CancellationToken cancellationToken = default);

        Task UpdateAsync(Destination destination, CancellationToken cancellationToken = default);

        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    }
}
