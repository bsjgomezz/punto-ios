"use client";

import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { useAuth } from "@/features/auth/auth-provider";
import { listarOrdenes, type OrdenResumen } from "./api";
import { ListaOrdenes } from "./lista-ordenes";
import { NuevaOrdenForm } from "./nueva-orden-form";

type Props = { cliente: { id: string; nombre: string; apellido: string }; puedeRecibir: boolean };

/** Reparaciones del cliente en su ficha, con el botón para recibir un equipo sin salir de ahí. */
export function OrdenesCliente({ cliente, puedeRecibir }: Props) {
  const { user } = useAuth();
  const router = useRouter();
  const [ordenes, setOrdenes] = useState<OrdenResumen[] | null>(null);
  const [recibiendo, setRecibiendo] = useState(false);

  useEffect(() => {
    if (!user) return;
    const controller = new AbortController();
    user
      .getIdToken()
      .then((token) => listarOrdenes(token, { clienteId: cliente.id }, controller.signal))
      .then((pagina) => setOrdenes(pagina.items))
      .catch(() => {
        if (!controller.signal.aborted) setOrdenes([]);
      });
    return () => controller.abort();
  }, [user, cliente.id]);

  return (
    <section aria-label="Reparaciones del cliente" className="history">
      <div className="section-head">
        <h2 className="client-form-title">Reparaciones</h2>
        {puedeRecibir && !recibiendo && (
          <button type="button" className="btn" onClick={() => setRecibiendo(true)}>
            Recibir equipo
          </button>
        )}
      </div>

      {recibiendo && (
        <NuevaOrdenForm
          cliente={cliente}
          onCreada={(orden) => router.push(`/reparaciones/${orden.id}`)}
          onCancelar={() => setRecibiendo(false)}
        />
      )}

      {ordenes === null && <p className="list-count">Cargando reparaciones…</p>}
      {ordenes?.length === 0 && <p className="list-count">Todavía no dejó ningún equipo a reparar.</p>}
      {ordenes && ordenes.length > 0 && <ListaOrdenes ordenes={ordenes} mostrarCliente={false} />}
    </section>
  );
}
