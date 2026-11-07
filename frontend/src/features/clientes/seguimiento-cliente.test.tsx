import { cleanup, fireEvent, render, screen, waitFor } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";

const mocks = vi.hoisted(() => ({ getIdToken: vi.fn() }));

vi.mock("@/features/auth/auth-provider", () => ({
  useAuth: () => ({ user: { email: "a@b.com", getIdToken: mocks.getIdToken } }),
}));

import { SeguimientoCliente } from "./seguimiento-cliente";

const CODIGO = "ABCDEFGHJKMNPQRSTVWXYZ012345";
const respuestaCodigo = { codigo: CODIGO, formateado: "ABCD-EFGH-JKMN-PQRS-TVWX-YZ01-2345", generadoEn: "2026-11-07T12:00:00Z" };

const ana = {
  id: "c1",
  nombre: "Ana",
  apellido: "Pérez",
  telefono: "3498 469259",
  email: null,
  direccion: null,
  observaciones: null,
  creadoEn: "2026-11-07T12:00:00Z",
};

const json = (cuerpo: unknown, status = 200) => new Response(JSON.stringify(cuerpo), { status });

function stubApi() {
  const fetchMock = vi.fn<(url: string, init?: RequestInit) => Promise<Response>>(() => Promise.resolve(json(respuestaCodigo)));
  vi.stubGlobal("fetch", fetchMock);
  return fetchMock;
}

afterEach(() => {
  cleanup();
  vi.unstubAllGlobals();
  vi.restoreAllMocks();
  vi.clearAllMocks();
});

describe("SeguimientoCliente", () => {
  it("si no tiene código ofrece generarlo y lo muestra", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    const fetchMock = stubApi();
    const onCambio = vi.fn();

    render(<SeguimientoCliente cliente={ana} onCambio={onCambio} />);
    fireEvent.click(screen.getByRole("button", { name: "Generar código" }));

    expect(await screen.findByLabelText("Código de seguimiento")).toHaveTextContent(respuestaCodigo.formateado);
    expect(fetchMock.mock.calls[0][0]).toContain("/api/v1/clientes/c1/seguimiento");
    expect(onCambio).toHaveBeenCalled();
  });

  it("con código, el botón de WhatsApp consulta el código y abre el chat con el link", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    const fetchMock = stubApi();
    const pestana = { opener: {}, location: { href: "" }, close: vi.fn() };
    vi.spyOn(window, "open").mockReturnValue(pestana as unknown as Window);

    render(<SeguimientoCliente cliente={{ ...ana, codigoSeguimientoGeneradoEn: "2026-11-07T12:00:00Z" }} onCambio={vi.fn()} />);
    expect(screen.queryByLabelText("Código de seguimiento")).not.toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Enviar por WhatsApp" }));

    await waitFor(() => expect(pestana.location.href).not.toBe(""));
    expect(fetchMock.mock.calls[0][0]).toContain("/api/v1/clientes/c1/seguimiento/consultar");
    expect(pestana.location.href).toMatch(/^https:\/\/wa\.me\/5493498469259\?text=/);
    expect(decodeURIComponent(pestana.location.href)).toContain(`/seguimiento/${CODIGO}`);
    expect(pestana.opener).toBeNull();
  });

  it("regenerar pide confirmación antes de invalidar el link anterior", async () => {
    mocks.getIdToken.mockResolvedValue("t");
    const fetchMock = stubApi();

    render(<SeguimientoCliente cliente={{ ...ana, codigoSeguimientoGeneradoEn: "2026-11-07T12:00:00Z" }} onCambio={vi.fn()} />);
    fireEvent.click(screen.getByRole("button", { name: "Regenerar" }));

    expect(screen.getByText(/deja de funcionar/)).toBeInTheDocument();
    expect(fetchMock).not.toHaveBeenCalled();

    fireEvent.click(screen.getByRole("button", { name: "Sí, regenerar" }));
    expect(await screen.findByText("Código regenerado. El link anterior ya no funciona.")).toBeInTheDocument();
    expect(fetchMock.mock.calls[0][0]).toMatch(/\/seguimiento$/);
  });

  it("avisa si el teléfono no permite abrir el chat directo", () => {
    render(
      <SeguimientoCliente
        cliente={{ ...ana, telefono: "15 469259", codigoSeguimientoGeneradoEn: "2026-11-07T12:00:00Z" }}
        onCambio={vi.fn()}
      />,
    );

    expect(screen.getByText(/WhatsApp te va a pedir que elijas el contacto/)).toBeInTheDocument();
  });
});
