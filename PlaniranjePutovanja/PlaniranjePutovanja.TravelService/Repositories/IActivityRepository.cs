using PlaniranjePutovanja.TravelService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Repositories
{
    public interface IActivityRepository
    {
        Task<Activity?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<IEnumerable<Activity>> GetByDestinationIdAsync(string destinationId, CancellationToken cancellationToken = default);

        Task AddAsync(Activity activity, CancellationToken cancellationToken = default);

        Task UpdateAsync(Activity activity, CancellationToken cancellationToken = default);

        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    }

}