"use client";

import { useEffect, useState } from "react";
import { ApiError, apiGet } from "@/lib/api/client";
import { useAuth } from "./auth-provider";

type UsuarioActual = {
  uid: string;
  email: string | null;
  emailVerificado: boolean;
  nombre: string | null;
  usuarioId: string | null;
  rol: string | null;
  permisos: string[];
};

type State =
  | { kind: "checking" }
  | { kind: "ok"; email: string | null; rol: string | null }
  | { kind: "rejected" }
  | { kind: "forbidden" }
  | { kind: "unreachable" };

/** Confirma que el servidor reconoce la sesión (valida el token de Firebase). */
export function ServerIdentity() {
  const { user } = useAuth();
  const [state, setState] = useState<State>({ kind: "checking" });

  useEffect(() => {
    if (!user) return;
    const controller = new AbortController();

    user
      .getIdToken()
      .then((token) => apiGet<UsuarioActual>("/api/v1/me", token, controller.signal))
      .then((me) => setState({ kind: "ok", email: me.email, rol: me.rol }))
      .catch((error: unknown) => {
        if (controller.signal.aborted) return;
        if (error instanceof ApiError && error.status === 401) {
          setState({ kind: "rejected" });
        } else if (error instanceof ApiError && error.status === 403) {
          setState({ kind: "forbidden" });
        } else {
          setState({ kind: "unreachable" });
        }
      });

    return () => controller.abort();
  }, [user]);

  const text =
    state.kind === "ok"
      ? `El servidor reconoce tu sesión${state.email ? ` (${state.email})` : ""}${state.rol ? ` como ${state.rol}` : ""}.`
      : state.kind === "forbidden"
        ? "Tu cuenta todavía no está habilitada en Punto iOS. Pedile al administrador que te registre con este email."
        : state.kind === "rejected"
        ? "El servidor no reconoce tu sesión. Cerrá sesión y volvé a ingresar."
        : state.kind === "unreachable"
          ? "No se pudo consultar al servidor. Verificá que el backend esté corriendo."
          : "Verificando con el servidor…";

  return (
    <p className="identity" data-state={state.kind} role="status">
      {text}
    </p>
  );
}
