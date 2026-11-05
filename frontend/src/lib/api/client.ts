import { API_BASE_URL } from "./config";

export class ApiError extends Error {
  constructor(public readonly status: number) {
    super(`El servidor respondió ${status}.`);
    this.name = "ApiError";
  }
}

/** GET autenticado: envía el Firebase ID Token en `Authorization: Bearer`. */
export async function apiGet<T>(path: string, token: string, signal?: AbortSignal): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: { Authorization: `Bearer ${token}` },
    cache: "no-store",
    signal,
  });

  if (!response.ok) throw new ApiError(response.status);
  return (await response.json()) as T;
}
