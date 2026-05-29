import axios from "axios";

type ApiErrorPayload = {
  message?: string;
  error?: string;
  errors?: Record<string, string[] | string>;
};

export const getApiErrorMessage = (error: unknown): string => {
  if (axios.isAxiosError<ApiErrorPayload>(error)) {
    const payload = error.response?.data;

    if (typeof payload === "string") {
      return payload;
    }

    if (payload?.message) {
      return payload.message;
    }

    if (payload?.error) {
      return payload.error;
    }

    if (payload?.errors) {
      const firstEntry = Object.values(payload.errors)[0];

      if (Array.isArray(firstEntry)) {
        return firstEntry[0] ?? "An error occurred.";
      }

      if (typeof firstEntry === "string") {
        return firstEntry;
      }
    }

    return error.message || "An error occurred.";
  }

  if (error instanceof Error) {
    return error.message;
  }

  return "An unexpected error occurred.";
};
