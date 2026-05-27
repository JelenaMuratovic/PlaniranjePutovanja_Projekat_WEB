using PlaniranjePutovanja.Common.DTOs.Expense;
using PlaniranjePutovanja.Common.DTOs.Travel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.ExpenseService.Services
{
    public interface IBudgetBusinessService
    {
        Task InitializeAsync(IEnumerable<TravelDto> travels);

        Task<ExpenseDto> AddExpenseAsync(string travelId, CreateExpenseDto dto, CancellationToken cancellationToken = default);

        Task<TravelBudgetSummaryDto?> GetBudgetSummaryAsync(string travelId, CancellationToken cancellationToken = default);

        Task<IEnumerable<ExpenseDto>> GetExpensesByTravelIdAsync(string travelId, CancellationToken cancellationToken = default);

        Task<bool> DeleteExpenseAsync(string travelId, string expenseId, CancellationToken cancellationToken = default);

        //Task PersistBudgetsAsync(CancellationToken cancellationToken = default);

        Task<bool> DeleteExpensesByTravelIdAsync(string travelId, CancellationToken cancellationToken = default);
    }
}
