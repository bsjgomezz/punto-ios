"use client";

import { useRouter } from "next/navigation";
import { useAuth } from "./auth-provider";

export function SessionBar() {
  const { user, signOut } = useAuth();
  const router = useRouter();

  async function onSignOut() {
    await signOut();
    router.replace("/ingresar");
  }

  return (
    <div className="session-bar">
      <span className="session-email">{user?.email}</span>
      <button type="button" className="btn btn-quiet" onClick={onSignOut}>
        Cerrar sesión
      </button>
    </div>
  );
}
