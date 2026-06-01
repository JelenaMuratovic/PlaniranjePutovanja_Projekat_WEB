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
    public sealed class DestinationRepository : IDestinationRepository
    {
        private readonly TravelDbContext _dbContext;

        public DestinationRepository(TravelDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Destination?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Destinations
                .Include(d => d.Activities)
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Destination>> GetByTravelIdAsync(string travelId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Destinations
            // Ukljucujemo aktivnosti za svaku destinaciju u listi
            .Include(d => d.Activities)
            // Filtriramo destinacije koje pripadaju iskljucivo ovom putovanju
            .Where(d => d.TravelId == travelId)
            // Sortiramo ih po vremenu kreiranja
            .OrderBy(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Destination destination, CancellationToken cancellationToken = default)
        {
            await _dbContext.Destinations.AddAsync(destination, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Destination destination, CancellationToken cancellationToken = default)
        {
            _dbContext.Destinations.Update(destination);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var destination = await _dbContext.Destinations.FindAsync(new object[] { id }, cancellationToken);
            if (destination != null)
            {
                _dbContext.Destinations.Remove(destination);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
