import { useEffect, useRef, useState, type FormEvent } from "react";
import { expenseApi } from "../../api/expense";
import { getApiErrorMessage } from "../../helpers/apiError";
import { formatDateOnly, formatDateOnlyForApi } from "../../helpers/date";
import type {
  CreateExpenseDto,
  ExpenseCategory,
  ExpenseDto,
  TravelBudgetSummaryDto,
} from "../../models/expense";

type TravelExpensePanelProps = {
  travelId: string;
  plannedBudget: number;
  refreshKey?: number;
  onRefresh?: () => Promise<void> | void;
};

type ExpenseCategoryOption = {
  value: number;
  label: string;
};

const expenseCategoryOptions: ExpenseCategoryOption[] = [
  { value: 0, label: "Transport" },
  { value: 1, label: "Accommodation" },
  { value: 2, label: "Food" },
  { value: 3, label: "Tickets" },
  { value: 4, label: "Shopping" },
  { value: 5, label: "Other" },
];

const formatMoney = (value: number): string =>
  value.toLocaleString("en-GB", {
    style: "currency",
    currency: "EUR",
  });

export const TravelExpensePanel = ({
  travelId,
  plannedBudget,
  refreshKey,
  onRefresh,
}: TravelExpensePanelProps) => {
  const [summary, setSummary] = useState<TravelBudgetSummaryDto | null>(null);
  const [expenses, setExpenses] = useState<ExpenseDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [notice, setNotice] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [expensePendingDelete, setExpensePendingDelete] =
    useState<ExpenseDto | null>(null);
  const [formState, setFormState] = useState<CreateExpenseDto>({
    name: "",
    category: 5 as unknown as ExpenseCategory,
    amount: 0,
    expenseDate: formatDateOnlyForApi(new Date()),
    description: "",
  });

  const noticeTimer = useRef<number | null>(null);
  const errorTimer = useRef<number | null>(null);

  const clearNotice = () => {
    if (noticeTimer.current) {
      window.clearTimeout(noticeTimer.current);
      noticeTimer.current = null;
    }
    setNotice(null);
  };

  const clearError = () => {
    if (errorTimer.current) {
      window.clearTimeout(errorTimer.current);
      errorTimer.current = null;
    }
    setError(null);
  };

  const showNotice = (message: string) => {
    if (noticeTimer.current) {
      window.clearTimeout(noticeTimer.current);
    }

    setNotice(message);
    noticeTimer.current = window.setTimeout(() => {
      setNotice(null);
      noticeTimer.current = null;
    }, 2600);
  };

  const showError = (message: string) => {
    if (errorTimer.current) {
      window.clearTimeout(errorTimer.current);
    }

    setError(message);
    errorTimer.current = window.setTimeout(() => {
      setError(null);
      errorTimer.current = null;
    }, 3200);
  };

  useEffect(
    () => () => {
      if (noticeTimer.current) {
        window.clearTimeout(noticeTimer.current);
      }
      if (errorTimer.current) {
        window.clearTimeout(errorTimer.current);
      }
    },
    [],
  );

  const loadBudgetAndExpenses = async () => {
    setIsLoading(true);
    clearError();

    try {
      const [budgetSummary, expenseList] = await Promise.all([
        expenseApi.getBudgetSummary(travelId),
        expenseApi.getExpensesByTravelId(travelId),
      ]);

      setSummary(budgetSummary);
      setExpenses(expenseList);
    } catch (loadError) {
      console.error("Failed to load budget data", loadError);
      showError("Budget data could not be loaded right now.");
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    setSummary(null);
    setExpenses([]);
    setExpensePendingDelete(null);
    setFormState({
      name: "",
      category: 5 as unknown as ExpenseCategory,
      amount: 0,
      expenseDate: formatDateOnlyForApi(new Date()),
      description: "",
    });
    clearNotice();
    clearError();

    void loadBudgetAndExpenses();
  }, [travelId, refreshKey]);

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (!formState.name.trim()) {
      return;
    }

    try {
      setIsSubmitting(true);
      await expenseApi.addExpense(travelId, {
        ...formState,
        name: formState.name.trim(),
        description: formState.description.trim(),
      });

      setFormState((current) => ({
        ...current,
        name: "",
        amount: 0,
        description: "",
        expenseDate: formatDateOnlyForApi(new Date()) || current.expenseDate,
        category: 5 as unknown as ExpenseCategory,
      }));

      showNotice("Expense added successfully.");
      await loadBudgetAndExpenses();
      if (onRefresh) {
        void onRefresh();
      }
    } catch (addError) {
      console.error("Failed to add expense", addError);
      showError(
        getApiErrorMessage(addError) ||
          "Expense could not be saved. Please try again.",
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  const confirmDeleteExpense = async () => {
    if (!expensePendingDelete) {
      return;
    }

    try {
      await expenseApi.deleteExpense(travelId, expensePendingDelete.id);
      setExpensePendingDelete(null);
      showNotice("Expense deleted successfully.");
      await loadBudgetAndExpenses();
      if (onRefresh) {
        void onRefresh();
      }
    } catch (deleteError) {
      console.error("Failed to delete expense", deleteError);
      showError(
        getApiErrorMessage(deleteError) ||
          "Expense could not be deleted. Please try again.",
      );
    }
  };

  const effectiveSummary = summary ?? {
    travelId,
    plannedBudget,
    totalExpenses: 0,
    remainingBudget: plannedBudget,
    spentPercentage: 0,
    expenseCount: 0,
  };

  return (
    <section className="expense-panel">
      <div className="section-heading section-heading--inline expense-panel__header">
        <div
          className={`expense-panel__badge ${effectiveSummary.remainingBudget < 0 ? "expense-panel__badge--warning" : ""}`}
        >
          <strong>{effectiveSummary.expenseCount}</strong>
          <span>expenses</span>
        </div>
      </div>

      <div className="expense-panel__summary-grid">
        <article className="travel-workspace__meta-card">
          <span>Planned budget</span>
          <strong>{formatMoney(effectiveSummary.plannedBudget)}</strong>
        </article>
        <article className="travel-workspace__meta-card">
          <span>Total spent</span>
          <strong>{formatMoney(effectiveSummary.totalExpenses)}</strong>
        </article>
        <article className="travel-workspace__meta-card">
          <span>Remaining budget</span>
          <strong
            className={
              effectiveSummary.remainingBudget < 0
                ? "expense-panel__negative"
                : ""
            }
          >
            {formatMoney(effectiveSummary.remainingBudget)}
          </strong>
        </article>
        <article className="travel-workspace__meta-card">
          <span>Spent percentage</span>
          <strong>{effectiveSummary.spentPercentage.toFixed(0)}%</strong>
        </article>
      </div>

      <div className="expense-panel__progress">
        <div className="expense-panel__progress-track" aria-hidden="true">
          <div
            className={`expense-panel__progress-fill ${effectiveSummary.remainingBudget < 0 ? "expense-panel__progress-fill--warning" : ""}`}
            style={{
              width: `${Math.min(100, Math.max(0, effectiveSummary.spentPercentage))}%`,
            }}
          />
        </div>
        <span>
          {formatMoney(effectiveSummary.totalExpenses)} spent from{" "}
          {formatMoney(effectiveSummary.plannedBudget)}
        </span>
      </div>

      <form className="expense-panel__form" onSubmit={handleSubmit}>
        <div className="form-grid form-grid--two">
          <div className="field">
            <label htmlFor="expense-name">Name</label>
            <input
              id="expense-name"
              value={formState.name}
              onChange={(event) =>
                setFormState((current) => ({
                  ...current,
                  name: event.target.value,
                }))
              }
              placeholder="Taxi, hotel, groceries..."
              required
            />
          </div>

          <div className="field">
            <label htmlFor="expense-category">Category</label>
            <select
              id="expense-category"
              value={formState.category}
              onChange={(event) =>
                setFormState((current) => ({
                  ...current,
                  category: Number(
                    event.target.value,
                  ) as unknown as ExpenseCategory,
                }))
              }
            >
              {expenseCategoryOptions.map((option) => (
                <option key={option.value} value={option.value}>
                  {option.label}
                </option>
              ))}
            </select>
          </div>
        </div>

        <div className="form-grid form-grid--two">
          <div className="field">
            <label htmlFor="expense-date">Date</label>
            <input
              id="expense-date"
              type="date"
              value={formState.expenseDate}
              onChange={(event) =>
                setFormState((current) => ({
                  ...current,
                  expenseDate: event.target.value,
                }))
              }
              required
            />
          </div>

          <div className="field">
            <label htmlFor="expense-amount">Amount</label>
            <input
              id="expense-amount"
              type="number"
              min={0}
              step="0.01"
              value={formState.amount}
              onChange={(event) =>
                setFormState((current) => ({
                  ...current,
                  amount: Number(event.target.value),
                }))
              }
              required
            />
          </div>
        </div>

        <div className="field">
          <label htmlFor="expense-description">Description</label>
          <textarea
            id="expense-description"
            rows={3}
            value={formState.description}
            onChange={(event) =>
              setFormState((current) => ({
                ...current,
                description: event.target.value,
              }))
            }
            placeholder="Optional notes about this cost"
          />
        </div>

        <button
          type="submit"
          className="button button--primary button--sm"
          disabled={isSubmitting}
        >
          {isSubmitting ? "Saving..." : "Add expense"}
        </button>
      </form>

      {notice ? <div className="message message--success">{notice}</div> : null}

      {error ? <div className="message message--error">{error}</div> : null}

      <div className="expense-panel__list">
        {isLoading ? (
          <div className="empty-state">
            <h5>Loading expenses</h5>
            <p>Fetching budget data for this travel plan.</p>
          </div>
        ) : expenses.length > 0 ? (
          expenses.map((expense) => (
            <article key={expense.id} className="expense-item">
              <div className="expense-item__icon" aria-hidden="true">
                •
              </div>

              <div className="expense-item__content">
                <div className="expense-item__header">
                  <h5>{expense.name}</h5>
                  <strong>{formatMoney(expense.amount)}</strong>
                </div>

                <div className="expense-item__meta">
                  <span>{expense.category}</span>
                  <span>{formatDateOnly(expense.expenseDate)}</span>
                </div>

                {expense.description ? <p>{expense.description}</p> : null}
              </div>

              <button
                type="button"
                className="button button--secondary button--sm expense-item__delete"
                onClick={() => setExpensePendingDelete(expense)}
              >
                Delete
              </button>
            </article>
          ))
        ) : (
          <div className="empty-state empty-state--soft">
            <h5>No expenses yet</h5>
            <p>Add manual travel costs to keep the budget accurate.</p>
          </div>
        )}
      </div>

      {expensePendingDelete ? (
        <div
          className="modal-backdrop"
          role="presentation"
          onClick={() => setExpensePendingDelete(null)}
        >
          <div
            className="modal-card"
            role="dialog"
            aria-modal="true"
            aria-labelledby="expense-delete-title"
            onClick={(event) => event.stopPropagation()}
          >
            <div className="modal-card__header">
              <div>
                <h2 id="expense-delete-title">Delete expense</h2>
                <p>
                  Are you sure you want to delete {expensePendingDelete.name}?
                </p>
              </div>

              <button
                type="button"
                className="modal-card__close"
                onClick={() => setExpensePendingDelete(null)}
                aria-label="Close modal"
              >
                ×
              </button>
            </div>

            <div className="stack">
              <p>This action cannot be undone.</p>
              <div className="form-grid form-grid--two">
                <button
                  type="button"
                  className="button button--secondary"
                  onClick={() => setExpensePendingDelete(null)}
                >
                  Cancel
                </button>
                <button
                  type="button"
                  className="button button--primary"
                  onClick={confirmDeleteExpense}
                >
                  Delete expense
                </button>
              </div>
            </div>
          </div>
        </div>
      ) : null}
    </section>
  );
};

export default TravelExpensePanel;
