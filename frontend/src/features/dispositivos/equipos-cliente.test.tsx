import { cleanup, fireEvent, render, screen, waitFor } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";

const mocks = vi.hoisted(() => ({ getIdToken: vi.fn() }));

vi.mock("@/features/auth/auth-provider", () => ({
  useAuth: () => ({ user: { email: "a@b.com", getIdToken: mocks.getIdToken } }),
}));

import { EquiposCliente } from "./equipos-cliente";

const iphone = {
  id: "d1",
  clienteId: "c1",
  marca: "Apple",
  modelo: "iPhone 13",
  almacenamiento: "128 GB",
  color: "Azul",
  imeiSerieEnmascarado: "•••••••••••7518",
  observaciones: null,
  creadoEn: "2026-11-07T12:00:00Z",
};

const json = (cuerpo: unknown, status = 200) => new Response(JSON.stringify(cuerpo), { status });

function stubApi(handler: (url: string, init?: RequestInit) => Response) {
  const fetchMock = vi.fn((url: string, init?: RequestInit) => Promise.resolve(handler(url, init)));
  vi.stubGlobal("fetch", fetchMock);
  return fetchMock;
}

const TODOS = ["dispositivos.ver", "dispositivos.crear", "dispositivos.editar", "dispositivos.ver_imei"];

afterEach(() => {
  cleanup();
  vi.unstubAllGlobals();
  vi.clearAllMocks();
});

describe("EquiposCliente", () => {
  it("lista los equipos con el IMEI enmascarado", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    stubApi(() => json([iphone]));

    render(<EquiposCliente clienteId="c1" permisos={["dispositivos.ver"]} />);

    expect(await screen.findByText("Apple iPhone 13")).toBeInTheDocument();
    expect(screen.getByText("128 GB · Azul")).toBeInTheDocument();
    expect(screen.getByLabelText("IMEI enmascarado")).toHaveTextContent("•••••••••••7518");
    expect(screen.queryByRole("button", { name: /Ver IMEI/ })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Agregar equipo" })).not.toBeInTheDocument();
    expect(screen.queryByRole("button", { name: /Editar/ })).not.toBeInTheDocument();
  });

  it("con permiso muestra el IMEI completo pidiéndolo al servidor y lo vuelve a ocultar", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    const fetchMock = stubApi((url, init) =>
      init?.method === "POST" && url.endsWith("/api/v1/dispositivos/d1/imei")
        ? json({ imeiSerie: "490154203237518" })
        : json([iphone]),
    );

    render(<EquiposCliente clienteId="c1" permisos={TODOS} />);
    fireEvent.click(await screen.findByRole("button", { name: "Ver IMEI completo de Apple iPhone 13" }));

    expect(await screen.findByLabelText("IMEI completo")).toHaveTextContent("490154203237518");
    expect(fetchMock.mock.calls.some(([, init]) => init?.method === "POST")).toBe(true);

    fireEvent.click(screen.getByRole("button", { name: "Ocultar" }));
    expect(screen.getByLabelText("IMEI enmascarado")).toHaveTextContent("•••••••••••7518");
  });

  it("agrega un equipo con Apple como marca por defecto", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    let lista = [] as (typeof iphone)[];
    const fetchMock = stubApi((_url, init) => {
      if (init?.method === "POST") {
        lista = [iphone];
        return json(iphone, 201);
      }
      return json(lista);
    });

    render(<EquiposCliente clienteId="c1" permisos={TODOS} />);
    await screen.findByText("Todavía no tiene equipos registrados.");
    fireEvent.click(screen.getByRole("button", { name: "Agregar equipo" }));

    expect(screen.getByLabelText("Marca")).toHaveValue("Apple");
    fireEvent.change(screen.getByLabelText("Modelo"), { target: { value: "iPhone 13" } });
    fireEvent.change(screen.getByLabelText("IMEI o número de serie (opcional)"), { target: { value: "490154203237518" } });
    fireEvent.click(screen.getByRole("button", { name: "Guardar equipo" }));

    expect(await screen.findByText("Equipo agregado: Apple iPhone 13.")).toBeInTheDocument();
    const post = fetchMock.mock.calls.find(([, init]) => init?.method === "POST")!;
    expect(post[0]).toContain("/api/v1/clientes/c1/dispositivos");
    expect(JSON.parse(post[1]!.body as string)).toMatchObject({ marca: "Apple", modelo: "iPhone 13", imeiSerie: "490154203237518" });
  });

  it("un IMEI repetido no se guarda y muestra de quién es", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    stubApi((_url, init) =>
      init?.method === "POST"
        ? json(
            {
              title: "Ese IMEI o número de serie ya está registrado en otro equipo.",
              codigo: "imei_duplicado",
              cliente: { id: "c9", nombre: "Luis", apellido: "Gómez", telefono: "3498 222222" },
            },
            409,
          )
        : json([]),
    );

    render(<EquiposCliente clienteId="c1" permisos={TODOS} />);
    fireEvent.click(await screen.findByRole("button", { name: "Agregar equipo" }));
    fireEvent.change(screen.getByLabelText("Modelo"), { target: { value: "iPhone 13" } });
    fireEvent.change(screen.getByLabelText("IMEI o número de serie (opcional)"), { target: { value: "490154203237518" } });
    fireEvent.click(screen.getByRole("button", { name: "Guardar equipo" }));

    const enlace = await screen.findByRole("link", { name: "Gómez, Luis" });
    expect(enlace).toHaveAttribute("href", "/clientes/c9");
    expect(screen.queryByRole("button", { name: "Guardar igual" })).not.toBeInTheDocument();
  });

  it("al editar sin escribir un IMEI nuevo no lo modifica", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    const fetchMock = stubApi((_url, init) =>
      init?.method === "PUT" ? json({ ...iphone, modelo: "iPhone 13 Pro" }) : json([iphone]),
    );

    render(<EquiposCliente clienteId="c1" permisos={TODOS} />);
    fireEvent.click(await screen.findByRole("button", { name: "Editar Apple iPhone 13" }));

    expect(screen.getByPlaceholderText("Sin cambios (termina en 7518)")).toHaveValue("");
    fireEvent.change(screen.getByLabelText("Modelo"), { target: { value: "iPhone 13 Pro" } });
    fireEvent.click(screen.getByRole("button", { name: "Guardar cambios" }));

    await waitFor(() => expect(fetchMock.mock.calls.some(([, init]) => init?.method === "PUT")).toBe(true));
    const put = fetchMock.mock.calls.find(([, init]) => init?.method === "PUT")!;
    expect(put[0]).toContain("/api/v1/dispositivos/d1");
    expect(JSON.parse(put[1]!.body as string)).toMatchObject({ modelo: "iPhone 13 Pro", actualizarImei: false });
    expect(await screen.findByText("Equipo actualizado: Apple iPhone 13 Pro.")).toBeInTheDocument();
  });
});
