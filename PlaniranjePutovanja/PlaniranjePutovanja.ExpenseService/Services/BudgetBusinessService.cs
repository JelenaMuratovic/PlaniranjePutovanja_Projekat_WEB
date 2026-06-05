using FluentValidation;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using PlaniranjePutovanja.Common.DTOs.Expense;
using PlaniranjePutovanja.Common.DTOs.Travel;
using PlaniranjePutovanja.ExpenseService.Clients;
using PlaniranjePutovanja.ExpenseService.Mappers;
using PlaniranjePutovanja.ExpenseService.Models;
using PlaniranjePutovanja.ExpenseService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.ExpenseService.Services
{
    public sealed class BudgetBusinessService : IBudgetBusinessService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IExpenseMapper _expenseMapper;
        private readonly IValidator<CreateExpenseDto> _createExpenseValidator;
        private readonly IReliableStateManager _stateManager;
        private readonly ITravelServiceClient _travelServiceClient;

        //private IReliableDictionary<string, BudgetState> _budgetCollection;
        private const string DictionaryName = "budgets";

        public BudgetBusinessService(
        IExpenseRepository expenseRepository,
        IExpenseMapper expenseMapper,
        IValidator<CreateExpenseDto> createExpenseValidator,
        IReliableStateManager stateManager,
        ITravelServiceClient travelServiceClient)
        {
            _expenseRepository = expenseRepository;
            _expenseMapper = expenseMapper;
            _createExpenseValidator = createExpenseValidator;
            _stateManager = stateManager;
            _travelServiceClient = travelServiceClient;
        }

        private async Task<IReliableDictionary<string, BudgetState>> GetCollectionAsync()
        {
            return await _stateManager.GetOrAddAsync<IReliableDictionary<string, BudgetState>>(DictionaryName);
        }

        private async Task<TravelDto> GetExistingTravelAsync(string travelId)
        {
            var travel = await _travelServiceClient.GetTravelByIdAsync(travelId);
            if (travel == null)
            {
                throw new KeyNotFoundException($"Travel with id '{travelId}' was not found.");
            }

            return travel;
        }

        private async Task<BudgetState> BuildStateFromSourceAsync(
            string travelId,
            decimal plannedBudget,
            CancellationToken cancellationToken = default)
        {
            var expenses = await _expenseRepository.GetByTravelIdAsync(travelId, cancellationToken);

            return new BudgetState
            {
                TravelId = travelId,
                PlannedBudget = plannedBudget,
                Expenses = expenses.ToList(),
                LastUpdated = DateTime.UtcNow
            };
        }

        private async Task<BudgetState> RefreshStateAsync(
            IReliableDictionary<string, BudgetState> budgetCollection,
            ITransaction tx,
            TravelDto travel,
            CancellationToken cancellationToken = default)
        {
            var state = await BuildStateFromSourceAsync(travel.Id, travel.Budget, cancellationToken);
            await budgetCollection.SetAsync(tx, travel.Id, state);
            return state;
        }

        private async Task<BudgetState> RefreshStateAsync(
            IReliableDictionary<string, BudgetState> budgetCollection,
            ITransaction tx,
            string travelId,
            decimal plannedBudget,
            CancellationToken cancellationToken = default)
        {
            var state = await BuildStateFromSourceAsync(travelId, plannedBudget, cancellationToken);
            await budgetCollection.SetAsync(tx, travelId, state);
            return state;
        }

        public async Task InitializeAsync(IEnumerable<TravelDto> travels)
        {
            var budgetCollection = await GetCollectionAsync();

            foreach (var travel in travels)
            {
                using var tx = _stateManager.CreateTransaction();
                await RefreshStateAsync(budgetCollection, tx, travel);
                await tx.CommitAsync();
            }
        }

        public async Task<ExpenseDto> AddExpenseAsync(string travelId, CreateExpenseDto dto, CancellationToken cancellationToken = default)
        {
            // Validacija ulaza
            var validationResult = await _createExpenseValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
            var travel = await GetExistingTravelAsync(travelId);

            // Trajni upis u SQL bazu
            var expense = _expenseMapper.ToExpense(dto);
            expense.TravelId = travelId;
            await _expenseRepository.AddAsync(expense, cancellationToken);

            //var budgetCollection = await _stateManager.GetOrAddAsync<IReliableDictionary<string, BudgetState>>("budgets");
            var budgetCollection = await GetCollectionAsync();
            using var tx = _stateManager.CreateTransaction();

            await budgetCollection.TryGetValueAsync(tx, travelId, LockMode.Update);
            await RefreshStateAsync(budgetCollection, tx, travel, cancellationToken);
            await tx.CommitAsync();

            return _expenseMapper.ToExpenseDto(expense);
        }

        public async Task<TravelBudgetSummaryDto?> GetBudgetSummaryAsync(string travelId, CancellationToken cancellationToken = default)
        {
            //var budgetCollection = await _stateManager.GetOrAddAsync<IReliableDictionary<string, BudgetState>>("budgets");
            var budgetCollection = await GetCollectionAsync();
            using var tx = _stateManager.CreateTransaction();

            var travel = await GetExistingTravelAsync(travelId);
            await budgetCollection.TryGetValueAsync(tx, travelId, LockMode.Update);
            var state = await RefreshStateAsync(budgetCollection, tx, travel, cancellationToken);
            await tx.CommitAsync();

            return _expenseMapper.ToBudgetSummaryDto(state);
        }

        public async Task<IEnumerable<ExpenseDto>> GetExpensesByTravelIdAsync(string travelId, CancellationToken cancellationToken = default)
        {
            //var budgetCollection = await _stateManager.GetOrAddAsync<IReliableDictionary<string, BudgetState>>("budgets");
            var budgetCollection = await GetCollectionAsync();
            using var tx = _stateManager.CreateTransaction();

            var travel = await GetExistingTravelAsync(travelId);
            await budgetCollection.TryGetValueAsync(tx, travelId, LockMode.Update);
            var state = await RefreshStateAsync(budgetCollection, tx, travel, cancellationToken);
            await tx.CommitAsync();

            return state.Expenses
                .Select(_expenseMapper.ToExpenseDto)
                .ToList();
        }

        public async Task<bool> DeleteExpenseAsync(string travelId, string expenseId, CancellationToken cancellationToken = default)
        {
            var travel = await GetExistingTravelAsync(travelId);
            var expense = await _expenseRepository.GetByIdAsync(expenseId, cancellationToken);
            if (expense == null) return false;

            if (expense.TravelId != travelId)
            {
                throw new UnauthorizedAccessException("The expense doesn't belong to the specified travel.");
            }

            // Obrisemo iz baze
            await _expenseRepository.DeleteAsync(expenseId, cancellationToken);

            // Obrisemo iz recnika
            //var budgetCollection = await _stateManager.GetOrAddAsync<IReliableDictionary<string, BudgetState>>("budgets");
            var budgetCollection = await GetCollectionAsync();
            using var tx = _stateManager.CreateTransaction();

            await budgetCollection.TryGetValueAsync(tx, expense.TravelId, LockMode.Update);
            await RefreshStateAsync(budgetCollection, tx, travel, cancellationToken);

            await tx.CommitAsync();
            return true;
        }

        public async Task<bool> DeleteExpensesByTravelIdAsync(string travelId, CancellationToken cancellationToken = default)
        {
            var travel = await _travelServiceClient.GetTravelByIdAsync(travelId);
            if (travel == null)
            {
                throw new KeyNotFoundException($"Travel with id '{travelId}' was not found.");
            }
            var expenses = await _expenseRepository.GetByTravelIdAsync(travelId, cancellationToken);
            foreach (var exp in expenses)
            {
                await _expenseRepository.DeleteAsync(exp.Id, cancellationToken);
            }

            var budgetCollection = await GetCollectionAsync();
            using var tx = _stateManager.CreateTransaction();
            // Brisanje celog putovanja iz in-memory recnika
            await budgetCollection.TryRemoveAsync(tx, travelId);
            await tx.CommitAsync();

            return true;
        }

        public async Task AddSystemGeneratedExpenseAsync(Expense expense, decimal plannedBudget, CancellationToken cancellationToken = default)
        {
            var budgetCollection = await GetCollectionAsync();
            using var tx = _stateManager.CreateTransaction();

            await budgetCollection.TryGetValueAsync(tx, expense.TravelId, LockMode.Update);
            await RefreshStateAsync(budgetCollection, tx, expense.TravelId, plannedBudget, cancellationToken);
            await tx.CommitAsync();
        }

        public async Task UpdateSystemGeneratedExpenseAsync(Expense expense, decimal oldAmount, CancellationToken cancellationToken = default)
        {
            var travel = await GetExistingTravelAsync(expense.TravelId);
            var budgetCollection = await GetCollectionAsync();
            using var tx = _stateManager.CreateTransaction();

            await budgetCollection.TryGetValueAsync(tx, expense.TravelId, LockMode.Update);
            await RefreshStateAsync(budgetCollection, tx, travel, cancellationToken);

            await tx.CommitAsync();
        }

        public async Task RemoveSystemGeneratedExpenseAsync(string travelId, string expenseId, CancellationToken cancellationToken = default)
        {
            var travel = await GetExistingTravelAsync(travelId);
            var budgetCollection = await GetCollectionAsync();
            using var tx = _stateManager.CreateTransaction();

            await budgetCollection.TryGetValueAsync(tx, travelId, LockMode.Update);
            await RefreshStateAsync(budgetCollection, tx, travel, cancellationToken);

            await tx.CommitAsync();
        }

        //public async Task PersistBudgetsAsync(CancellationToken cancellationToken = default)
        //{
        //    // Periodically ili na shutdown — sprema sve iz memorije u bazu
        //    using var tx = _stateManager.CreateTransaction();

        //    var enumerable = await _budgetCollection.CreateEnumerableAsync(tx);

        //    using (var enumerator = enumerable.GetAsyncEnumerator())
        //    {
        //        while (await enumerator.MoveNextAsync(cancellationToken))
        //        {
        //            var budgetState = enumerator.Current.Value;

        //            // Sve troskove spremi u bazu
        //            foreach (var expense in budgetState.Expenses)
        //            {
        //                await _expenseRepository.AddOrUpdateAsync(expense, cancellationToken);
        //            }
        //        }
        //    }
        //}
    }
}
