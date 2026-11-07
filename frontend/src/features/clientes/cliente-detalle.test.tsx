import { cleanup, fireEvent, render, screen, waitFor, within } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";

const mocks = vi.hoisted(() => ({
  getIdToken: vi.fn(),
  signOut: vi.fn(),
}));

vi.mock("next/navigation", () => ({ useRouter: () => ({ replace: vi.fn() }) }));
vi.mock("@/features/auth/auth-provider", () => ({
  useAuth: () => ({ user: { email: "dueno@puntoios.com", getIdToken: mocks.getIdToken }, signOut: mocks.signOut }),
}));

import { ClienteDetalle } from "./cliente-detalle";

const ana = {
  id: "c1",
  nombre: "Ana",
  apellido: "Pérez",
  telefono: "3498 111111",
  email: "ana@mail.com",
  direccion: null,
  observaciones: null,
  creadoEn: "2026-11-07T12:00:00Z",
};

const historial = {
  items: [
    {
      id: "h2",
      accion: "editado",
      usuarioId: "u1",
      usuario: "Recepción 1",
      fecha: "2026-11-08T12:00:00Z",
      cambios: [{ campo: "apellido", anterior: "Pérez", nuevo: "Gómez" }],
    },
    { id: "h1", accion: "creado", usuarioId: "u1", usuario: "Recepción 1", fecha: "2026-11-07T12:00:00Z", cambios: [] },
  ],
  page: 1,
  pageSize: 50,
  totalCount: 2,
  totalPages: 1,
};

function me(permisos: string[]) {
  return { uid: "u", email: "dueno@puntoios.com", emailVerificado: true, nombre: null, usuarioId: "id", rol: "Administrador", permisos };
}

const json = (cuerpo: unknown, status = 200) => new Response(JSON.stringify(cuerpo), { status });

type Handler = (url: string, init?: RequestInit) => Response;

function stubApi(handler: Handler) {
  const fetchMock = vi.fn((url: string, init?: RequestInit) => Promise.resolve(handler(url, init)));
  vi.stubGlobal("fetch", fetchMock);
  return fetchMock;
}

afterEach(() => {
  cleanup();
  vi.unstubAllGlobals();
  vi.clearAllMocks();
});

describe("ClienteDetalle", () => {
  it("muestra los datos y el historial con quién cambió qué", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    stubApi((url) => {
      if (url.includes("/api/v1/me")) return json(me(["clientes.ver"]));
      if (url.includes("/historial")) return json(historial);
      return json(ana);
    });

    render(<ClienteDetalle id="c1" />);

    expect(await screen.findByRole("heading", { name: "Pérez, Ana" })).toBeInTheDocument();
    expect(screen.getByText("3498 111111")).toBeInTheDocument();
    const lista = within(screen.getByRole("region", { name: "Historial de cambios" }));
    expect(lista.getByText("Datos editados")).toBeInTheDocument();
    expect(lista.getByText("Cliente creado")).toBeInTheDocument();
    expect(lista.getByText("Gómez")).toBeInTheDocument();
    expect(lista.getAllByText(/Recepción 1/)).toHaveLength(2);
  });

  it("sin permiso de edición no muestra el botón de editar", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    stubApi((url) => {
      if (url.includes("/api/v1/me")) return json(me(["clientes.ver"]));
      if (url.includes("/historial")) return json(historial);
      return json(ana);
    });

    render(<ClienteDetalle id="c1" />);
    await screen.findByRole("heading", { name: "Pérez, Ana" });

    expect(screen.queryByRole("button", { name: "Editar cliente" })).not.toBeInTheDocument();
  });

  it("edita el cliente, envía un PUT y recarga el historial", async () => {
    mocks.getIdToken.mockResolvedValue("token-9");
    let apellido = "Pérez";
    const fetchMock = stubApi((url, init) => {
      if (url.includes("/api/v1/me")) return json(me(["clientes.ver", "clientes.editar"]));
      if (init?.method === "PUT") {
        apellido = "Gómez";
        return json({ ...ana, apellido });
      }
      if (url.includes("/historial")) return json(historial);
      return json({ ...ana, apellido });
    });

    render(<ClienteDetalle id="c1" />);
    fireEvent.click(await screen.findByRole("button", { name: "Editar cliente" }));
    fireEvent.change(screen.getByLabelText("Apellido"), { target: { value: "Gómez" } });
    fireEvent.click(screen.getByRole("button", { name: "Guardar cambios" }));

    expect(await screen.findByRole("heading", { name: "Gómez, Ana" })).toBeInTheDocument();
    expect(screen.getByText(/Cambios guardados/)).toBeInTheDocument();
    const put = fetchMock.mock.calls.find(([, init]) => init?.method === "PUT")!;
    expect(put[0]).toContain("/api/v1/clientes/c1");
    expect(JSON.parse(put[1]!.body as string)).toMatchObject({ nombre: "Ana", apellido: "Gómez", telefono: "3498 111111" });
    expect((put[1]!.headers as Record<string, string>).Authorization).toBe("Bearer token-9");
  });

  it("avisa si el teléfono nuevo ya lo tiene otro cliente y permite guardar igual", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    const puts: unknown[] = [];
    stubApi((url, init) => {
      if (url.includes("/api/v1/me")) return json(me(["clientes.ver", "clientes.editar"]));
      if (init?.method === "PUT") {
        const cuerpo = JSON.parse(init.body as string);
        puts.push(cuerpo);
        return cuerpo.permitirDuplicado
          ? json(ana)
          : json(
              {
                title: "Teléfono repetido",
                codigo: "telefono_duplicado",
                coincidencias: [{ id: "c2", nombre: "Luis", apellido: "Gómez", telefono: "3498 222222" }],
              },
              409,
            );
      }
      if (url.includes("/historial")) return json(historial);
      return json(ana);
    });

    render(<ClienteDetalle id="c1" />);
    fireEvent.click(await screen.findByRole("button", { name: "Editar cliente" }));
    fireEvent.change(screen.getByLabelText("Teléfono"), { target: { value: "3498 222222" } });
    fireEvent.click(screen.getByRole("button", { name: "Guardar cambios" }));

    expect(await screen.findByText(/Gómez, Luis/)).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Guardar igual" }));

    await waitFor(() => expect(puts).toHaveLength(2));
    expect(puts[1]).toMatchObject({ permitirDuplicado: true });
    expect(await screen.findByText(/Cambios guardados/)).toBeInTheDocument();
  });

  it("muestra un mensaje si el cliente no existe", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    stubApi((url) =>
      url.includes("/api/v1/me") ? json(me(["clientes.ver"])) : json({ title: "No existe", codigo: "cliente_inexistente" }, 404),
    );

    render(<ClienteDetalle id="nada" />);

    expect(await screen.findByText("No se encontró el cliente.")).toBeInTheDocument();
  });
});
