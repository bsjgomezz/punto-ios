/** Traduce un error de Firebase Auth a un mensaje claro para el usuario. */
export function describeAuthError(error: unknown): string {
  const code =
    typeof error === "object" && error !== null && "code" in error
      ? String((error as { code: unknown }).code)
      : "";

  switch (code) {
    case "auth/invalid-credential":
    case "auth/wrong-password":
    case "auth/user-not-found":
      return "Email o contraseña incorrectos.";
    case "auth/invalid-email":
      return "El email no es válido.";
    case "auth/user-disabled":
      return "Esta cuenta está deshabilitada. Pedile al administrador que la active.";
    case "auth/too-many-requests":
      return "Demasiados intentos. Esperá unos minutos y probá de nuevo.";
    case "auth/popup-closed-by-user":
    case "auth/cancelled-popup-request":
      return "Se cerró la ventana de Google antes de terminar. Probá de nuevo.";
    case "auth/popup-blocked":
      return "El navegador bloqueó la ventana de Google. Permití las ventanas emergentes para este sitio.";
    case "auth/account-exists-with-different-credential":
      return "Ya existe una cuenta con ese email. Ingresá con email y contraseña.";
    case "auth/network-request-failed":
      return "No hay conexión con el servicio de acceso. Revisá tu internet.";
    default:
      return "No se pudo iniciar sesión. Probá de nuevo.";
  }
}
