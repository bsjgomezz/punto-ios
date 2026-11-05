/** URL base del backend. Se puede cambiar con NEXT_PUBLIC_API_BASE_URL (frontend/.env.local). */
export const API_BASE_URL = (
  process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5080"
).replace(/\/+$/, "");
