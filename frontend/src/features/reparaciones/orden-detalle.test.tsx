import { cleanup, fireEvent, render, screen } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";

const mocks = vi.hoisted(() => ({ getIdToken: vi.fn() }));

vi.mock("next/navigation", () => ({ useRouter: () => ({ replace: vi.fn(), push: vi.fn() }) }));
vi.mock("@/features/auth/auth-provider", () => ({
  useAuth: () => ({ user: { email: "a@b.com", getIdToken: mocks.getIdToken }, signOut: vi.fn() }),
}));

import { OrdenDetalle } from "./orden-detalle";

const orden = {
  id: "o1",
  numero: 25,
  estado: "recibido",
  cliente: { id: "c1", nombre: "Ana", apellido: "Pérez", telefono: "3498 111111" },
  equipo: {
    id: "d1",
    clienteId: "c1",
    marca: "Apple",
    modelo: "iPhone 13",
    almacenamiento: null,
    color: null,
    imeiSerieEnmascarado: null,
    observaciones: null,
    creadoEn: "2026-11-07T12:00:00Z",
  },
  problemaInformado: "No carga",
  tiposReparacion: ["conector_carga", "otro"],
  otroTipoDescripcion: "Limpieza",
  tecnico: { id: "t1", nombre: "Técnico 1" },
  fechaEstimadaEntrega: "2026-11-10",
  desbloqueoTipo: "patron",
  creadoPor: "Recepción 1",
  creadoEn: "2026-11-07T12:00:00Z",
};

const me = (permisos: string[]) => ({
  uid: "u",
  email: "a@b.com",
  emailVerificado: true,
  nombre: null,
  usuarioId: "id",
  rol: "Técnico",
  permisos,
});

const json = (cuerpo: unknown, status = 200) => new Response(JSON.stringify(cuerpo), { status });

function stubApi(permisos: string[]) {
  const fetchMock = vi.fn<(url: string, init?: RequestInit) => Promise<Response>>((url, init) => {
    if (url.includes("/api/v1/me")) return Promise.resolve(json(me(permisos)));
    if (init?.method === "POST") return Promise.resolve(json({ tipo: "patron", valor: "1-5-9" }));
    if (url.includes("/historial")) return Promise.resolve(json({ items: [] }));
    return Promise.resolve(json(orden));
  });
  vi.stubGlobal("fetch", fetchMock);
  return fetchMock;
}

afterEach(() => {
  cleanup();
  vi.unstubAllGlobals();
  vi.clearAllMocks();
});

describe("OrdenDetalle", () => {
  it("muestra el número de orden, los datos y el aviso de la fecha estimada", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    stubApi(["reparaciones.ver"]);

    render(<OrdenDetalle id="o1" />);

    expect(await screen.findByRole("heading", { name: "Orden 25" })).toBeInTheDocument();
    expect(screen.getByText("Recibido")).toBeInTheDocument();
    expect(screen.getByText("Conector de carga, Otro: Limpieza")).toBeInTheDocument();
    expect(screen.getByText(/puede cambiar y se le avisa/)).toBeInTheDocument();
    expect(screen.getByText(/Patrón guardado/)).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Ver patrón" })).not.toBeInTheDocument();
  });

  it("con permiso muestra el patrón dibujado al pedirlo", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    const fetchMock = stubApi(["reparaciones.ver", "reparaciones.ver_desbloqueo"]);
    render(<OrdenDetalle id="o1" />);

    fireEvent.click(await screen.findByRole("button", { name: "Ver patrón" }));

    expect(await screen.findByRole("group", { name: "Patrón guardado" })).toBeInTheDocument();
    expect(fetchMock.mock.calls.some(([url, init]) => init?.method === "POST" && url.endsWith("/reparaciones/o1/desbloqueo"))).toBe(true);
  });
});
