using PlaniranjePutovanja.Common.Enums;
using PlaniranjePutovanja.ExpenseService.Models;
using PlaniranjePutovanja.ExpenseService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.ExpenseService.Services
{
    public sealed class ActivityExpenseService : IActivityExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IBudgetBusinessService _budgetBusinessService;

        public ActivityExpenseService(IExpenseRepository expenseRepository, IBudgetBusinessService budgetBusinessService)
        {
            _expenseRepository = expenseRepository;
            _budgetBusinessService = budgetBusinessService;
        }

        public async Task<string> CreateActivityExpenseAsync(
            string travelId,
            string activityId,
            string activityName,
            decimal price,
            decimal plannedBudget,
            CancellationToken cancellationToken = default)
        {
            // Kreira sistemski trosak
            var expense = new Expense
            {
                Id = Guid.NewGuid().ToString(),
                TravelId = travelId,
                Name = $"Activity: {activityName}",
                ActivityId = activityId,
                Amount = price,
                Description = $"[System] Activity: {activityName}",
                Category = ExpenseCategory.Activities,
                ExpenseDate = DateTime.UtcNow,
                IsSystemGenerated = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Upisemo u bazu
            await _expenseRepository.AddAsync(expense, cancellationToken);

            // Azuriramo BudgetState
            await _budgetBusinessService.AddSystemGeneratedExpenseAsync(expense, plannedBudget, cancellationToken);

            return expense.Id;
        }

        public async Task<bool> UpdateActivityExpenseAsync(
           string travelId,
           string activityId,
           decimal newPrice,
           CancellationToken cancellationToken = default)
        {
            // Pronadjemo postojeci sistemski trosak za ovu aktivnost
            var expenses = await _expenseRepository.GetByActivityIdAsync(activityId, cancellationToken);
            var activityExpense = expenses.FirstOrDefault();

            if (activityExpense == null)
            {
                // Ako ne postoji, kreiraj novi (trebalo bi da bude prva-ako-nema scenario)
                return false;
            }

            var oldPrice = activityExpense.Amount;
            activityExpense.Amount = newPrice;
            activityExpense.UpdatedAt = DateTime.UtcNow;

            // Azuriramo u bazi
            await _expenseRepository.UpdateAsync(activityExpense, cancellationToken);

            // Azuriramo BudgetState sa novom cenom
            await _budgetBusinessService.UpdateSystemGeneratedExpenseAsync(activityExpense, oldPrice, cancellationToken);

            return true;
        }

        public async Task<bool> DeleteActivityExpenseAsync(
           string travelId,
           string activityId,
           CancellationToken cancellationToken = default)
        {
            // Pronadjemo postojeci sistemski trosak za ovu aktivnost
            var expenses = await _expenseRepository.GetByActivityIdAsync(activityId, cancellationToken);
            var activityExpense = expenses.FirstOrDefault();

            if (activityExpense == null)
            {
                return false;
            }

            // Obrisemo iz baze
            await _expenseRepository.DeleteAsync(activityExpense.Id, cancellationToken);

            // Ukloni iz BudgetState
            await _budgetBusinessService.RemoveSystemGeneratedExpenseAsync(travelId, activityExpense.Id, cancellationToken);

            return true;
        }
    }
}
