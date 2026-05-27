using Microsoft.EntityFrameworkCore;
using PlaniranjePutovanja.TravelService.Models;
using PlaniranjePutovanja.TravelService.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Repositories
{
    public sealed class ActivityRepository : IActivityRepository
    {
        private readonly TravelDbContext _dbContext;

        public ActivityRepository(TravelDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Activity?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Activities
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Activity>> GetByDestinationIdAsync(string destinationId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Activities
                .Where(a => a.DestinationId == destinationId)
                .OrderBy(a => a.ActivityDate)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Activity activity, CancellationToken cancellationToken = default)
        {
            await _dbContext.Activities.AddAsync(activity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Activity activity, CancellationToken cancellationToken = default)
        {
            _dbContext.Activities.Update(activity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var activity = await _dbContext.Activities.FindAsync(new object[] { id }, cancellationToken);
            if (activity != null)
            {
                _dbContext.Activities.Remove(activity);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
