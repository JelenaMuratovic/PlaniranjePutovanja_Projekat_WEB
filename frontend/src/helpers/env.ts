const rawApiUrl = import.meta.env.VITE_API_URL as string | undefined;

export const API_URL = rawApiUrl?.trim() ?? "";

export const requireApiUrl = (): string => {
  if (!API_URL) {
    throw new Error("VITE_API_URL not defined in .env file.");
  }

  return API_URL;
};

export const joinApiUrl = (baseUrl: string, path: string): string => {
  const normalizedBaseUrl = baseUrl.endsWith("/")
    ? baseUrl.slice(0, -1)
    : baseUrl;
  const normalizedPath = path.startsWith("/") ? path.slice(1) : path;

  return `${normalizedBaseUrl}/${normalizedPath}`;
};
