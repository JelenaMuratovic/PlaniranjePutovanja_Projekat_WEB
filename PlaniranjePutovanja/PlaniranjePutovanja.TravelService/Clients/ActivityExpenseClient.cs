using Microsoft.ServiceFabric.Services.Remoting.Client;
using PlaniranjePutovanja.Common.Interfaces.Expense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.TravelService.Clients
{
    public sealed class ActivityExpenseClient : IActivityExpenseClient
    {
        private readonly Uri _expenseServiceUri = new Uri("fabric:/PlaniranjePutovanja/PlaniranjePutovanja.ExpenseService");

        private IExpenseService GetProxy()
        {
            return ServiceProxy.Create<IExpenseService>(_expenseServiceUri, new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(0));
        }

        public async Task<string> CreateActivityExpenseAsync(
            string travelId,
            string activityId,
            string activityName,
            decimal price,
            decimal plannedBudget)
        {
            try
            {
                var proxy = GetProxy();

                var expenseId = await proxy.CreateActivityExpenseAsync(travelId, activityId, activityName, price, plannedBudget);
                return expenseId;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to create activity expense for activity '{activityId}'.", ex);
            }
        }

        public async Task<bool> UpdateActivityExpenseAsync(
            string travelId,
            string activityId,
            decimal newPrice)
        {
            try
            {
                var proxy = GetProxy();

                return await proxy.UpdateActivityExpenseAsync(travelId, activityId, newPrice);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to update activity expense for activity '{activityId}'.", ex);
            }
        }

        public async Task<bool> DeleteActivityExpenseAsync(
            string travelId,
            string activityId)
        {
            try
            {
                var proxy = GetProxy();

                return await proxy.DeleteActivityExpenseAsync(travelId, activityId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to delete activity expense for activity '{activityId}'.", ex);
            }
        }
    }
}
