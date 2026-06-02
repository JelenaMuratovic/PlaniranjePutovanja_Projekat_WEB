using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Clients
{
    public interface IActivityExpenseClient
    {
        Task<string> CreateActivityExpenseAsync(
            string travelId,
            string activityId,
            string activityName,
            decimal price,
            decimal plannedBudget);

        Task<bool> UpdateActivityExpenseAsync(
            string travelId,
            string activityId,
            decimal newPrice);

        Task<bool> DeleteActivityExpenseAsync(
            string travelId,
            string activityId);
    }
}
