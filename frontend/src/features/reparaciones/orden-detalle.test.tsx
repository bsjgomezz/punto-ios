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

import { formatearMonto } from "./api";
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

/** Monto como lo deja Testing Library al normalizar el texto (el espacio duro de Intl pasa a ser un espacio común). */
const montoEnPantalla = (monto: number) => formatearMonto(monto).replace(/\s/g, " ");

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

  describe("datos técnicos", () => {
    const PERMISOS_TECNICO = ["reparaciones.ver", "reparaciones.reparar"];
    const ordenConDatos = {
      ...orden,
      estadosDisponibles: [],
      pruebas: "Probé el cargador original",
      diagnostico: "Conector sulfatado",
      mediciones: null,
      componentesAfectados: ["Conector de carga"],
      notasInternas: "Cliente apurado",
    };

    /** Orden con datos técnicos; el PUT responde `respuestaPut`. */
    function stubDatosTecnicos(respuestaPut: () => Response, permisos: string[] = PERMISOS_TECNICO, ordenInicial: unknown = ordenConDatos) {
      const fetchMock = vi.fn<(url: string, init?: RequestInit) => Promise<Response>>((url, init) => {
        if (url.includes("/api/v1/me")) return Promise.resolve(json(me(permisos)));
        if (init?.method === "PUT" && url.endsWith("/datos-tecnicos")) return Promise.resolve(respuestaPut());
        if (url.includes("/historial")) return Promise.resolve(json({ items: [] }));
        if (url.endsWith("/fotos")) return Promise.resolve(json([]));
        return Promise.resolve(json(ordenInicial));
      });
      vi.stubGlobal("fetch", fetchMock);
      return fetchMock;
    }

    it("con el permiso muestra la sección con los valores actuales", async () => {
      mocks.getIdToken.mockResolvedValue("t");
      stubDatosTecnicos(() => json({}));

      render(<OrdenDetalle id="o1" />);

      const seccion = within(await screen.findByRole("region", { name: "Datos técnicos" }));
      expect(seccion.getByLabelText("Pruebas realizadas")).toHaveValue("Probé el cargador original");
      expect(seccion.getByLabelText("Diagnóstico")).toHaveValue("Conector sulfatado");
      expect(seccion.getByLabelText("Mediciones")).toHaveValue("");
      expect(seccion.getByLabelText("Notas internas")).toHaveValue("Cliente apurado");
      expect(seccion.getByText("Privadas: no las ve el cliente.")).toBeInTheDocument();
      const lista = within(seccion.getByRole("list", { name: "Componentes afectados" }));
      expect(lista.getAllByRole("listitem").map((li) => li.textContent?.replace("×", "").trim())).toEqual(["Conector de carga"]);
    });

    it("sin el permiso no muestra la sección", async () => {
      mocks.getIdToken.mockResolvedValue("t");
      stubDatosTecnicos(() => json({}), ["reparaciones.ver", "reparaciones.cambiar_estado"]);

      render(<OrdenDetalle id="o1" />);

      await screen.findByRole("heading", { name: "Orden 25" });
      expect(screen.queryByRole("region", { name: "Datos técnicos" })).not.toBeInTheDocument();
      expect(screen.queryByText("Conector sulfatado")).not.toBeInTheDocument();
    });

    it("agrega y quita componentes afectados", async () => {
      mocks.getIdToken.mockResolvedValue("t");
      stubDatosTecnicos(() => json({}));

      render(<OrdenDetalle id="o1" />);

      const input = await screen.findByRole("textbox", { name: "Componentes afectados" });
      fireEvent.change(input, { target: { value: "Batería" } });
      fireEvent.click(screen.getByRole("button", { name: "Agregar" }));
      fireEvent.change(input, { target: { value: "Flex de botones" } });
      fireEvent.keyDown(input, { key: "Enter" });

      const nombres = () =>
        within(screen.getByRole("list", { name: "Componentes afectados" }))
          .getAllByRole("listitem")
          .map((li) => li.textContent?.replace("×", "").trim());
      expect(nombres()).toEqual(["Conector de carga", "Batería", "Flex de botones"]);
      expect(input).toHaveValue("");

      fireEvent.click(screen.getByRole("button", { name: "Quitar Batería" }));
      expect(nombres()).toEqual(["Conector de carga", "Flex de botones"]);
    });

    it("guarda con PUT los cinco campos y muestra lo que devuelve el backend", async () => {
      mocks.getIdToken.mockResolvedValue("t");
      const actualizada = {
        ...ordenConDatos,
        diagnostico: "Conector roto, hay que cambiarlo",
        componentesAfectados: ["Conector de carga", "Batería"],
      };
      const fetchMock = stubDatosTecnicos(() => json(actualizada));

      render(<OrdenDetalle id="o1" />);

      fireEvent.change(await screen.findByLabelText("Diagnóstico"), { target: { value: "Conector roto" } });
      fireEvent.change(screen.getByRole("textbox", { name: "Componentes afectados" }), { target: { value: "Batería" } });
      fireEvent.click(screen.getByRole("button", { name: "Agregar" }));
      fireEvent.click(screen.getByRole("button", { name: "Guardar datos técnicos" }));

      expect(await screen.findByDisplayValue("Conector roto, hay que cambiarlo")).toBeInTheDocument();
      const put = fetchMock.mock.calls.find(([url, init]) => init?.method === "PUT" && url.endsWith("/reparaciones/o1/datos-tecnicos"));
      expect(put).toBeDefined();
      expect(JSON.parse(String(put![1]!.body))).toEqual({
        pruebas: "Probé el cargador original",
        diagnostico: "Conector roto",
        mediciones: null,
        componentesAfectados: ["Conector de carga", "Batería"],
        notasInternas: "Cliente apurado",
      });
      expect(screen.queryByRole("alert")).not.toBeInTheDocument();
    });

    it("muestra el error si el rol no tiene permiso, sin romper la pantalla", async () => {
      mocks.getIdToken.mockResolvedValue("t");
      stubDatosTecnicos(() => json({ title: "Forbidden", status: 403 }, 403));

      render(<OrdenDetalle id="o1" />);

      fireEvent.click(await screen.findByRole("button", { name: "Guardar datos técnicos" }));

      expect(await screen.findByRole("alert")).toHaveTextContent("Tu rol no permite editar los datos técnicos de la orden.");
      expect(screen.getByRole("heading", { name: "Orden 25" })).toBeInTheDocument();
      expect(screen.getByLabelText("Diagnóstico")).toHaveValue("Conector sulfatado");
    });

    it("muestra el título del error de validación del backend", async () => {
      mocks.getIdToken.mockResolvedValue("t");
      stubDatosTecnicos(() =>
        json({ title: "No se pueden cargar más de 20 componentes.", codigo: "datos_tecnicos_invalidos", status: 400 }, 400),
      );

      render(<OrdenDetalle id="o1" />);

      fireEvent.click(await screen.findByRole("button", { name: "Guardar datos técnicos" }));

      expect(await screen.findByRole("alert")).toHaveTextContent("No se pueden cargar más de 20 componentes.");
    });
  });

  describe("presupuesto", () => {
    const PERMISOS_PRESUPUESTO = ["reparaciones.ver", "presupuestos.gestionar"];
    const vigente = {
      id: "v2",
      numero: 2,
      items: [
        { descripcion: "Cambio de módulo", tipo: "repuesto", monto: 15000 },
        { descripcion: "Colocación", tipo: "mano_obra", monto: 2500.5 },
      ],
      total: 17500.5,
      creadoPor: "Técnico 1",
      creadoEn: "2026-11-08T12:00:00Z",
    };
    const anterior = {
      id: "v1",
      numero: 1,
      items: [{ descripcion: "Cambio de módulo", tipo: "repuesto", monto: 12000 }],
      total: 12000,
      creadoPor: "Técnico 1",
      creadoEn: "2026-11-07T12:00:00Z",
    };
    const ordenSinPresupuesto = { ...orden, estado: "esperando_presupuesto", estadosDisponibles: [], presupuestoVigente: null };

    /** El POST de presupuestos responde `respuestaPost`; el GET de presupuestos devuelve las dos versiones. */
    function stubPresupuesto(
      respuestaPost: () => Response,
      permisos: string[] = PERMISOS_PRESUPUESTO,
      ordenInicial: unknown = ordenSinPresupuesto,
    ) {
      const fetchMock = vi.fn<(url: string, init?: RequestInit) => Promise<Response>>((url, init) => {
        if (url.includes("/api/v1/me")) return Promise.resolve(json(me(permisos)));
        if (init?.method === "POST" && url.endsWith("/presupuestos")) return Promise.resolve(respuestaPost());
        if (url.endsWith("/presupuestos")) return Promise.resolve(json([vigente, anterior]));
        if (url.includes("/historial")) return Promise.resolve(json({ items: [] }));
        if (url.endsWith("/fotos")) return Promise.resolve(json([]));
        return Promise.resolve(json(ordenInicial));
      });
      vi.stubGlobal("fetch", fetchMock);
      return fetchMock;
    }

    it("sin el permiso no muestra el formulario para crear una versión", async () => {
      mocks.getIdToken.mockResolvedValue("t");
      stubPresupuesto(() => json({}), ["reparaciones.ver"]);

      render(<OrdenDetalle id="o1" />);

      const seccion = within(await screen.findByRole("region", { name: "Presupuesto" }));
      expect(seccion.queryByRole("group", { name: "Nueva versión del presupuesto" })).not.toBeInTheDocument();
      expect(seccion.queryByRole("button", { name: "Guardar presupuesto" })).not.toBeInTheDocument();
    });

    it("sin presupuesto lo avisa y no ofrece ver versiones anteriores", async () => {
      mocks.getIdToken.mockResolvedValue("t");
      stubPresupuesto(() => json({}));

      render(<OrdenDetalle id="o1" />);

      expect(await screen.findByText("Todavía no se presupuestó esta orden.")).toBeInTheDocument();
      expect(screen.queryByRole("button", { name: "Ver versiones anteriores" })).not.toBeInTheDocument();
    });

    it("muestra los ítems y el total de la versión vigente, y a pedido las anteriores", async () => {
      mocks.getIdToken.mockResolvedValue("t");
      const fetchMock = stubPresupuesto(() => json({}), ["reparaciones.ver"], { ...ordenSinPresupuesto, presupuestoVigente: vigente });

      render(<OrdenDetalle id="o1" />);

      const seccion = within(await screen.findByRole("region", { name: "Presupuesto" }));
      expect(seccion.getByText("Versión 2")).toBeInTheDocument();
      const items = within(seccion.getByRole("list", { name: "Ítems del presupuesto vigente" }))
        .getAllByRole("listitem")
        .map((li) => li.textContent);
      expect(items).toEqual([
        `Repuesto: Cambio de módulo — ${formatearMonto(15000)}`,
        `Mano de obra: Colocación — ${formatearMonto(2500.5)}`,
      ]);
      expect(seccion.getByText(montoEnPantalla(17500.5), { selector: "strong" })).toBeInTheDocument();
      expect(seccion.getByText(/Creado por Técnico 1 el/)).toBeInTheDocument();
      // Las versiones anteriores no se piden al cargar la pantalla.
      const pidioVersiones = () => fetchMock.mock.calls.some(([url, init]) => init?.method === "GET" && url.endsWith("/reparaciones/o1/presupuestos"));
      expect(pidioVersiones()).toBe(false);

      fireEvent.click(seccion.getByRole("button", { name: "Ver versiones anteriores" }));

      const lista = within(await seccion.findByRole("list", { name: "Versiones anteriores del presupuesto" }));
      expect(lista.getAllByRole("listitem")).toHaveLength(1);
      expect(lista.getByText(/Versión 1/)).toBeInTheDocument();
      expect(lista.getByText(montoEnPantalla(12000))).toBeInTheDocument();
      expect(pidioVersiones()).toBe(true);
    });

    it("con el permiso arma los ítems, muestra el total en vivo y guarda la versión", async () => {
      mocks.getIdToken.mockResolvedValue("t");
      const actualizada = { ...ordenSinPresupuesto, estado: "presupuesto_enviado", presupuestoVigente: { ...vigente, numero: 1, id: "v1" } };
      const fetchMock = stubPresupuesto(() => json(actualizada));

      render(<OrdenDetalle id="o1" />);

      const form = within(await screen.findByRole("group", { name: "Nueva versión del presupuesto" }));
      expect(form.getByRole("button", { name: "Guardar presupuesto" })).toBeDisabled();

      const item1 = within(form.getByRole("group", { name: "Ítem 1" }));
      fireEvent.change(item1.getByRole("combobox", { name: "Tipo" }), { target: { value: "repuesto" } });
      fireEvent.change(item1.getByRole("textbox", { name: "Descripción" }), { target: { value: "Cambio de módulo" } });
      fireEvent.change(item1.getByRole("spinbutton", { name: "Monto" }), { target: { value: "15000" } });

      fireEvent.click(form.getByRole("button", { name: "Agregar ítem" }));
      const item2 = within(form.getByRole("group", { name: "Ítem 2" }));
      fireEvent.change(item2.getByRole("textbox", { name: "Descripción" }), { target: { value: "Colocación" } });
      fireEvent.change(item2.getByRole("spinbutton", { name: "Monto" }), { target: { value: "2500.50" } });

      expect(form.getByText(montoEnPantalla(17500.5), { selector: "strong" })).toBeInTheDocument();
      fireEvent.click(form.getByRole("button", { name: "Guardar presupuesto" }));

      expect(await screen.findByText("Presupuesto enviado", { selector: ".estado-chip" })).toBeInTheDocument();
      const post = fetchMock.mock.calls.find(([url, init]) => init?.method === "POST" && url.endsWith("/reparaciones/o1/presupuestos"));
      expect(post).toBeDefined();
      expect(JSON.parse(String(post![1]!.body))).toEqual({
        items: [
          { tipo: "repuesto", descripcion: "Cambio de módulo", monto: 15000 },
          { tipo: "mano_obra", descripcion: "Colocación", monto: 2500.5 },
        ],
      });
      expect(screen.getByText("Versión 1")).toBeInTheDocument();
      // El formulario vuelve a una sola fila vacía.
      expect(form.queryByRole("group", { name: "Ítem 2" })).not.toBeInTheDocument();
      expect(within(form.getByRole("group", { name: "Ítem 1" })).getByRole("textbox", { name: "Descripción" })).toHaveValue("");
      expect(screen.queryByRole("alert")).not.toBeInTheDocument();
    });

    it("muestra tal cual el error de validación del backend", async () => {
      mocks.getIdToken.mockResolvedValue("t");
      stubPresupuesto(() =>
        json({ title: "La descripción de un ítem no puede superar los 200 caracteres.", codigo: "presupuesto_invalido", status: 400 }, 400),
      );

      render(<OrdenDetalle id="o1" />);

      const item1 = within(await screen.findByRole("group", { name: "Ítem 1" }));
      fireEvent.change(item1.getByRole("textbox", { name: "Descripción" }), { target: { value: "Cambio de módulo" } });
      fireEvent.change(item1.getByRole("spinbutton", { name: "Monto" }), { target: { value: "100" } });
      fireEvent.click(screen.getByRole("button", { name: "Guardar presupuesto" }));

      expect(await screen.findByRole("alert")).toHaveTextContent("La descripción de un ítem no puede superar los 200 caracteres.");
      expect(item1.getByRole("textbox", { name: "Descripción" })).toHaveValue("Cambio de módulo");
    });

    it("muestra un mensaje de permiso si el rol no puede presupuestar", async () => {
      mocks.getIdToken.mockResolvedValue("t");
      stubPresupuesto(() => json({ title: "Forbidden", status: 403 }, 403));

      render(<OrdenDetalle id="o1" />);

      const item1 = within(await screen.findByRole("group", { name: "Ítem 1" }));
      fireEvent.change(item1.getByRole("textbox", { name: "Descripción" }), { target: { value: "Cambio de módulo" } });
      fireEvent.change(item1.getByRole("spinbutton", { name: "Monto" }), { target: { value: "100" } });
      fireEvent.click(screen.getByRole("button", { name: "Guardar presupuesto" }));

      expect(await screen.findByRole("alert")).toHaveTextContent("Tu rol no permite crear presupuestos de la orden.");
      expect(screen.getByRole("heading", { name: "Orden 25" })).toBeInTheDocument();
    });
  });

  it("muestra en el historial la creación de una versión del presupuesto", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    stubCambioDeEstado(() => json({}), { ...orden, estadosDisponibles: [] }, [
      {
        id: "r1",
        accion: "presupuesto_creado",
        usuario: "Técnico 1",
        fecha: "2026-11-08T12:00:00Z",
        cambios: [
          { campo: "version_presupuesto", anterior: "1", nuevo: "2" },
          { campo: "total_presupuesto", anterior: "12000.00", nuevo: "17500.50" },
        ],
      },
    ]);

    render(<OrdenDetalle id="o1" />);

    const historial = within(await screen.findByRole("region", { name: "Historial de la orden" }));
    expect(
      historial.getByText(
        (_, el) => el?.tagName === "SPAN" && el.textContent === `Creó la versión 2 del presupuesto por ${formatearMonto(17500.5)}`,
      ),
    ).toBeInTheDocument();
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
