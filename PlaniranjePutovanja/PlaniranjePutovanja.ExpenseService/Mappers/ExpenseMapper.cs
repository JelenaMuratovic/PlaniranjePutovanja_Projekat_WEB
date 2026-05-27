using PlaniranjePutovanja.Common.DTOs.Expense;
using PlaniranjePutovanja.ExpenseService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.ExpenseService.Mappers
{
    public sealed class ExpenseMapper : IExpenseMapper
    {
        public Expense ToExpense(CreateExpenseDto dto)
        {
            return new Expense
            {
                Name = dto.Name,
                Category = dto.Category,
                Amount = dto.Amount,
                ExpenseDate = dto.ExpenseDate,
                Description = dto.Description
            };
        }

        public ExpenseDto ToExpenseDto(Expense expense)
        {
            return new ExpenseDto
            {
                Id = expense.Id,
                TravelId = expense.TravelId,
                Name = expense.Name,
                Category = expense.Category.ToString(),
                Amount = expense.Amount,
                ExpenseDate = expense.ExpenseDate,
                Description = expense.Description,
                CreatedAt = expense.CreatedAt
            };
        }

        public TravelBudgetSummaryDto ToBudgetSummaryDto(BudgetState budgetState)
        {
            return new TravelBudgetSummaryDto
            {
                TravelId = budgetState.TravelId,
                PlannedBudget = budgetState.PlannedBudget,
                TotalExpenses = budgetState.TotalSpent,
                RemainingBudget = budgetState.Remaining,
                SpentPercentage = budgetState.SpentPercentage,
                ExpenseCount = budgetState.Expenses.Count
            };
        }
    }
}
