// import axios from "axios";

// type ApiErrorPayload = {
//   message?: string;
//   error?: string;
//   errors?: Record<string, string[] | string>;
// };

// const DEFAULT_ERROR_MESSAGE = "Something went wrong. Please try again.";

// const unwrapAggregateMessage = (message: string): string => {
//   const prefix = "One or more errors occurred. (";

//   if (message.startsWith(prefix) && message.endsWith(")")) {
//     return message.slice(prefix.length, -1);
//   }

//   return message;
// };

// const hasSensitiveDetails = (message: string): boolean => {
//   const normalizedMessage = message.toLowerCase();
//   const idLikeValuePattern =
//     /\b[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}\b|\b[a-z0-9_-]{16,}\b/i;

//   return (
//     normalizedMessage.includes("with id") ||
//     normalizedMessage.includes("userid") ||
//     normalizedMessage.includes("travelid") ||
//     normalizedMessage.includes("destinationid") ||
//     normalizedMessage.includes("expenseid") ||
//     normalizedMessage.includes("activityid") ||
//     normalizedMessage.includes("token") ||
//     normalizedMessage.includes("exception") ||
//     idLikeValuePattern.test(message)
//   );
// };

// const firstValidationMessage = (
//   errors?: Record<string, string[] | string>,
// ): string | null => {
//   if (!errors) {
//     return null;
//   }

//   const firstEntry = Object.values(errors)[0];

//   if (Array.isArray(firstEntry)) {
//     return firstEntry[0] ?? null;
//   }

//   if (typeof firstEntry === "string") {
//     return firstEntry;
//   }

//   return null;
// };

// const sanitizeServerMessage = (
//   message: string | null | undefined,
// ): string | null => {
//   if (!message) {
//     return null;
//   }

//   const cleanedMessage = unwrapAggregateMessage(message).trim();

//   if (!cleanedMessage || hasSensitiveDetails(cleanedMessage)) {
//     return null;
//   }

//   return cleanedMessage;
// };

// const getStatusMessage = (status?: number): string => {
//   switch (status) {
//     case 400:
//       return "Please check the entered data and try again.";
//     case 401:
//       return "Please sign in again to continue.";
//     case 403:
//       return "You do not have permission to perform this action.";
//     case 404:
//       return "The requested item could not be found.";
//     case 409:
//       return "This action cannot be completed with the current data.";
//     case 500:
//     case 502:
//     case 503:
//     case 504:
//       return "The server could not complete the request right now.";
//     default:
//       return DEFAULT_ERROR_MESSAGE;
//   }
// };

// export const getApiErrorMessage = (error: unknown): string => {
//   if (axios.isAxiosError<ApiErrorPayload>(error)) {
//     const payload = error.response?.data;
//     const status = error.response?.status;

//     if (typeof payload === "string") {
//       return sanitizeServerMessage(payload) ?? getStatusMessage(status);
//     }

//     const validationMessage = sanitizeServerMessage(
//       firstValidationMessage(payload?.errors),
//     );

//     if (validationMessage) {
//       return validationMessage;
//     }

//     const payloadMessage = sanitizeServerMessage(payload?.message);

//     if (payloadMessage) {
//       return payloadMessage;
//     }

//     const payloadError = sanitizeServerMessage(payload?.error);

//     if (payloadError) {
//       return payloadError;
//     }

//     return getStatusMessage(status);
//   }

//   if (error instanceof Error) {
//     return sanitizeServerMessage(error.message) ?? DEFAULT_ERROR_MESSAGE;
//   }

//   return DEFAULT_ERROR_MESSAGE;
// };
import axios from "axios";

const DEFAULT_ERROR_MESSAGE = "Something went wrong. Please try again.";

export const getApiErrorMessage = (error: unknown): string => {
  if (axios.isAxiosError(error)) {
    const payload = error.response?.data;
    if (payload?.message) {
      return payload.message;
    }

    return DEFAULT_ERROR_MESSAGE;
  }
  if (error instanceof Error) {
    return error.message;
  }
  return DEFAULT_ERROR_MESSAGE;
};
