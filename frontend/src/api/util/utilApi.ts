import { AUTH_TOKEN_KEY } from "../../helpers/authStorage";
import { createApiClient } from "../client";

export const utilApi = createApiClient({
  resourcePath: "api/util",
  getToken: () => localStorage.getItem(AUTH_TOKEN_KEY),
});
