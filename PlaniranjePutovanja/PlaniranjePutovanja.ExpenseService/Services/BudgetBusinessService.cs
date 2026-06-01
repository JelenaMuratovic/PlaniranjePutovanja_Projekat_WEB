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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task InitializeAsync(IEnumerable<TravelDto> travels)
        {
            // Kreiramo ili uzimamo existing ReliableCollection
            //_budgetCollection = await _stateManager.GetOrAddAsync<IReliableDictionary<string, BudgetState>>("budgets");
            var budgetCollection = await GetCollectionAsync();

            // Ucitavamo sve troskove iz baze za svako putovanje
            foreach (var travel in travels)
            {
                using var tx = _stateManager.CreateTransaction();
                var currentBudgetResult = await budgetCollection.TryGetValueAsync(tx, travel.Id);

                // Ako vec imamo stanje u recniku, preskacemo da ne bismo pregazili najnovije stanje u memoriji
                if (!currentBudgetResult.HasValue)
                {
                    // Iz baze citamo istorijske troskove za ovo putovanje (vratice praznu listu ako ih nema)
                    var expenses = await _expenseRepository.GetByTravelIdAsync(travel.Id);

                    var budgetState = new BudgetState
                    {
                        TravelId = travel.Id,
                        PlannedBudget = (decimal)travel.Budget,
                        Expenses = expenses.ToList(),
                        LastUpdated = DateTime.UtcNow
                    };

                    //await _budgetCollection.AddAsync(tx, travel.Id, budgetState);
                    await budgetCollection.AddAsync(tx, travel.Id, budgetState);

                }
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
            var travelFounded = await _travelServiceClient.GetTravelByIdAsync(travelId);
            if (travelFounded == null)
            {
                throw new KeyNotFoundException($"Travel with id '{travelId}' was not found.");
            }

            // Trajni upis u SQL bazu
            var expense = _expenseMapper.ToExpense(dto);
            expense.TravelId = travelId;
            await _expenseRepository.AddAsync(expense, cancellationToken);

            //var budgetCollection = await _stateManager.GetOrAddAsync<IReliableDictionary<string, BudgetState>>("budgets");
            var budgetCollection = await GetCollectionAsync();
            using var tx = _stateManager.CreateTransaction();

            // Osiguravamo se LockMode.Update da niko drugi ne menja ovaj kljuc dok mi radimo upis
            var budgetResult = await budgetCollection.TryGetValueAsync(tx, travelId, LockMode.Update);

            BudgetState updatedBudgetState;

            if (budgetResult.HasValue)
            {
                // Ako putovanje vec postoji u recniku
                var oldBudgetState = budgetResult.Value;

                // Pravimo potpuno novi objekat radi bezbednosti memorije
                updatedBudgetState = new BudgetState
                {
                    TravelId = oldBudgetState.TravelId,
                    PlannedBudget = oldBudgetState.PlannedBudget,
                    LastUpdated = DateTime.UtcNow,
                    Expenses = new List<Expense>(oldBudgetState.Expenses) { expense } // stara lista + novi element
                };
            }
            else
            {
                // Ako je putovanje kreirano naknadno i nema ga u recniku
                var travel = await _travelServiceClient.GetTravelByIdAsync(travelId);
                decimal plannedBudget = travel != null ? (decimal)travel.Budget : 0;

                updatedBudgetState = new BudgetState
                {
                    TravelId = travelId,
                    PlannedBudget = plannedBudget,
                    Expenses = new List<Expense> { expense }, // Ovo nam je ujedno i prvi trosak
                    LastUpdated = DateTime.UtcNow
                };
            }

            // Upisujemo nazad u recnik (bilo da je update ili novi add)
            await budgetCollection.SetAsync(tx, travelId, updatedBudgetState);
            await tx.CommitAsync();

            return _expenseMapper.ToExpenseDto(expense);
        }

        public async Task<TravelBudgetSummaryDto?> GetBudgetSummaryAsync(string travelId, CancellationToken cancellationToken = default)
        {
            //var budgetCollection = await _stateManager.GetOrAddAsync<IReliableDictionary<string, BudgetState>>("budgets");
            var budgetCollection = await GetCollectionAsync();
            using var tx = _stateManager.CreateTransaction();

            var budgetResult = await budgetCollection.TryGetValueAsync(tx, travelId);

            // Ako korisnik trazi stanje za novo putovanje za koje jos nema troskova
            if (!budgetResult.HasValue)
            {
                var travel = await _travelServiceClient.GetTravelByIdAsync(travelId);
                if (travel == null)
                {
                    throw new KeyNotFoundException($"Travel with id '{travelId}' was not found.");
                }
                decimal plannedBudget = (decimal)travel.Budget;

                // Inicijalizujemo prazno stanje 
                using var innerTx = _stateManager.CreateTransaction();
                var newState = new BudgetState { TravelId = travelId, PlannedBudget = plannedBudget, Expenses = new List<Expense>() };
                await budgetCollection.AddAsync(innerTx, travelId, newState);
                await innerTx.CommitAsync();

                return _expenseMapper.ToBudgetSummaryDto(newState);
            }

            return _expenseMapper.ToBudgetSummaryDto(budgetResult.Value);
        }

        public async Task<IEnumerable<ExpenseDto>> GetExpensesByTravelIdAsync(string travelId, CancellationToken cancellationToken = default)
        {
            //var budgetCollection = await _stateManager.GetOrAddAsync<IReliableDictionary<string, BudgetState>>("budgets");
            var budgetCollection = await GetCollectionAsync();
            using var tx = _stateManager.CreateTransaction();

            var budgetResult = await budgetCollection.TryGetValueAsync(tx, travelId);
            if (!budgetResult.HasValue)
            {
                // Ako ga nema u recniku, probamo iz baze
                var dbExpenses = await _expenseRepository.GetByTravelIdAsync(travelId, cancellationToken);
                return dbExpenses.Select(_expenseMapper.ToExpenseDto).ToList();
            }

            return budgetResult.Value.Expenses
                .Select(_expenseMapper.ToExpenseDto)
                .ToList();
        }

        public async Task<bool> DeleteExpenseAsync(string travelId, string expenseId, CancellationToken cancellationToken = default)
        {
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

            var budgetResult = await budgetCollection.TryGetValueAsync(tx, expense.TravelId, LockMode.Update);
            if (budgetResult.HasValue)
            {
                var oldState = budgetResult.Value;
                var updatedState = new BudgetState
                {
                    TravelId = oldState.TravelId,
                    PlannedBudget = oldState.PlannedBudget,
                    LastUpdated = DateTime.UtcNow,
                    Expenses = oldState.Expenses.Where(e => e.Id != expenseId).ToList() // kopiramo sve OSIM obrisanog
                };

                await budgetCollection.SetAsync(tx, expense.TravelId, updatedState);
            }

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
