import { cleanup, fireEvent, render, screen, waitFor } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import { StatusBoard } from "./status-board";

function mockApi(statusByPath: Record<string, number>) {
  vi.stubGlobal(
    "fetch",
    vi.fn(async (url: string) => {
      const path = new URL(url).pathname;
      return new Response("", { status: statusByPath[path] ?? 404 });
    }),
  );
}

afterEach(() => {
  cleanup();
  vi.unstubAllGlobals();
});

describe("StatusBoard", () => {
  it("muestra todo operativo cuando el servidor y la base responden", async () => {
    mockApi({ "/health": 200, "/health/ready": 200 });

    render(<StatusBoard />);

    await waitFor(() => expect(screen.getByText("Responde")).toBeInTheDocument());
    expect(screen.getByText("Conectada")).toBeInTheDocument();
    expect(screen.getByText("En línea")).toBeInTheDocument();
    expect(screen.queryByText(/docker compose up -d/)).not.toBeInTheDocument();
  });

  it("explica cómo resolverlo cuando la base de datos falla", async () => {
    mockApi({ "/health": 200, "/health/ready": 503 });

    render(<StatusBoard />);

    await waitFor(() => expect(screen.getByText("No disponible")).toBeInTheDocument());
    expect(screen.getByText(/docker compose up -d/)).toBeInTheDocument();
    expect(screen.getByText("Responde")).toBeInTheDocument();
  });

  it("vuelve a comprobar al apretar el botón", async () => {
    mockApi({ "/health": 503, "/health/ready": 503 });
    render(<StatusBoard />);
    await waitFor(() => expect(screen.getByText("No responde")).toBeInTheDocument());

    mockApi({ "/health": 200, "/health/ready": 200 });
    fireEvent.click(screen.getByRole("button", { name: "Comprobar de nuevo" }));

    await waitFor(() => expect(screen.getByText("Responde")).toBeInTheDocument());
    expect(screen.getByText("Conectada")).toBeInTheDocument();
  });
});
