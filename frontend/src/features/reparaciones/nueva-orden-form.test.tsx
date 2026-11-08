import { cleanup, fireEvent, render, screen, waitFor, within } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";

const mocks = vi.hoisted(() => ({ getIdToken: vi.fn() }));

vi.mock("@/features/auth/auth-provider", () => ({
  useAuth: () => ({ user: { email: "a@b.com", getIdToken: mocks.getIdToken } }),
}));

import { NuevaOrdenForm } from "./nueva-orden-form";

const ana = { id: "c1", nombre: "Ana", apellido: "Pérez" };
const iphone = {
  id: "d1",
  clienteId: "c1",
  marca: "Apple",
  modelo: "iPhone 13",
  almacenamiento: "128 GB",
  color: null,
  imeiSerieEnmascarado: "•••••••••••7518",
  observaciones: null,
  creadoEn: "2026-11-07T12:00:00Z",
};

const json = (cuerpo: unknown, status = 200) => new Response(JSON.stringify(cuerpo), { status });

function stubApi(equipos: unknown[], respuestaPost: () => Response = () => json({ id: "o1", numero: 1 }, 201)) {
  const fetchMock = vi.fn<(url: string, init?: RequestInit) => Promise<Response>>((url, init) => {
    if (init?.method === "POST") return Promise.resolve(respuestaPost());
    if (url.includes("/tecnicos")) return Promise.resolve(json([{ id: "t1", nombre: "Técnico 1" }]));
    return Promise.resolve(json(equipos));
  });
  vi.stubGlobal("fetch", fetchMock);
  return fetchMock;
}

const cuerpoDelPost = (fetchMock: ReturnType<typeof stubApi>) =>
  JSON.parse(fetchMock.mock.calls.find(([, init]) => init?.method === "POST")![1]!.body as string);

afterEach(() => {
  cleanup();
  vi.unstubAllGlobals();
  vi.clearAllMocks();
});

