import { afterEach, describe, expect, it, vi } from "vitest";
import { probeEndpoint } from "./probe";

afterEach(() => {
  vi.unstubAllGlobals();
  vi.useRealTimers();
});

describe("probeEndpoint", () => {
  it("devuelve ok cuando el servidor responde 2xx", async () => {
    vi.stubGlobal("fetch", vi.fn().mockResolvedValue(new Response("Healthy", { status: 200 })));

    expect(await probeEndpoint("http://api/health")).toEqual({ state: "ok" });
  });

  it("devuelve error con el código cuando el servidor responde 503", async () => {
    vi.stubGlobal("fetch", vi.fn().mockResolvedValue(new Response("Unhealthy", { status: 503 })));

    expect(await probeEndpoint("http://api/health/ready")).toEqual({
      state: "error",
      detail: "El servidor respondió 503.",
    });
  });

  it("devuelve error cuando la red falla", async () => {
    vi.stubGlobal("fetch", vi.fn().mockRejectedValue(new TypeError("Failed to fetch")));

    expect(await probeEndpoint("http://api/health")).toEqual({
      state: "error",
      detail: "No hubo respuesta.",
    });
  });

  it("corta la espera al superar el tiempo máximo", async () => {
    vi.useFakeTimers();
    vi.stubGlobal(
      "fetch",
      vi.fn((_url: string, init: RequestInit) =>
        new Promise((_resolve, reject) => {
          init.signal?.addEventListener("abort", () => reject(new DOMException("Aborted", "AbortError")));
        }),
      ),
    );

    const pending = probeEndpoint("http://api/health", undefined, 1000);
    await vi.advanceTimersByTimeAsync(1000);

    expect(await pending).toEqual({ state: "error", detail: "No hubo respuesta." });
  });
});
