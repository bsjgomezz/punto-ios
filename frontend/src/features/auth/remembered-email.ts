const KEY = "puntoios.email";

/** Email recordado en este dispositivo (nunca la contraseña). */
export function readRememberedEmail(): string {
  try {
    return localStorage.getItem(KEY) ?? "";
  } catch {
    return "";
  }
}

export function writeRememberedEmail(email: string | null): void {
  try {
    if (email) localStorage.setItem(KEY, email);
    else localStorage.removeItem(KEY);
  } catch {
    // Sin almacenamiento disponible (modo privado, etc.): simplemente no se recuerda.
  }
}
