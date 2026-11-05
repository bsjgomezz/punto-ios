import { RequireAuth } from "@/features/auth/require-auth";
import { ServerIdentity } from "@/features/auth/server-identity";
import { SessionBar } from "@/features/auth/session-bar";

export default function PanelPage() {
  return (
    <RequireAuth>
      <main className="shell">
        <header className="panel-head">
          <p className="brand">
            <span className="brand-mark" aria-hidden="true" />
            Punto iOS
          </p>
          <SessionBar />
        </header>

        <h1 className="page-title">Panel</h1>
        <p className="hero-lead">
          Todavía no hay datos. Los clientes y las reparaciones se van a gestionar desde acá.
        </p>

        <ServerIdentity />
      </main>
    </RequireAuth>
  );
}
