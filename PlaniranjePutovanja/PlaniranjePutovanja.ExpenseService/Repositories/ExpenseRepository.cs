using Microsoft.EntityFrameworkCore;
using PlaniranjePutovanja.ExpenseService.Models;
using PlaniranjePutovanja.ExpenseService.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.ExpenseService.Repositories
{
    public sealed class ExpenseRepository : IExpenseRepository
    {
        private readonly ExpenseDbContext _dbContext;

        public ExpenseRepository(ExpenseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Expense?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Expenses
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Expense>> GetByTravelIdAsync(string travelId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Expenses
                .Where(e => e.TravelId == travelId)
                .OrderByDescending(e => e.ExpenseDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Expense>> GetByActivityIdAsync(string activityId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Expenses
                .Where(e => e.ActivityId == activityId && e.IsSystemGenerated)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Expense expense, CancellationToken cancellationToken = default)
        {
            await _dbContext.Expenses.AddAsync(expense, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Expense expense, CancellationToken cancellationToken = default)
        {
            _dbContext.Expenses.Update(expense);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task AddOrUpdateAsync(Expense expense, CancellationToken cancellationToken = default)
        {
            var existing = await _dbContext.Expenses
                .FirstOrDefaultAsync(e => e.Id == expense.Id, cancellationToken);

            if (existing != null)
            {
                existing.Name = expense.Name;
                existing.Amount = expense.Amount;
                existing.Category = expense.Category;
                existing.ExpenseDate = expense.ExpenseDate;
                existing.Description = expense.Description;
                existing.UpdatedAt = DateTime.UtcNow;

                _dbContext.Expenses.Update(existing);
            }
            else
            {
                await _dbContext.Expenses.AddAsync(expense, cancellationToken);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var expense = await _dbContext.Expenses.FindAsync(new object[] { id }, cancellationToken);
            if (expense != null)
            {
                _dbContext.Expenses.Remove(expense);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
