using PlaniranjePutovanja.Common.DTOs.Expense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.UtilService.Clients
{
    public interface IExpenseServiceClient
    {
        Task<TravelBudgetSummaryDto?> GetBudgetSummaryAsync(string travelId);
    }
}
