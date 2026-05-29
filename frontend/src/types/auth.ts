import type {
  AuthResponseDto,
  LoginRequestDto,
  RegisterRequestDto,
  UserDto,
} from "../models/auth/dtos";

export type AuthContextType = {
  user: UserDto | null;
  isAuthenticated: boolean;
  token: string | null;
  login: (response: AuthResponseDto) => void;
  logout: () => void;
  loading: boolean;
};

export type { AuthResponseDto, LoginRequestDto, RegisterRequestDto, UserDto };
