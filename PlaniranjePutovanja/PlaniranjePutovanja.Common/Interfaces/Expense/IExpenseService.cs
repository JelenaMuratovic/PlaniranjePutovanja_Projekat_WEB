using Microsoft.ServiceFabric.Services.Remoting;
using PlaniranjePutovanja.Common.DTOs.Expense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.Common.Interfaces.Expense
{
    public interface IExpenseService : IService
    {
        /// <summary>
        /// Dodaje novi trosak povezan s odredjenim putovanjem
        /// </summary>
        Task<ExpenseDto> AddExpenseAsync(string travelId, CreateExpenseDto request);

        /// <summary>
        /// Vraca sve troskove povezane s odredjenim putovanjem
        /// </summary>
        Task<IEnumerable<ExpenseDto>> GetExpensesByTravelIdAsync(string travelId);

        /// <summary>
        /// Vraca pregled ukupnog budzeta za odredjeno putovanje
        /// </summary>
        Task<TravelBudgetSummaryDto?> GetBudgetSummaryAsync(string travelId);

        /// <summary>
        /// Brise odredjeni trosak
        /// </summary>
        Task<bool> DeleteExpenseAsync(string travelId, string expenseId);

        /// <summary>
        /// Brise sve troskove povezane s odredjenim putovanjem
        /// </summary>
        Task<bool> DeleteExpensesByTravelIdAsync(string travelId);

        /// <summary>
        /// Kreira novi trosak aktivnosti povezan s odredjenim putovanjem i aktivnoscu
        /// </summary>
        Task<string> CreateActivityExpenseAsync(
        string travelId,
        string activityId,
        string activityName,
        decimal price,
        decimal plannedBudget);

        /// <summary>
        /// Azurira cenu troska aktivnosti povezanog s odredjenim putovanjem i aktivnoscu
        /// </summary>
        Task<bool> UpdateActivityExpenseAsync(
            string travelId,
            string activityId,
            decimal newPrice);

        /// <summary>
        /// Brise trosak aktivnosti povezanog s odredjenim putovanjem i aktivnoscu
        /// </summary>
        Task<bool> DeleteActivityExpenseAsync(
            string travelId,
            string activityId);
    }
}
