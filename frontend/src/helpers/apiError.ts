import axios from "axios";

type ApiErrorPayload = {
  message?: string;
  error?: string;
  errors?: Record<string, string[] | string>;
};

const unwrapAggregateMessage = (message: string): string => {
  const prefix = "One or more errors occurred. (";

  if (message.startsWith(prefix) && message.endsWith(")")) {
    return message.slice(prefix.length, -1);
  }

  return message;
};

export const getApiErrorMessage = (error: unknown): string => {
  if (axios.isAxiosError<ApiErrorPayload>(error)) {
    const payload = error.response?.data;

    if (typeof payload === "string") {
      return payload;
    }

    if (payload?.message) {
      return unwrapAggregateMessage(payload.message);
    }

    if (payload?.error) {
      return unwrapAggregateMessage(payload.error);
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

    return unwrapAggregateMessage(error.message || "An error occurred.");
  }

  if (error instanceof Error) {
    return unwrapAggregateMessage(error.message);
  }

  return "An unexpected error occurred.";
};
