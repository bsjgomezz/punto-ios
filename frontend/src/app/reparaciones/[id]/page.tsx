import { Suspense } from "react";
import { RequireAuth } from "@/features/auth/require-auth";
import { OrdenDetalle } from "@/features/reparaciones/orden-detalle";

async function Detalle({ params }: PageProps<"/reparaciones/[id]">) {
  const { id } = await params;
  return (
    <RequireAuth>
      <OrdenDetalle id={id} />
    </RequireAuth>
  );
}

export default function OrdenRoute(props: PageProps<"/reparaciones/[id]">) {
  return (
    <Suspense fallback={null}>
      <Detalle {...props} />
    </Suspense>
  );
}
