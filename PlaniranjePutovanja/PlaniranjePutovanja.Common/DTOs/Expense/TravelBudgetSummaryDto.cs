using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.Common.DTOs.Expense
{
    public sealed class TravelBudgetSummaryDto
    {
        public string TravelId { get; set; } = string.Empty;

        public decimal PlannedBudget { get; set; }

        public decimal TotalExpenses { get; set; }

        public decimal RemainingBudget { get; set; }

        public decimal SpentPercentage { get; set; }

        public int ExpenseCount { get; set; }
    }
}