describe("NuevaOrdenForm", () => {
  it("con un solo equipo lo elige solo y envía la orden con patrón, técnico y tipos", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    const fetchMock = stubApi([iphone]);
    const onCreada = vi.fn();
    render(<NuevaOrdenForm cliente={ana} onCreada={onCreada} onCancelar={vi.fn()} />);

    expect(await screen.findByRole("radio", { name: /iPhone 13/ })).toBeChecked();
    fireEvent.change(screen.getByLabelText("Problema que cuenta el cliente"), { target: { value: "No carga" } });
    fireEvent.click(screen.getByRole("checkbox", { name: "Conector de carga" }));
    fireEvent.click(screen.getByRole("checkbox", { name: "Batería" }));
    await screen.findByRole("option", { name: "Técnico 1" });
    fireEvent.change(screen.getByLabelText("Técnico"), { target: { value: "t1" } });
    fireEvent.click(screen.getByRole("radio", { name: "Patrón" }));
    for (const p of [1, 2, 3, 6]) fireEvent.click(screen.getByRole("button", { name: `Punto ${p}` }));
    fireEvent.click(screen.getByRole("button", { name: "Recibir equipo" }));

    await waitFor(() => expect(onCreada).toHaveBeenCalled());
    expect(cuerpoDelPost(fetchMock)).toEqual({
      clienteId: "c1",
      dispositivoId: "d1",
      problemaInformado: "No carga",
      tiposReparacion: ["conector_carga", "bateria"],
      tecnicoId: "t1",
      desbloqueoTipo: "patron",
      desbloqueoValor: "1-2-3-6",
    });
  });

  it("si el cliente no tiene equipos pide cargarlo y lo envía como equipo nuevo", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    const fetchMock = stubApi([]);
    render(<NuevaOrdenForm cliente={ana} onCreada={vi.fn()} onCancelar={vi.fn()} />);

    expect(await screen.findByRole("radio", { name: "Cargar el equipo" })).toBeChecked();
    const form = screen.getByRole("form", { name: "Recibir equipo" });
    fireEvent.change(within(form).getByLabelText("Modelo"), { target: { value: "iPhone 15" } });
    fireEvent.change(screen.getByLabelText("Problema que cuenta el cliente"), { target: { value: "Pantalla rota" } });
    fireEvent.click(screen.getByRole("checkbox", { name: "Pantalla" }));
    fireEvent.click(screen.getByRole("button", { name: "Recibir equipo" }));

    await waitFor(() => expect(fetchMock.mock.calls.some(([, init]) => init?.method === "POST")).toBe(true));
    expect(cuerpoDelPost(fetchMock)).toMatchObject({ equipoNuevo: { marca: "Apple", modelo: "iPhone 15" } });
    expect(cuerpoDelPost(fetchMock)).not.toHaveProperty("dispositivoId");
  });

  it("muestra el aviso de que la fecha estimada es orientativa", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    stubApi([iphone]);
    render(<NuevaOrdenForm cliente={ana} onCreada={vi.fn()} onCancelar={vi.fn()} />);

    const fecha = await screen.findByLabelText("Fecha estimada de entrega (opcional)");
    expect(fecha).toHaveAccessibleDescription(/puede cambiar y se le avisa/);
  });

  it("sin tipo de reparación no envía y lo pide", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    const fetchMock = stubApi([iphone]);
    render(<NuevaOrdenForm cliente={ana} onCreada={vi.fn()} onCancelar={vi.fn()} />);
    await screen.findByRole("radio", { name: /iPhone 13/ });

    fireEvent.change(screen.getByLabelText("Problema que cuenta el cliente"), { target: { value: "No carga" } });
    fireEvent.click(screen.getByRole("button", { name: "Recibir equipo" }));

    expect(await screen.findByText("Marcá al menos un tipo de reparación.")).toBeInTheDocument();
    expect(fetchMock.mock.calls.some(([, init]) => init?.method === "POST")).toBe(false);
  });

  it("si el IMEI del equipo nuevo ya existe muestra de quién es", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    stubApi([], () =>
      json({ title: "Repetido", codigo: "imei_duplicado", cliente: { id: "c9", nombre: "Luis", apellido: "Gómez", telefono: "1" } }, 409),
    );
    render(<NuevaOrdenForm cliente={ana} onCreada={vi.fn()} onCancelar={vi.fn()} />);
    await screen.findByRole("radio", { name: "Cargar el equipo" });

    const form = screen.getByRole("form", { name: "Recibir equipo" });
    fireEvent.change(within(form).getByLabelText("Modelo"), { target: { value: "iPhone 15" } });
    fireEvent.change(screen.getByLabelText("Problema que cuenta el cliente"), { target: { value: "x" } });
    fireEvent.click(screen.getByRole("checkbox", { name: "Pantalla" }));
    fireEvent.click(screen.getByRole("button", { name: "Recibir equipo" }));

    expect(await screen.findByRole("link", { name: "Gómez, Luis" })).toHaveAttribute("href", "/clientes/c9");
  });

  it("si enciende se marca en No, oculta la fecha y avisa que no se puede determinar", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    const fetchMock = stubApi([iphone]);
    render(<NuevaOrdenForm cliente={ana} onCreada={vi.fn()} onCancelar={vi.fn()} />);
    await screen.findByRole("radio", { name: /iPhone 13/ });

    expect(screen.getByLabelText("Fecha estimada de entrega (opcional)")).toBeInTheDocument();

    fireEvent.click(within(screen.getByRole("group", { name: "Enciende" })).getByRole("button", { name: "No" }));

    expect(screen.queryByLabelText("Fecha estimada de entrega (opcional)")).not.toBeInTheDocument();
    expect(screen.getByText("No se puede determinar")).toBeInTheDocument();
    expect(screen.getByRole("alert")).toHaveTextContent("no enciende");

    fireEvent.change(screen.getByLabelText("Problema que cuenta el cliente"), { target: { value: "No prende" } });
    fireEvent.click(screen.getByRole("checkbox", { name: "Diagnóstico" }));
    fireEvent.click(screen.getByRole("button", { name: "Recibir equipo" }));

    await waitFor(() => expect(fetchMock.mock.calls.some(([, init]) => init?.method === "POST")).toBe(true));
    const cuerpo = cuerpoDelPost(fetchMock);
    expect(cuerpo).not.toHaveProperty("fechaEstimadaEntrega");
    expect(cuerpo.checklist).toEqual({ items: { enciende: false } });
  });

  it("vuelve a tocar el mismo botón del checklist para dejarlo sin revisar", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    stubApi([iphone]);
    render(<NuevaOrdenForm cliente={ana} onCreada={vi.fn()} onCancelar={vi.fn()} />);
    await screen.findByRole("radio", { name: /iPhone 13/ });

    const botonSi = within(screen.getByRole("group", { name: "Carga" })).getByRole("button", { name: "Sí" });
    fireEvent.click(botonSi);
    expect(botonSi).toHaveAttribute("aria-pressed", "true");
    fireEvent.click(botonSi);
    expect(botonSi).toHaveAttribute("aria-pressed", "false");
  });

  it("sube las fotos elegidas después de crear la orden", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    const fetchMock = stubApi([iphone]);
    render(<NuevaOrdenForm cliente={ana} onCreada={vi.fn()} onCancelar={vi.fn()} />);
    await screen.findByRole("radio", { name: /iPhone 13/ });

    const archivo = new File(["contenido"], "foto.jpg", { type: "image/jpeg" });
    fireEvent.change(screen.getByLabelText("Elegir fotos"), { target: { files: [archivo] } });
    expect(await screen.findByText("foto.jpg")).toBeInTheDocument();

    fireEvent.change(screen.getByLabelText("Problema que cuenta el cliente"), { target: { value: "No carga" } });
    fireEvent.click(screen.getByRole("checkbox", { name: "Conector de carga" }));
    fireEvent.click(screen.getByRole("button", { name: "Recibir equipo" }));

    await waitFor(() =>
      expect(fetchMock.mock.calls.some(([url, init]) => init?.method === "POST" && url.toString().includes("/fotos"))).toBe(true),
    );
  });
});
