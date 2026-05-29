import type { UserDto } from "../models/auth/dtos";

export const AUTH_TOKEN_KEY = "access_token";
export const AUTH_USER_KEY = "access_user";

export const saveAuthSession = (token: string, user: UserDto): void => {
  localStorage.setItem(AUTH_TOKEN_KEY, token);
  localStorage.setItem(AUTH_USER_KEY, JSON.stringify(user));
};

export const clearAuthSession = (): void => {
  localStorage.removeItem(AUTH_TOKEN_KEY);
  localStorage.removeItem(AUTH_USER_KEY);
};

export const readStoredUser = (): UserDto | null => {
  const rawUser = localStorage.getItem(AUTH_USER_KEY);

  if (!rawUser) {
    return null;
  }

  try {
    return JSON.parse(rawUser) as UserDto;
  } catch {
    return null;
  }
};
