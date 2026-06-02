using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.ExpenseService.Services
{
    public interface IActivityExpenseService
    {
        /// <summary>
        /// Kada se dodaje aktivnost sa cenom, kreira odgovarajuci sistemski trosak
        /// </summary>
        Task<string> CreateActivityExpenseAsync(
            string travelId,
            string activityId,
            string activityName,
            decimal price,
            decimal plannedBudget,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kada se menja cena aktivnosti, azurira sistemski trosak
        /// </summary>
        Task<bool> UpdateActivityExpenseAsync(
            string travelId,
            string activityId,
            decimal newPrice,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kada se obrise aktivnost, obrise njen sistemski trosak
        /// </summary>
        Task<bool> DeleteActivityExpenseAsync(
            string travelId,
            string activityId,
            CancellationToken cancellationToken = default);
    }
}
