using PlaniranjePutovanja.TravelService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Repositories
{
    public interface ITravelRepository
    {
        Task<Travel?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<IEnumerable<Travel>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Travel>> GetAllAsync(CancellationToken cancellationToken = default);

        Task AddAsync(Travel travel, CancellationToken cancellationToken = default);

        Task UpdateAsync(Travel travel, CancellationToken cancellationToken = default);

        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    }
}
