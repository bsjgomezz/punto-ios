import { cleanup, fireEvent, render, screen, waitFor } from "@testing-library/react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

const mocks = vi.hoisted(() => ({
  replace: vi.fn(),
  signIn: vi.fn(),
  signInWithGoogle: vi.fn(),
  auth: { status: "signedOut" as string, configError: null as string | null },
}));

vi.mock("next/navigation", () => ({ useRouter: () => ({ replace: mocks.replace }) }));
vi.mock("./auth-provider", () => ({
  useAuth: () => ({
    ...mocks.auth,
    signIn: mocks.signIn,
    signInWithGoogle: mocks.signInWithGoogle,
  }),
}));

import { LoginForm } from "./login-form";

function completar(email: string, password: string) {
  fireEvent.change(screen.getByLabelText("Email"), { target: { value: email } });
  fireEvent.change(screen.getByLabelText("Contraseña"), { target: { value: password } });
}

beforeEach(() => {
  localStorage.clear();
  mocks.auth.status = "signedOut";
  mocks.auth.configError = null;
});

afterEach(() => {
  cleanup();
  vi.clearAllMocks();
});

describe("LoginForm", () => {
  it("pide completar ambos campos y no llama a Firebase", () => {
    render(<LoginForm />);

    fireEvent.click(screen.getByRole("button", { name: "Ingresar" }));

    expect(screen.getByRole("alert")).toHaveTextContent("Ingresá tu email y tu contraseña.");
    expect(mocks.signIn).not.toHaveBeenCalled();
  });

  it("ingresa y lleva al panel con credenciales válidas", async () => {
    mocks.signIn.mockResolvedValue(undefined);
    render(<LoginForm />);

    completar("  dueno@puntoios.com ", "secreta123");
    fireEvent.click(screen.getByRole("button", { name: "Ingresar" }));

    await waitFor(() => expect(mocks.replace).toHaveBeenCalledWith("/panel"));
    expect(mocks.signIn).toHaveBeenCalledWith("dueno@puntoios.com", "secreta123", true);
  });

  it("recuerda el email (nunca la contraseña) cuando el tick está marcado", async () => {
    mocks.signIn.mockResolvedValue(undefined);
    render(<LoginForm />);

    completar("dueno@puntoios.com", "secreta123");
    fireEvent.click(screen.getByRole("button", { name: "Ingresar" }));

    await waitFor(() => expect(mocks.replace).toHaveBeenCalledWith("/panel"));
    expect(localStorage.getItem("puntoios.email")).toBe("dueno@puntoios.com");
    expect(JSON.stringify({ ...localStorage })).not.toContain("secreta123");
  });

  it("si se desmarca el tick, no mantiene la sesión y borra el email recordado", async () => {
    localStorage.setItem("puntoios.email", "viejo@puntoios.com");
    mocks.signIn.mockResolvedValue(undefined);
    render(<LoginForm />);

    completar("dueno@puntoios.com", "secreta123");
    fireEvent.click(screen.getByLabelText("Recordarme en este dispositivo"));
    fireEvent.click(screen.getByRole("button", { name: "Ingresar" }));

    await waitFor(() => expect(mocks.replace).toHaveBeenCalledWith("/panel"));
    expect(mocks.signIn).toHaveBeenCalledWith("dueno@puntoios.com", "secreta123", false);
    expect(localStorage.getItem("puntoios.email")).toBeNull();
  });

  it("completa el email recordado al abrir el formulario", async () => {
    localStorage.setItem("puntoios.email", "dueno@puntoios.com");
    render(<LoginForm />);

    await waitFor(() => expect(screen.getByLabelText("Email")).toHaveValue("dueno@puntoios.com"));
  });

  it("muestra un mensaje claro si las credenciales son incorrectas", async () => {
    mocks.signIn.mockRejectedValue({ code: "auth/invalid-credential" });
    render(<LoginForm />);

    completar("dueno@puntoios.com", "mala");
    fireEvent.click(screen.getByRole("button", { name: "Ingresar" }));

    expect(await screen.findByRole("alert")).toHaveTextContent("Email o contraseña incorrectos.");
    expect(mocks.replace).not.toHaveBeenCalled();
    expect(screen.getByRole("button", { name: "Ingresar" })).toBeEnabled();
  });

  it("ingresa con Google y lleva al panel", async () => {
    mocks.signInWithGoogle.mockResolvedValue(undefined);
    render(<LoginForm />);

    fireEvent.click(screen.getByRole("button", { name: "Ingresar con Google" }));

    await waitFor(() => expect(mocks.replace).toHaveBeenCalledWith("/panel"));
    expect(mocks.signInWithGoogle).toHaveBeenCalledWith(true);
    expect(mocks.signIn).not.toHaveBeenCalled();
  });

  it("si se cierra la ventana de Google, explica qué pasó y permite reintentar", async () => {
    mocks.signInWithGoogle.mockRejectedValue({ code: "auth/popup-closed-by-user" });
    render(<LoginForm />);

    fireEvent.click(screen.getByRole("button", { name: "Ingresar con Google" }));

    expect(await screen.findByRole("alert")).toHaveTextContent("Se cerró la ventana de Google");
    expect(mocks.replace).not.toHaveBeenCalled();
    expect(screen.getByRole("button", { name: "Ingresar con Google" })).toBeEnabled();
  });

  it("muestra el problema de configuración de Firebase", () => {
    mocks.auth.configError = "Falta la configuración de Firebase.";
    render(<LoginForm />);

    expect(screen.getByRole("alert")).toHaveTextContent("Falta la configuración de Firebase.");
  });

  it("si ya hay sesión, redirige al panel", async () => {
    mocks.auth.status = "signedIn";
    render(<LoginForm />);

    await waitFor(() => expect(mocks.replace).toHaveBeenCalledWith("/panel"));
  });
});
