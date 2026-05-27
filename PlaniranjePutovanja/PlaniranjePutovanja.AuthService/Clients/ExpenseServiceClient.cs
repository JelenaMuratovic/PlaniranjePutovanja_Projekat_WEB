using Microsoft.ServiceFabric.Services.Remoting.Client;
using PlaniranjePutovanja.Common.Interfaces.Expense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.AuthService.Clients
{
    public sealed class ExpenseServiceClient : IExpenseServiceClient
    {
        private readonly Uri _expenseServiceUri = new("fabric:/PlaniranjePutovanja/PlaniranjePutovanja.ExpenseService");

        public async Task<bool> DeleteExpensesByTravelIdAsync(string travelId)
        {
            var proxy = ServiceProxy.Create<IExpenseService>(_expenseServiceUri, new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(0));
            return await proxy.DeleteExpensesByTravelIdAsync(travelId);
        }
    }
}
