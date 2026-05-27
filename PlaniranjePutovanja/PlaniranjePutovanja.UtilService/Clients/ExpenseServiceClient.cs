using Microsoft.ServiceFabric.Services.Remoting.Client;
using PlaniranjePutovanja.Common.DTOs.Expense;
using PlaniranjePutovanja.Common.Interfaces.Expense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.UtilService.Clients
{
    public sealed class ExpenseServiceClient : IExpenseServiceClient
    {
        private readonly Uri _expenseServiceUri = new("fabric:/PlaniranjePutovanja/PlaniranjePutovanja.ExpenseService");

        private IExpenseService GetProxy()
        {
            return ServiceProxy.Create<IExpenseService>(_expenseServiceUri, new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(0));
        }

        public async Task<TravelBudgetSummaryDto?> GetBudgetSummaryAsync(string travelId)
        {
            try
            {
                var proxy = GetProxy();
                return await proxy.GetBudgetSummaryAsync(travelId);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
