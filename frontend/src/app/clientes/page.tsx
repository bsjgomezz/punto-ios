import { RequireAuth } from "@/features/auth/require-auth";
import { ClientesPage } from "@/features/clientes/clientes-page";

export default function ClientesRoute() {
  return (
    <RequireAuth>
      <ClientesPage />
    </RequireAuth>
  );
}
