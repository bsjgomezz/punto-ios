import { cleanup, render, screen, waitFor } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";

const mocks = vi.hoisted(() => ({
  replace: vi.fn(),
  auth: { status: "loading" as string },
}));

vi.mock("next/navigation", () => ({ useRouter: () => ({ replace: mocks.replace }) }));
vi.mock("./auth-provider", () => ({ useAuth: () => mocks.auth }));

import { RequireAuth } from "./require-auth";

afterEach(() => {
  cleanup();
  vi.clearAllMocks();
});

describe("RequireAuth", () => {
  it("mientras verifica la sesión no muestra el contenido ni redirige", () => {
    mocks.auth.status = "loading";
    render(<RequireAuth><p>Contenido privado</p></RequireAuth>);

    expect(screen.queryByText("Contenido privado")).not.toBeInTheDocument();
    expect(screen.getByRole("status")).toHaveTextContent("Verificando tu sesión");
    expect(mocks.replace).not.toHaveBeenCalled();
  });

  it("sin sesión redirige a /ingresar y no muestra el contenido", async () => {
    mocks.auth.status = "signedOut";
    render(<RequireAuth><p>Contenido privado</p></RequireAuth>);

    await waitFor(() => expect(mocks.replace).toHaveBeenCalledWith("/ingresar"));
    expect(screen.queryByText("Contenido privado")).not.toBeInTheDocument();
  });

  it("con sesión muestra el contenido", () => {
    mocks.auth.status = "signedIn";
    render(<RequireAuth><p>Contenido privado</p></RequireAuth>);

    expect(screen.getByText("Contenido privado")).toBeInTheDocument();
    expect(mocks.replace).not.toHaveBeenCalled();
  });
});
