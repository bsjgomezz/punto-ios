import { cleanup, fireEvent, render, screen, waitFor } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";

const mocks = vi.hoisted(() => {
  const getIdToken = vi.fn();
  // Referencia estable, como el `user` real del AuthProvider (los useEffect dependen de [user]).
  return { getIdToken, user: { email: "a@b.com", getIdToken } };
});

vi.mock("next/navigation", () => ({ useRouter: () => ({ replace: vi.fn(), push: vi.fn() }) }));
vi.mock("@/features/auth/auth-provider", () => ({
  useAuth: () => ({ user: mocks.user, signOut: vi.fn() }),
}));

import { ReparacionesPage } from "./reparaciones-page";

const me = (rol: string, usuarioId = "tec-1") => ({
  uid: "u",
  email: "a@b.com",
  emailVerificado: true,
  nombre: null,
  usuarioId,
  rol,
  permisos: ["reparaciones.ver"],
});

const json = (cuerpo: unknown, status = 200) => new Response(JSON.stringify(cuerpo), { status });

function stubApi(rol: string) {
  const fetchMock = vi.fn<(url: string, init?: RequestInit) => Promise<Response>>((url) => {
    if (url.includes("/api/v1/me")) return Promise.resolve(json(me(rol)));
    return Promise.resolve(json({ items: [], page: 1, pageSize: 20, totalCount: 0, totalPages: 0 }));
  });
  vi.stubGlobal("fetch", fetchMock);
  return fetchMock;
}

const pedidosDeLista = (fetchMock: ReturnType<typeof stubApi>) =>
  fetchMock.mock.calls.map(([url]) => url).filter((url) => url.includes("/api/v1/reparaciones?"));

afterEach(() => {
  cleanup();
  vi.unstubAllGlobals();
  vi.clearAllMocks();
});

describe("ReparacionesPage — filtros de la bandeja", () => {
  it("a un técnico le muestra solo sus órdenes activas por defecto", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    const fetchMock = stubApi("Técnico");

    render(<ReparacionesPage />);

    expect(await screen.findByLabelText("Solo mis reparaciones")).toBeChecked();
    expect(screen.getByLabelText("Solo activas")).toBeChecked();
    await waitFor(() => expect(pedidosDeLista(fetchMock)).toHaveLength(1));
    const url = pedidosDeLista(fetchMock)[0];
    expect(url).toContain("tecnicoId=tec-1");
    expect(url).toContain("soloActivas=true");
  });

  it("a un rol que no es técnico no le muestra 'Solo mis reparaciones' ni filtra por técnico", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    const fetchMock = stubApi("Recepción");

    render(<ReparacionesPage />);

    expect(await screen.findByLabelText("Solo activas")).toBeChecked();
    await waitFor(() => expect(pedidosDeLista(fetchMock)).toHaveLength(1));
    expect(screen.queryByLabelText("Solo mis reparaciones")).not.toBeInTheDocument();
    expect(pedidosDeLista(fetchMock)[0]).not.toContain("tecnicoId");
  });

  it("al desmarcar 'Solo activas' vuelve a pedir la lista incluyendo entregadas y canceladas", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    const fetchMock = stubApi("Administrador");
    render(<ReparacionesPage />);
    await waitFor(() => expect(pedidosDeLista(fetchMock)).toHaveLength(1));

    fireEvent.click(screen.getByLabelText("Solo activas"));

    await waitFor(() => expect(pedidosDeLista(fetchMock)).toHaveLength(2));
    const ultimo = pedidosDeLista(fetchMock)[1];
    expect(ultimo).toContain("soloActivas=false");
    expect(ultimo).toContain("page=1");
  });

  it("si el técnico desmarca 'Solo mis reparaciones' pide sin filtrar por técnico", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    const fetchMock = stubApi("Técnico");
    render(<ReparacionesPage />);
    fireEvent.click(await screen.findByLabelText("Solo mis reparaciones"));

    await waitFor(() => expect(pedidosDeLista(fetchMock)).toHaveLength(2));
    expect(pedidosDeLista(fetchMock)[1]).not.toContain("tecnicoId");
  });
});
