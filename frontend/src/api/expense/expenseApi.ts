import { AUTH_TOKEN_KEY } from "../../helpers/authStorage";
import { createApiClient } from "../client";
import type {
  CreateExpenseDto,
  ExpenseDto,
  TravelBudgetSummaryDto,
} from "../../models/expense/dtos";

const expenseClient = createApiClient({
  resourcePath: "api/expense",
  getToken: () => localStorage.getItem(AUTH_TOKEN_KEY),
});

export const expenseApi = {
  addExpense: async (
    travelId: string,
    data: CreateExpenseDto,
  ): Promise<ExpenseDto> => {
    const response = await expenseClient.post<ExpenseDto>(
      `/travels/${travelId}/expenses`,
      data,
    );

    return response.data;
  },

  getExpensesByTravelId: async (travelId: string): Promise<ExpenseDto[]> => {
    const response = await expenseClient.get<ExpenseDto[]>(
      `/travels/${travelId}/expenses`,
    );

    return response.data;
  },

  getBudgetSummary: async (
    travelId: string,
  ): Promise<TravelBudgetSummaryDto> => {
    const response = await expenseClient.get<TravelBudgetSummaryDto>(
      `/travels/${travelId}/budget`,
    );

    return response.data;
  },

  deleteExpense: async (travelId: string, expenseId: string): Promise<void> => {
    await expenseClient.delete(`/travels/${travelId}/expenses/${expenseId}`);
  },
};
