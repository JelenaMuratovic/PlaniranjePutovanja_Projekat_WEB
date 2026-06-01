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
    public sealed class TravelRepository : ITravelRepository
    {
        private readonly TravelDbContext _dbContext;

        public TravelRepository(TravelDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Travel?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Travels
                .Include(t => t.Destinations).ThenInclude(d => d.Activities)
                .Include(t => t.Checklists)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Travel>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Travels
            // Ukljucujemo destinacije i za svaku destinaciju njene aktivnosti
            .Include(t => t.Destinations)
            .ThenInclude(d => d.Activities)
            // Ukljucujemo checkliste koje pripadaju tom putovanju
            .Include(t => t.Checklists)
            // Filtriramo samo aktivna putovanja za tog konkretnog korisnika
            .Where(t => t.UserId == userId && t.IsActive)
            // Sortiramo da najnovija kreirana putovanja budu prva
            .OrderByDescending(t => t.CreatedAt)
            // Izvrsavamo upit i pretvaramo u listu
           .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Travel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Travels
                .Where(t => t.IsActive)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Travel travel, CancellationToken cancellationToken = default)
        {
            await _dbContext.Travels.AddAsync(travel, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Travel travel, CancellationToken cancellationToken = default)
        {
            _dbContext.Travels.Update(travel);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var travel = await _dbContext.Travels.FindAsync(new object[] { id }, cancellationToken);
            if (travel != null)
            {
                _dbContext.Travels.Remove(travel);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
