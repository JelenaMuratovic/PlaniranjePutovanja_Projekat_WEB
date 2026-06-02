using PlaniranjePutovanja.Common.DTOs.Expense;
using PlaniranjePutovanja.Common.DTOs.Travel;
using PlaniranjePutovanja.ExpenseService.Models;
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

        /// <summary>
        /// Dodaje sistemski generisani trosak iz aktivnosti u BudgetState
        /// </summary>
        Task AddSystemGeneratedExpenseAsync(
            Expense expense,
            decimal plannedBudget,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Azuriraj cenu sistemskog troska iz aktivnosti
        /// </summary>
        Task UpdateSystemGeneratedExpenseAsync(
            Expense expense,
            decimal oldAmount,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ukloni sistemski trosak iz aktivnosti iz BudgetState
        /// </summary>
        Task RemoveSystemGeneratedExpenseAsync(
            string travelId,
            string expenseId,
            CancellationToken cancellationToken = default);
    }
}
