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

import { ClientesPage } from "./clientes-page";

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

function pagina(items: unknown[], totalCount = items.length, page = 1, totalPages = 1) {
  return { items, page, pageSize: 20, totalCount, totalPages };
}

function me(permisos: string[]) {
  return { uid: "u", email: "dueno@puntoios.com", emailVerificado: true, nombre: null, usuarioId: "id", rol: "Administrador", permisos };
}

type Handler = (url: string, init?: RequestInit) => Response | Promise<Response>;

/** Responde según la ruta; devuelve el mock para inspeccionar las llamadas. */
function stubApi(handler: Handler) {
  const fetchMock = vi.fn((url: string, init?: RequestInit) => Promise.resolve(handler(url, init)));
  vi.stubGlobal("fetch", fetchMock);
  return fetchMock;
}

const json = (cuerpo: unknown, status = 200) => new Response(JSON.stringify(cuerpo), { status });

afterEach(() => {
  cleanup();
  vi.unstubAllGlobals();
  vi.clearAllMocks();
});

describe("ClientesPage", () => {
  it("lista los clientes con el token de la sesión", async () => {
    mocks.getIdToken.mockResolvedValue("token-123");
    const fetchMock = stubApi((url) =>
      url.includes("/api/v1/me") ? json(me(["clientes.ver"])) : json(pagina([ana])),
    );

    render(<ClientesPage />);

    expect(await screen.findByText("Pérez, Ana")).toBeInTheDocument();
    expect(screen.getByText("1 cliente")).toBeInTheDocument();
    const llamada = fetchMock.mock.calls.find(([url]) => url.includes("/api/v1/clientes"))!;
    expect((llamada[1]!.headers as Record<string, string>).Authorization).toBe("Bearer token-123");
  });

  it("busca al enviar el formulario de búsqueda", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    const fetchMock = stubApi((url) =>
      url.includes("/api/v1/me") ? json(me(["clientes.ver"])) : json(pagina([ana])),
    );
    render(<ClientesPage />);
    await screen.findByText("Pérez, Ana");

    fireEvent.change(screen.getByLabelText("Buscar clientes"), { target: { value: "ana gómez" } });
    fireEvent.click(screen.getByRole("button", { name: "Buscar" }));

    await waitFor(() =>
      expect(fetchMock.mock.calls.some(([url]) => url.includes("q=ana+g%C3%B3mez"))).toBe(true),
    );
  });

  it("explica cuando no hay resultados", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    stubApi((url) => (url.includes("/api/v1/me") ? json(me(["clientes.ver"])) : json(pagina([]))));

    render(<ClientesPage />);

    expect(await screen.findByText("Todavía no hay clientes cargados.")).toBeInTheDocument();
  });

  it("oculta 'Nuevo cliente' si el rol no puede crear", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    stubApi((url) => (url.includes("/api/v1/me") ? json(me(["clientes.ver"])) : json(pagina([ana]))));

    render(<ClientesPage />);
    await screen.findByText("Pérez, Ana");

    expect(screen.queryByRole("button", { name: "Nuevo cliente" })).not.toBeInTheDocument();
  });

  it("avisa si el rol no permite ver clientes (403)", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    stubApi((url) => (url.includes("/api/v1/me") ? json(me([])) : new Response("", { status: 403 })));

    render(<ClientesPage />);

    expect(await screen.findByText("Tu rol no permite ver clientes.")).toBeInTheDocument();
  });

  it("registra un cliente nuevo y refresca la lista", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    const fetchMock = stubApi((url, init) => {
      if (url.includes("/api/v1/me")) return json(me(["clientes.ver", "clientes.crear"]));
      if (init?.method === "POST") return json({ ...ana, id: "c2", nombre: "Luis", apellido: "Gómez" }, 201);
      return json(pagina([ana]));
    });
    render(<ClientesPage />);
    await screen.findByText("Pérez, Ana");

    fireEvent.click(await screen.findByRole("button", { name: "Nuevo cliente" }));
    const form = screen.getByRole("form", { name: "Nuevo cliente" });
    fireEvent.change(within(form).getByLabelText("Nombre"), { target: { value: "Luis" } });
    fireEvent.change(within(form).getByLabelText("Apellido"), { target: { value: "Gómez" } });
    fireEvent.change(within(form).getByLabelText("Teléfono"), { target: { value: "3498 222222" } });
    fireEvent.click(within(form).getByRole("button", { name: "Guardar cliente" }));

    expect(await screen.findByText("Cliente guardado: Gómez, Luis.")).toBeInTheDocument();
    const post = fetchMock.mock.calls.find(([, init]) => init?.method === "POST")!;
    expect(JSON.parse(post[1]!.body as string)).toMatchObject({
      nombre: "Luis",
      apellido: "Gómez",
      telefono: "3498 222222",
      permitirDuplicado: false,
    });
    expect(screen.queryByRole("form", { name: "Nuevo cliente" })).not.toBeInTheDocument();
  });

  it("si el teléfono ya existe muestra quién lo tiene y permite guardarlo igual", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    let intentos = 0;
    const fetchMock = stubApi((url, init) => {
      if (url.includes("/api/v1/me")) return json(me(["clientes.ver", "clientes.crear"]));
      if (init?.method === "POST") {
        intentos += 1;
        return intentos === 1
          ? json(
              {
                title: "Ya hay un cliente con ese teléfono.",
                codigo: "telefono_duplicado",
                coincidencias: [{ id: "c1", nombre: "Ana", apellido: "Pérez", telefono: "3498 111111" }],
              },
              409,
            )
          : json({ ...ana, id: "c3", nombre: "Marta", apellido: "Pérez" }, 201);
      }
      return json(pagina([ana]));
    });
    render(<ClientesPage />);
    await screen.findByText("Pérez, Ana");

    fireEvent.click(await screen.findByRole("button", { name: "Nuevo cliente" }));
    const form = screen.getByRole("form", { name: "Nuevo cliente" });
    fireEvent.change(within(form).getByLabelText("Nombre"), { target: { value: "Marta" } });
    fireEvent.change(within(form).getByLabelText("Apellido"), { target: { value: "Pérez" } });
    fireEvent.change(within(form).getByLabelText("Teléfono"), { target: { value: "3498 111111" } });
    fireEvent.click(within(form).getByRole("button", { name: "Guardar cliente" }));

    const aviso = await screen.findByText(/Ya hay un cliente con ese teléfono:/);
    expect(aviso).toBeInTheDocument();
    expect(within(form).getByText(/Pérez, Ana/)).toBeInTheDocument();

    fireEvent.click(within(form).getByRole("button", { name: "Guardar igual" }));

    expect(await screen.findByText("Cliente guardado: Pérez, Marta.")).toBeInTheDocument();
    const posts = fetchMock.mock.calls.filter(([, init]) => init?.method === "POST");
    expect(JSON.parse(posts[1][1]!.body as string).permitirDuplicado).toBe(true);
  });

  it("muestra el mensaje del servidor si los datos son inválidos", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    stubApi((url, init) => {
      if (url.includes("/api/v1/me")) return json(me(["clientes.ver", "clientes.crear"]));
      if (init?.method === "POST") return json({ title: "El teléfono debe tener entre 6 y 15 dígitos.", codigo: "telefono_invalido" }, 400);
      return json(pagina([]));
    });
    render(<ClientesPage />);

    fireEvent.click(await screen.findByRole("button", { name: "Nuevo cliente" }));
    const form = screen.getByRole("form", { name: "Nuevo cliente" });
    fireEvent.change(within(form).getByLabelText("Nombre"), { target: { value: "Ana" } });
    fireEvent.change(within(form).getByLabelText("Apellido"), { target: { value: "Pérez" } });
    fireEvent.change(within(form).getByLabelText("Teléfono"), { target: { value: "123" } });
    fireEvent.click(within(form).getByRole("button", { name: "Guardar cliente" }));

    expect(await screen.findByText("El teléfono debe tener entre 6 y 15 dígitos.")).toBeInTheDocument();
  });
});
