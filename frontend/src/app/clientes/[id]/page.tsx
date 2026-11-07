import { Suspense } from "react";
import { RequireAuth } from "@/features/auth/require-auth";
import { ClienteDetalle } from "@/features/clientes/cliente-detalle";

async function Detalle({ params }: PageProps<"/clientes/[id]">) {
  const { id } = await params;
  return (
    <RequireAuth>
      <ClienteDetalle id={id} />
    </RequireAuth>
  );
}

export default function ClienteRoute(props: PageProps<"/clientes/[id]">) {
  return (
    <Suspense fallback={null}>
      <Detalle {...props} />
    </Suspense>
  );
}
