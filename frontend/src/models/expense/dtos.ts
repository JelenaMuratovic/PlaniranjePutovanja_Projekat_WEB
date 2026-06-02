export type ExpenseCategory =
  | "Transport"
  | "Accommodation"
  | "Food"
  | "Tickets"
  | "Shopping"
  | "Other";

export type CreateExpenseDto = {
  name: string;
  category: ExpenseCategory;
  amount: number;
  expenseDate: string;
  description: string;
};

export type ExpenseDto = {
  id: string;
  travelId: string;
  name: string;
  category: ExpenseCategory;
  amount: number;
  expenseDate: string;
  description: string;
  createdAt: string;
};

export type TravelBudgetSummaryDto = {
  travelId: string;
  plannedBudget: number;
  totalExpenses: number;
  remainingBudget: number;
  spentPercentage: number;
  expenseCount: number;
};