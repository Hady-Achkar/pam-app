import axios, { AxiosError } from "axios";

const API_URL = import.meta.env.VITE_API_URL ?? "http://localhost:5057/api";

const api = axios.create({
  baseURL: API_URL,
});

export const SERVER_URL = API_URL.replace(/\/api\/?$/, "");

export function getImageUrl(path: string | null): string | undefined {
  if (!path) return undefined;
  if (path.startsWith("http")) return path;
  return `${SERVER_URL}${path}`;
}

export function getApiErrorMessage(error: unknown, fallback: string): string {
  if (error instanceof AxiosError) {
    const data = error.response?.data;
    return data?.detail ?? data?.title ?? fallback;
  }
  return fallback;
}

export default api;
