import { cleanup, fireEvent, render, screen } from "@testing-library/react";
import { useState } from "react";
import { afterEach, describe, expect, it } from "vitest";
import { PatronDesbloqueo } from "./patron-desbloqueo";

afterEach(cleanup);

function Editor() {
  const [puntos, setPuntos] = useState<number[]>([]);
  return (
    <>
      <PatronDesbloqueo puntos={puntos} onChange={setPuntos} etiqueta="Patrón" />
      <output>{puntos.join("-")}</output>
    </>
  );
}

describe("PatronDesbloqueo", () => {
  it("arma el patrón en el orden en que se tocan los puntos y no repite puntos", () => {
    render(<Editor />);

    for (const p of [1, 5, 9, 6]) fireEvent.click(screen.getByRole("button", { name: `Punto ${p}` }));

    expect(screen.getByRole("status")).toHaveTextContent("1-5-9-6");
    expect(screen.getByRole("button", { name: "Punto 5, 2.º" })).toBeDisabled();
    expect(screen.getByRole("button", { name: "Punto 5, 2.º" })).toHaveAttribute("aria-pressed", "true");
  });

  it("sin onChange es solo para mirar", () => {
    render(<PatronDesbloqueo puntos={[3, 5, 7]} etiqueta="Patrón guardado" />);

    expect(screen.getByRole("group", { name: "Patrón guardado" })).toBeInTheDocument();
    expect(screen.queryAllByRole("button")).toHaveLength(0);
  });
});
