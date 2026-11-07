import Link from "next/link";
import type { Coincidencia } from "@/features/clientes/api";

/** Aviso de IMEI repetido: indica a qué cliente pertenece. No se puede guardar igual. */
export function AvisoImeiDuplicado({ duenio }: { duenio: Coincidencia | null }) {
  return (
    <div className="dup-warning" role="alert">
      <p>
        Ese IMEI o número de serie ya está registrado
        {duenio ? (
          <>
            {" "}en un equipo de{" "}
            <Link href={`/clientes/${duenio.id}`}>
              {duenio.apellido}, {duenio.nombre}
            </Link>
          </>
        ) : null}
        . Revisá el número: no se puede guardar repetido.
      </p>
    </div>
  );
}
