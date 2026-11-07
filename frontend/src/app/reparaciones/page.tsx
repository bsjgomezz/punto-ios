import { RequireAuth } from "@/features/auth/require-auth";
import { ReparacionesPage } from "@/features/reparaciones/reparaciones-page";

export default function ReparacionesRoute() {
  return (
    <RequireAuth>
      <ReparacionesPage />
    </RequireAuth>
  );
}
