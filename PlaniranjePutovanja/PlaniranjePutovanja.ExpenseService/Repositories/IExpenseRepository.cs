using PlaniranjePutovanja.ExpenseService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.ExpenseService.Repositories
{
    public interface IExpenseRepository
    {
        Task<Expense?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<IEnumerable<Expense>> GetByTravelIdAsync(string travelId, CancellationToken cancellationToken = default);

        Task AddAsync(Expense expense, CancellationToken cancellationToken = default);

        Task AddOrUpdateAsync(Expense expense, CancellationToken cancellationToken = default);

        Task UpdateAsync(Expense expense, CancellationToken cancellationToken = default);

        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    }
}
