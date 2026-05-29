import {
  createContext,
  useEffect,
  useState,
  type ReactNode,
} from "react";
import type { AuthContextType } from "../types/auth";
import type { AuthResponseDto, UserDto } from "../models/auth/dtos";
import {
  clearAuthSession,
  readStoredUser,
  saveAuthSession,
  AUTH_TOKEN_KEY,
} from "../helpers/authStorage";
import { decodeJwt, isJwtExpired } from "../helpers/jwt";

export const AuthContext = createContext<AuthContextType | undefined>(
  undefined,
);

type AuthProviderProps = {
  children: ReactNode;
};

export const AuthProvider = ({ children }: AuthProviderProps) => {
  const [token, setToken] = useState<string | null>(
    localStorage.getItem(AUTH_TOKEN_KEY),
  );
  const [user, setUser] = useState<UserDto | null>(null);
  const [loading, setLoading] = useState(true);

  const logout = () => {
    clearAuthSession();
    setToken(null);
    setUser(null);
  };

  useEffect(() => {
    const storedToken = localStorage.getItem(AUTH_TOKEN_KEY);

    if (!storedToken) {
      setLoading(false);
      return;
    }

    if (isJwtExpired(storedToken)) {
      logout();
      setLoading(false);
      return;
    }

    const storedUser = readStoredUser();

    if (storedUser) {
      setToken(storedToken);
      setUser(storedUser);
      setLoading(false);
      return;
    }

    const decoded = decodeJwt(storedToken);

    if (decoded) {
      const recoveredUser: UserDto = {
        id: decoded.id,
        firstName: decoded.firstName,
        lastName: decoded.lastName,
        email: decoded.email,
        role: decoded.role,
      };

      setToken(storedToken);
      setUser(recoveredUser);
      saveAuthSession(storedToken, recoveredUser);
    } else {
      logout();
    }

    setLoading(false);
  }, []);

  const login = (response: AuthResponseDto) => {
    if (!response.accessToken || !response.user) {
      return;
    }

    saveAuthSession(response.accessToken, response.user);
    setToken(response.accessToken);
    setUser(response.user);
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated: Boolean(token && user),
        token,
        login,
        logout,
        loading,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};
