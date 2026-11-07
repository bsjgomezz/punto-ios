import { cleanup, fireEvent, render, screen } from "@testing-library/react";
import { afterEach, describe, expect, it } from "vitest";
import { PasswordInput } from "./password-input";

afterEach(cleanup);

describe("PasswordInput", () => {
  it("arranca oculta con el ojo cerrado y al tocar el botón la muestra con el ojo abierto", () => {
    const { container } = render(
      <>
        <label htmlFor="pw">Contraseña</label>
        <PasswordInput id="pw" name="password" autoComplete="current-password" />
      </>,
    );
    const campo = screen.getByLabelText("Contraseña");

    expect(campo).toHaveAttribute("type", "password");
    expect(container.querySelector('[data-icon="ojo-cerrado"]')).not.toBeNull();

    fireEvent.click(screen.getByRole("button", { name: "Mostrar contraseña" }));

    expect(campo).toHaveAttribute("type", "text");
    expect(container.querySelector('[data-icon="ojo-abierto"]')).not.toBeNull();
    expect(screen.getByRole("button", { name: "Ocultar contraseña" })).toHaveAttribute("aria-pressed", "true");

    fireEvent.click(screen.getByRole("button", { name: "Ocultar contraseña" }));
    expect(campo).toHaveAttribute("type", "password");
  });
});
