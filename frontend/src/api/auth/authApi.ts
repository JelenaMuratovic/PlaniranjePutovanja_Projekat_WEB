import type {
  AuthResponseDto,
  LoginRequestDto,
  RegisterRequestDto,
  UserDto,
} from "../../models/auth/dtos";
import { AUTH_TOKEN_KEY } from "../../helpers/authStorage";
import { createApiClient } from "../client";

const authClient = createApiClient({
  resourcePath: "api/auth",
  getToken: () => localStorage.getItem(AUTH_TOKEN_KEY),
});

export const authApi = {
  login: async (data: LoginRequestDto): Promise<AuthResponseDto> => {
    const response = await authClient.post<AuthResponseDto>("/login", data);
    return response.data;
  },
  register: async (data: RegisterRequestDto): Promise<AuthResponseDto> => {
    const response = await authClient.post<AuthResponseDto>("/register", data);
    return response.data;
  },
  getAllUsers: async (): Promise<UserDto[]> => {
    const response = await authClient.get<UserDto[]>("/admin/users");
    return response.data;
  },
  deleteUser: async (userId: string): Promise<void> => {
    await authClient.delete(`/admin/users/${userId}`);
  },
};
