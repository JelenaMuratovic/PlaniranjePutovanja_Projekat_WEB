using PlaniranjePutovanja.Common.DTOs.Expense;
using PlaniranjePutovanja.ExpenseService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.ExpenseService.Mappers
{
    public interface IExpenseMapper
    {
        Expense ToExpense(CreateExpenseDto dto);

        ExpenseDto ToExpenseDto(Expense expense);

        TravelBudgetSummaryDto ToBudgetSummaryDto(BudgetState budgetState);
    }
}
