using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.ExpenseService.Models
{
    [DataContract]
    public class BudgetState
    {
        [DataMember]
        public string TravelId { get; set; } = string.Empty;
        [DataMember]
        public decimal PlannedBudget { get; set; }
        [DataMember]
        public List<Expense> Expenses { get; set; } = new();

        public decimal TotalSpent => Expenses.Sum(e => e.Amount);

        public decimal Remaining => PlannedBudget - TotalSpent;

        public decimal SpentPercentage => PlannedBudget > 0
            ? (TotalSpent / PlannedBudget) * 100
            : 0;
        [DataMember]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
