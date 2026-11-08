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

function stubApi(permisos: string[], ordenPersonalizada: unknown = orden, fotos: { id: string; creadoEn: string }[] = []) {
  const fetchMock = vi.fn<(url: string, init?: RequestInit) => Promise<Response>>((url, init) => {
    if (url.includes("/api/v1/me")) return Promise.resolve(json(me(permisos)));
    if (init?.method === "POST") return Promise.resolve(json({ tipo: "patron", valor: "1-5-9" }));
    if (url.includes("/historial")) return Promise.resolve(json({ items: [] }));
    if (url.match(/\/fotos\/[^/]+$/)) return Promise.resolve(new Response(new Blob(["x"], { type: "image/jpeg" })));
    if (url.endsWith("/fotos")) return Promise.resolve(json(fotos));
    return Promise.resolve(json(ordenPersonalizada));
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

  it("si el equipo no enciende avisa, no estima fecha, y muestra el checklist y las fotos", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    const ordenNoEnciende = {
      ...orden,
      fechaEstimadaEntrega: null,
      noEnciende: true,
      checklist: { items: { enciende: false }, saludBateria: 42, notas: "Vidrio roto" },
    };
    stubApi(["reparaciones.ver"], ordenNoEnciende, [{ id: "f1", creadoEn: "2026-11-07T12:00:00Z" }]);

    render(<OrdenDetalle id="o1" />);

    await screen.findByRole("heading", { name: "Orden 25" });
    expect(screen.getByRole("alert")).toHaveTextContent("no enciende");
    expect(screen.getByText("No se puede determinar")).toBeInTheDocument();
    expect(screen.getByText((_, el) => el?.tagName === "LI" && el.textContent === "Enciende: no")).toBeInTheDocument();
    expect(screen.getByText("Salud de batería: 42%")).toBeInTheDocument();
    expect(screen.getByText("Notas: Vidrio roto")).toBeInTheDocument();
    expect(await screen.findByRole("img", { name: "Foto del equipo" })).toBeInTheDocument();
  });
});
