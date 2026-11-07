"use client";

import { useEffect, useState } from "react";
import { apiGet } from "@/lib/api/client";
import { useAuth } from "./auth-provider";

export type UsuarioActual = {
  uid: string;
  email: string | null;
  emailVerificado: boolean;
  nombre: string | null;
  usuarioId: string | null;
  rol: string | null;
  permisos: string[];
};

export type EstadoMe =
  | { kind: "cargando" }
  | { kind: "listo"; me: UsuarioActual }
  | { kind: "error"; status: number | null };

/** Perfil del usuario en el servidor (rol y permisos). La UI lo usa solo para mostrar u ocultar acciones. */
export function useMe(): EstadoMe {
  const { user } = useAuth();
  const [estado, setEstado] = useState<EstadoMe>({ kind: "cargando" });

  useEffect(() => {
    if (!user) return;
    const controller = new AbortController();

    user
      .getIdToken()
      .then((token) => apiGet<UsuarioActual>("/api/v1/me", token, controller.signal))
      .then((me) => setEstado({ kind: "listo", me }))
      .catch((error: unknown) => {
        if (controller.signal.aborted) return;
        const status = typeof error === "object" && error !== null && "status" in error ? Number((error as { status: unknown }).status) : null;
        setEstado({ kind: "error", status });
      });

    return () => controller.abort();
  }, [user]);

  return estado;
}
