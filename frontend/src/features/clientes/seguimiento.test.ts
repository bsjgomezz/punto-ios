import { describe, expect, it } from "vitest";
import { linkDeSeguimiento, mensajeDeSeguimiento, numeroWhatsapp, urlWhatsapp } from "./seguimiento";

describe("numeroWhatsapp", () => {
  it.each([
    ["3498 469259", "5493498469259"],
    ["(03498) 469259", "5493498469259"],
    ["+54 9 3498 46-9259", "5493498469259"],
    ["+54 3498 469259", "5493498469259"],
    ["0054 9 342 555 1234", "5493425551234"],
  ])("arma el número internacional desde %s", (telefono, esperado) => {
    expect(numeroWhatsapp(telefono)).toBe(esperado);
  });

  it.each(["03498 15 469259", "469259", "123"])("si no quedan 10 dígitos no adivina (%s)", (telefono) => {
    expect(numeroWhatsapp(telefono)).toBeNull();
  });
});

describe("urlWhatsapp", () => {
  it("con número abre el chat del cliente con el mensaje", () => {
    expect(urlWhatsapp("3498 469259", "Hola & chau")).toBe("https://wa.me/5493498469259?text=Hola%20%26%20chau");
  });

  it("sin número válido deja elegir el contacto", () => {
    expect(urlWhatsapp("469259", "Hola")).toBe("https://wa.me/?text=Hola");
  });
});

describe("link y mensaje", () => {
  it("arma el link con la dirección del portal y el código", () => {
    const link = linkDeSeguimiento("ABC123", "https://puntoios.com.ar");
    expect(link).toBe("https://puntoios.com.ar/seguimiento/ABC123");
    expect(mensajeDeSeguimiento("Ana", link)).toContain("Ana");
    expect(mensajeDeSeguimiento("Ana", link)).toContain(link);
  });
});
