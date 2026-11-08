import { cleanup, fireEvent, render, screen, within } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";

const mocks = vi.hoisted(() => {
  const getIdToken = vi.fn();
  // Referencia estable, como el `user` real del AuthProvider: si cambiara en cada render,
  // los useEffect que dependen de [user] se re-ejecutarían sin parar y volverían a cargar la orden original.
  return { getIdToken, user: { email: "a@b.com", getIdToken } };
});

vi.mock("next/navigation", () => ({ useRouter: () => ({ replace: vi.fn(), push: vi.fn() }) }));
vi.mock("@/features/auth/auth-provider", () => ({
  useAuth: () => ({ user: mocks.user, signOut: vi.fn() }),
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

const PERMISOS_ESTADO = ["reparaciones.ver", "reparaciones.cambiar_estado"];

/** Orden en "recibido" con transiciones disponibles; el POST de cambio de estado responde `respuestaPost`. */
function stubCambioDeEstado(
  respuestaPost: () => Response,
  ordenInicial: unknown = { ...orden, estadosDisponibles: ["en_pruebas", "en_diagnostico", "cancelado"] },
  historial: unknown[] = [],
) {
  const fetchMock = vi.fn<(url: string, init?: RequestInit) => Promise<Response>>((url, init) => {
    if (url.includes("/api/v1/me")) return Promise.resolve(json(me(PERMISOS_ESTADO)));
    if (init?.method === "POST" && url.endsWith("/estado")) return Promise.resolve(respuestaPost());
    if (url.includes("/historial")) return Promise.resolve(json({ items: historial }));
    if (url.endsWith("/fotos")) return Promise.resolve(json([]));
    return Promise.resolve(json(ordenInicial));
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

  describe("cambio de estado", () => {
    it("no muestra el control si la orden está en un estado final", async () => {
      mocks.getIdToken.mockResolvedValue("t");
      stubCambioDeEstado(() => json({}), { ...orden, estado: "entregado", estadosDisponibles: [] });

      render(<OrdenDetalle id="o1" />);

      expect(await screen.findByText(/es un estado final/)).toBeInTheDocument();
      expect(screen.queryByRole("combobox", { name: "Pasar a" })).not.toBeInTheDocument();
      expect(screen.queryByRole("button", { name: "Cambiar estado" })).not.toBeInTheDocument();
    });

    it("sin el permiso no muestra el control", async () => {
      mocks.getIdToken.mockResolvedValue("t");
      stubApi(["reparaciones.ver"], { ...orden, estadosDisponibles: ["en_pruebas"] });

      render(<OrdenDetalle id="o1" />);

      await screen.findByRole("heading", { name: "Orden 25" });
      expect(screen.queryByRole("region", { name: "Estado de la orden" })).not.toBeInTheDocument();
    });

    it("ofrece solo los estados disponibles, con su nombre", async () => {
      mocks.getIdToken.mockResolvedValue("t");
      stubCambioDeEstado(() => json({}));

      render(<OrdenDetalle id="o1" />);

      const select = await screen.findByRole("combobox", { name: "Pasar a" });
      const opciones = within(select)
        .getAllByRole("option")
        .map((o) => [(o as HTMLOptionElement).value, o.textContent]);
      expect(opciones).toEqual([
        ["", "Elegí un estado"],
        ["en_pruebas", "En pruebas"],
        ["en_diagnostico", "En diagnóstico"],
        ["cancelado", "Cancelado"],
      ]);
      expect(screen.getByRole("button", { name: "Cambiar estado" })).toBeDisabled();
    });

    it("envía el nuevo estado y muestra la orden que devuelve el backend", async () => {
      mocks.getIdToken.mockResolvedValue("t");
      const actualizada = { ...orden, estado: "en_pruebas", estadosDisponibles: ["en_diagnostico", "cancelado"] };
      const fetchMock = stubCambioDeEstado(() => json(actualizada));

      render(<OrdenDetalle id="o1" />);

      fireEvent.change(await screen.findByRole("combobox", { name: "Pasar a" }), { target: { value: "en_pruebas" } });
      fireEvent.click(screen.getByRole("button", { name: "Cambiar estado" }));

      expect(await screen.findByText("En pruebas", { selector: ".estado-chip" })).toBeInTheDocument();
      const post = fetchMock.mock.calls.find(([url, init]) => init?.method === "POST" && url.endsWith("/reparaciones/o1/estado"));
      expect(post).toBeDefined();
      expect(JSON.parse(String(post![1]!.body))).toEqual({ nuevoEstado: "en_pruebas" });
      const opciones = within(screen.getByRole("combobox", { name: "Pasar a" }))
        .getAllByRole("option")
        .map((o) => (o as HTMLOptionElement).value);
      expect(opciones).toEqual(["", "en_diagnostico", "cancelado"]);
    });

    it("muestra el error del backend si la transición no está permitida", async () => {
      mocks.getIdToken.mockResolvedValue("t");
      stubCambioDeEstado(() =>
        json({ title: "No se puede pasar de Recibido a En pruebas.", codigo: "transicion_invalida", status: 409 }, 409),
      );

      render(<OrdenDetalle id="o1" />);

      fireEvent.change(await screen.findByRole("combobox", { name: "Pasar a" }), { target: { value: "en_pruebas" } });
      fireEvent.click(screen.getByRole("button", { name: "Cambiar estado" }));

      expect(await screen.findByRole("alert")).toHaveTextContent("No se puede pasar de Recibido a En pruebas.");
      expect(screen.getByRole("heading", { name: "Orden 25" })).toBeInTheDocument();
      expect(screen.getByText("Recibido", { selector: ".estado-chip" })).toBeInTheDocument();
      expect(screen.getByRole("combobox", { name: "Pasar a" })).toBeInTheDocument();
    });
  });

  it("muestra en el historial los cambios de estado con el nombre de cada estado", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    stubCambioDeEstado(() => json({}), { ...orden, estadosDisponibles: [] }, [
      {
        id: "r1",
        accion: "estado_cambiado",
        usuario: "Técnico 1",
        fecha: "2026-11-08T12:00:00Z",
        cambios: [{ campo: "estado", anterior: "recibido", nuevo: "en_diagnostico" }],
      },
    ]);

    render(<OrdenDetalle id="o1" />);

    const historial = within(await screen.findByRole("region", { name: "Historial de la orden" }));
    expect(
      historial.getByText((_, el) => el?.tagName === "SPAN" && el.textContent === "Cambió el estado de Recibido a En diagnóstico"),
    ).toBeInTheDocument();
  });
});
