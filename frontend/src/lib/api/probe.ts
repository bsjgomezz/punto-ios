export type ProbeResult =
  | { state: "checking" }
  | { state: "ok" }
  | { state: "error"; detail: string };

/**
 * Consulta un endpoint de salud. Nunca rechaza: cualquier falla se devuelve como
 * `{ state: "error" }`. Corta la espera a los `timeoutMs` o cuando `signal` se aborta.
 */
export async function probeEndpoint(
  url: string,
  signal?: AbortSignal,
  timeoutMs = 5000,
): Promise<ProbeResult> {
  const controller = new AbortController();
  const abort = () => controller.abort();

  if (signal?.aborted) controller.abort();
  signal?.addEventListener("abort", abort, { once: true });
  const timer = setTimeout(abort, timeoutMs);

  try {
    const response = await fetch(url, {
      signal: controller.signal,
      cache: "no-store",
    });
    return response.ok
      ? { state: "ok" }
      : { state: "error", detail: `El servidor respondió ${response.status}.` };
  } catch {
    return { state: "error", detail: "No hubo respuesta." };
  } finally {
    clearTimeout(timer);
    signal?.removeEventListener("abort", abort);
  }
}
