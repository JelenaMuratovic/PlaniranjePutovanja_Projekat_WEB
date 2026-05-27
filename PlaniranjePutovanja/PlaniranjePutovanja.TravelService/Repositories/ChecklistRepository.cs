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
    public sealed class ChecklistRepository : IChecklistRepository
    {
        private readonly TravelDbContext _dbContext;

        public ChecklistRepository(TravelDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Checklist?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Checklists
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Checklist>> GetByTravelIdAsync(string travelId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Checklists
                .Where(c => c.TravelId == travelId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Checklist checklist, CancellationToken cancellationToken = default)
        {
            await _dbContext.Checklists.AddAsync(checklist, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Checklist checklist, CancellationToken cancellationToken = default)
        {
            _dbContext.Checklists.Update(checklist);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var checklist = await _dbContext.Checklists.FindAsync(new object[] { id }, cancellationToken);
            if (checklist != null)
            {
                _dbContext.Checklists.Remove(checklist);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
