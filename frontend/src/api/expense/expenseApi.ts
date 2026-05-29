import { AUTH_TOKEN_KEY } from "../../helpers/authStorage";
import { createApiClient } from "../client";

export const expenseApi = createApiClient({
  resourcePath: "api/expense",
  getToken: () => localStorage.getItem(AUTH_TOKEN_KEY),
});
