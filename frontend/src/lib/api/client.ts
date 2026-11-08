import { API_BASE_URL } from "./config";

/** Cuerpo de error del backend (ProblemDetails) con el código de negocio en `codigo`. */
export type ProblemDetails = { title?: string; codigo?: string; [clave: string]: unknown };

export class ApiError extends Error {
  constructor(
    public readonly status: number,
    public readonly problem?: ProblemDetails,
  ) {
    super(problem?.title ?? `El servidor respondió ${status}.`);
    this.name = "ApiError";
  }
}

async function request<T>(
  method: "GET" | "POST" | "PUT",
  path: string,
  token: string,
  body?: unknown,
  signal?: AbortSignal,
): Promise<T> {
  const headers: Record<string, string> = { Authorization: `Bearer ${token}` };
  if (body !== undefined) headers["Content-Type"] = "application/json";

  const response = await fetch(`${API_BASE_URL}${path}`, {
    method,
    headers,
    body: body === undefined ? undefined : JSON.stringify(body),
    cache: "no-store",
    signal,
  });

  if (!response.ok) {
    const problem = (await response.json().catch(() => undefined)) as ProblemDetails | undefined;
    throw new ApiError(response.status, problem);
  }
  return (await response.json()) as T;
}

/** GET autenticado: envía el Firebase ID Token en `Authorization: Bearer`. */
export function apiGet<T>(path: string, token: string, signal?: AbortSignal): Promise<T> {
  return request<T>("GET", path, token, undefined, signal);
}

/** POST autenticado con cuerpo JSON. */
export function apiPost<T>(path: string, token: string, body: unknown, signal?: AbortSignal): Promise<T> {
  return request<T>("POST", path, token, body, signal);
}

/** PUT autenticado con cuerpo JSON. */
export function apiPut<T>(path: string, token: string, body: unknown, signal?: AbortSignal): Promise<T> {
  return request<T>("PUT", path, token, body, signal);
}

/** POST autenticado con `multipart/form-data` (subida de archivos). */
export async function apiPostForm<T>(path: string, token: string, datos: FormData, signal?: AbortSignal): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    method: "POST",
    headers: { Authorization: `Bearer ${token}` },
    body: datos,
    cache: "no-store",
    signal,
  });
  if (!response.ok) {
    const problem = (await response.json().catch(() => undefined)) as ProblemDetails | undefined;
    throw new ApiError(response.status, problem);
  }
  return (await response.json()) as T;
}

/** GET autenticado de un archivo binario (p. ej. una foto). */
export async function apiGetBlob(path: string, token: string, signal?: AbortSignal): Promise<Blob> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: { Authorization: `Bearer ${token}` },
    cache: "no-store",
    signal,
  });
  if (!response.ok) {
    const problem = (await response.json().catch(() => undefined)) as ProblemDetails | undefined;
    throw new ApiError(response.status, problem);
  }
  return await response.blob();
}
