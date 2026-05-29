export type UserDto = {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
};

export type LoginRequestDto = {
  email: string;
  password: string;
};

export type RegisterRequestDto = {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
};

export type AuthResponseDto = {
  isSuccess: boolean;
  message: string;
  accessToken?: string | null;
  user?: UserDto | null;
};