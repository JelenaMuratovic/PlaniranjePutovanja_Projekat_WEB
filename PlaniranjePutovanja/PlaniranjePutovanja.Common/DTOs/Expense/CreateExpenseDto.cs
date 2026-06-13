using PlaniranjePutovanja.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.Common.DTOs.Expense
{
    public sealed class CreateExpenseDto
    {
        public string Name { get; set; } = string.Empty;

        public ExpenseCategory Category { get; set; } = ExpenseCategory.Other;

        public decimal Amount { get; set; }

        public DateTime ExpenseDate { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}
