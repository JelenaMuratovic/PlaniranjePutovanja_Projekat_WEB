import axios, { type AxiosInstance } from "axios";
import { joinApiUrl, requireApiUrl } from "../helpers/env";

type ClientOptions = {
  resourcePath: string;
  getToken?: () => string | null;
};

export const createApiClient = ({
  resourcePath,
  getToken,
}: ClientOptions): AxiosInstance => {
  const client = axios.create({
    baseURL: joinApiUrl(requireApiUrl(), resourcePath),
    headers: {
      "Content-Type": "application/json",
    },
  });

  client.interceptors.request.use((config) => {
    const token = getToken?.();

    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    const activeShareToken = sessionStorage.getItem("active_share_token");
    if (activeShareToken) {
      config.headers["X-Share-Token"] = activeShareToken;
    }

    return config;
  });

  return client;
};
