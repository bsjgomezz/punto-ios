import { cleanup, render, screen } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";

const mocks = vi.hoisted(() => ({
  getIdToken: vi.fn(),
}));

vi.mock("./auth-provider", () => ({
  useAuth: () => ({ user: { getIdToken: mocks.getIdToken } }),
}));

import { ServerIdentity } from "./server-identity";

afterEach(() => {
  cleanup();
  vi.unstubAllGlobals();
  vi.clearAllMocks();
});

describe("ServerIdentity", () => {
  it("envía el token y confirma que el servidor reconoce la sesión", async () => {
    mocks.getIdToken.mockResolvedValue("token-123");
    const fetchMock = vi.fn().mockResolvedValue(
      new Response(JSON.stringify({ uid: "u1", email: "dueno@puntoios.com", emailVerificado: true, nombre: null }), {
        status: 200,
      }),
    );
    vi.stubGlobal("fetch", fetchMock);

    render(<ServerIdentity />);

    expect(await screen.findByText(/El servidor reconoce tu sesión \(dueno@puntoios.com\)/)).toBeInTheDocument();
    const [url, init] = fetchMock.mock.calls[0] as [string, RequestInit];
    expect(url).toMatch(/\/api\/v1\/me$/);
    expect((init.headers as Record<string, string>).Authorization).toBe("Bearer token-123");
  });

  it("avisa si el servidor rechaza la sesión (401)", async () => {
    mocks.getIdToken.mockResolvedValue("token-malo");
    vi.stubGlobal("fetch", vi.fn().mockResolvedValue(new Response("", { status: 401 })));

    render(<ServerIdentity />);

    expect(await screen.findByText(/El servidor no reconoce tu sesión/)).toBeInTheDocument();
  });

  it("explica qué hacer si no se llega al servidor", async () => {
    mocks.getIdToken.mockResolvedValue("token");
    vi.stubGlobal("fetch", vi.fn().mockRejectedValue(new TypeError("Failed to fetch")));

    render(<ServerIdentity />);

    expect(await screen.findByText(/Verificá que el backend esté corriendo/)).toBeInTheDocument();
  });
});
