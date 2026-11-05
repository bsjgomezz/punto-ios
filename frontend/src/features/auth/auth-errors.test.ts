import { describe, expect, it } from "vitest";
import { describeAuthError } from "./auth-errors";

describe("describeAuthError", () => {
  it.each(["auth/invalid-credential", "auth/wrong-password", "auth/user-not-found"])(
    "no revela si falló el email o la contraseña (%s)",
    (code) => {
      expect(describeAuthError({ code })).toBe("Email o contraseña incorrectos.");
    },
  );

  it.each([
    ["auth/invalid-email", "El email no es válido."],
    ["auth/user-disabled", "Esta cuenta está deshabilitada. Pedile al administrador que la active."],
    ["auth/too-many-requests", "Demasiados intentos. Esperá unos minutos y probá de nuevo."],
    ["auth/popup-closed-by-user", "Se cerró la ventana de Google antes de terminar. Probá de nuevo."],
    ["auth/popup-blocked", "El navegador bloqueó la ventana de Google. Permití las ventanas emergentes para este sitio."],
    ["auth/account-exists-with-different-credential", "Ya existe una cuenta con ese email. Ingresá con email y contraseña."],
    ["auth/network-request-failed", "No hay conexión con el servicio de acceso. Revisá tu internet."],
  ])("traduce %s", (code, mensaje) => {
    expect(describeAuthError({ code })).toBe(mensaje);
  });

  it("usa un mensaje genérico ante errores desconocidos o que no son de Firebase", () => {
    const generico = "No se pudo iniciar sesión. Probá de nuevo.";
    expect(describeAuthError({ code: "auth/otra-cosa" })).toBe(generico);
    expect(describeAuthError(new Error("boom"))).toBe(generico);
    expect(describeAuthError(undefined)).toBe(generico);
  });
});
