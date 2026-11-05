"use client";

import { useEffect, type ReactNode } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "./auth-provider";

/**
 * Protege la interfaz: sin sesión redirige a /ingresar.
 * Es solo una comodidad de UI; la seguridad real la aplica el backend validando el token.
 */
export function RequireAuth({ children }: { children: ReactNode }) {
  const { status } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (status === "signedOut") router.replace("/ingresar");
  }, [status, router]);

  if (status !== "signedIn") {
    return (
      <p className="gate-message" role="status">
        Verificando tu sesión…
      </p>
    );
  }

  return <>{children}</>;
}
