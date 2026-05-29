import { AUTH_TOKEN_KEY } from "../../helpers/authStorage";
import { createApiClient } from "../client";

export const travelApi = createApiClient({
  resourcePath: "api/travel",
  getToken: () => localStorage.getItem(AUTH_TOKEN_KEY),
});
