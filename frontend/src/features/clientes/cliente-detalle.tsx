"use client";

import { BackLink } from "@/components/back-link";
import { useEffect, useState } from "react";
import { useAuth } from "@/features/auth/auth-provider";
import { SessionBar } from "@/features/auth/session-bar";
import { useMe } from "@/features/auth/use-me";
import { ApiError } from "@/lib/api/client";
import {
  actualizarCliente,
  historialDeCliente,
  obtenerCliente,
  type Cliente,
  type RegistroHistorial,
} from "./api";
import { EquiposCliente } from "@/features/dispositivos/equipos-cliente";
import { OrdenesCliente } from "@/features/reparaciones/ordenes-cliente";
import { ClienteForm } from "./cliente-form";
import { SeguimientoCliente } from "./seguimiento-cliente";

type Datos =
  | { clave: string; kind: "listo"; cliente: Cliente; historial: RegistroHistorial[] }
  | { clave: string; kind: "noEncontrado" }
  | { clave: string; kind: "sinPermiso" }
  | { clave: string; kind: "error" };

const ETIQUETAS: Record<string, string> = {
  nombre: "Nombre",
  apellido: "Apellido",
  telefono: "Teléfono",
  email: "Email",
  direccion: "Dirección",
  observaciones: "Observaciones",
};

const ACCIONES: Record<string, string> = {
  creado: "Cliente creado",
  editado: "Datos editados",
  codigo_generado: "Código de seguimiento generado",
  codigo_regenerado: "Código de seguimiento regenerado",
  codigo_consultado: "Código de seguimiento consultado o enviado",
};

const formatoFecha = new Intl.DateTimeFormat("es-AR", { dateStyle: "medium", timeStyle: "short" });

function fecha(iso: string) {
  const f = new Date(iso);
  return Number.isNaN(f.getTime()) ? iso : formatoFecha.format(f);
}

export function ClienteDetalle({ id }: { id: string }) {
  const { user } = useAuth();
  const me = useMe();
  const permisos = me.kind === "listo" ? me.me.permisos : [];
  const puedeEditar = permisos.includes("clientes.editar");

  const [recarga, setRecarga] = useState(0);
  const [datos, setDatos] = useState<Datos | null>(null);
  const [editando, setEditando] = useState(false);
  const [guardado, setGuardado] = useState<string | null>(null);

  const clave = `${id}|${recarga}`;

  useEffect(() => {
    if (!user) return;
    const controller = new AbortController();

    user
      .getIdToken()
      .then((token) =>
        Promise.all([
          obtenerCliente(token, id, controller.signal),
          historialDeCliente(token, id, controller.signal),
        ]),
      )
      .then(([cliente, historial]) => setDatos({ clave, kind: "listo", cliente, historial: historial.items }))
      .catch((error: unknown) => {
        if (controller.signal.aborted) return;
        if (error instanceof ApiError && error.status === 404) setDatos({ clave, kind: "noEncontrado" });
        else if (error instanceof ApiError && error.status === 403) setDatos({ clave, kind: "sinPermiso" });
        else setDatos({ clave, kind: "error" });
      });

    return () => controller.abort();
  }, [user, id, clave]);

  const actual = datos?.clave === clave ? datos : null;
  const cliente = datos?.kind === "listo" ? datos.cliente : null;

  function alGuardar(actualizado: Cliente) {
    setEditando(false);
    setGuardado(`Cambios guardados: ${actualizado.apellido}, ${actualizado.nombre}.`);
    setRecarga((n) => n + 1);
  }

  return (
    <main className="shell shell-wide">
      <header className="panel-head">
        <p className="brand">
          <span className="brand-mark" aria-hidden="true" />
          Punto iOS
        </p>
        <SessionBar />
      </header>

      <BackLink href="/clientes">Volver a clientes</BackLink>

      {datos?.kind === "noEncontrado" && (
        <p className="form-error" role="alert">
          No se encontró el cliente.
        </p>
      )}
      {datos?.kind === "sinPermiso" && (
        <p className="form-error" role="alert">
          Tu rol no permite ver clientes.
        </p>
      )}
      {datos?.kind === "error" && (
        <p className="form-error" role="alert">
          No se pudo cargar el cliente. Verificá que el backend esté corriendo e intentá de nuevo.
        </p>
      )}
      {!datos && (
        <p className="gate-message" role="status">
          Cargando cliente…
        </p>
      )}

      {cliente && datos?.kind === "listo" && (
        <div aria-busy={!actual}>
          <h1 className="page-title">
            {cliente.apellido}, {cliente.nombre}
          </h1>

          {guardado && (
            <p className="identity" data-state="ok" role="status">
              {guardado}
            </p>
          )}

          {editando ? (
            <ClienteForm
              titulo="Editar cliente"
              etiqueta="Editar cliente"
              etiquetaGuardar="Guardar cambios"
              inicial={cliente}
              mensajeSinPermiso="Tu rol no permite editar clientes."
              guardar={(token, d) => actualizarCliente(token, id, d)}
              onGuardado={alGuardar}
              onCancelar={() => setEditando(false)}
            />
          ) : (
            <section aria-label="Datos del cliente">
              <dl className="detail-list">
                <div>
                  <dt>Teléfono</dt>
                  <dd>{cliente.telefono}</dd>
                </div>
                <div>
                  <dt>Email</dt>
                  <dd>{cliente.email ?? "—"}</dd>
                </div>
                <div>
                  <dt>Dirección</dt>
                  <dd>{cliente.direccion ?? "—"}</dd>
                </div>
                <div>
                  <dt>Observaciones</dt>
                  <dd>{cliente.observaciones ?? "—"}</dd>
                </div>
                <div>
                  <dt>Cliente desde</dt>
                  <dd>{fecha(cliente.creadoEn)}</dd>
                </div>
              </dl>
              {puedeEditar && (
                <button type="button" className="btn" onClick={() => { setGuardado(null); setEditando(true); }}>
                  Editar cliente
                </button>
              )}
            </section>
          )}

          {permisos.includes("reparaciones.ver") && (
            <OrdenesCliente cliente={cliente} puedeRecibir={permisos.includes("reparaciones.crear")} />
          )}

          {permisos.includes("dispositivos.ver") && <EquiposCliente clienteId={id} permisos={permisos} />}

          {permisos.includes("clientes.seguimiento") && (
            <SeguimientoCliente cliente={cliente} onCambio={() => setRecarga((n) => n + 1)} />
          )}

          <section aria-label="Historial de cambios" className="history">
            <h2 className="client-form-title">Historial de cambios</h2>
            {datos.historial.length === 0 ? (
              <p className="list-count">Sin movimientos registrados.</p>
            ) : (
              <ol className="history-list">
                {datos.historial.map((r) => (
                  <li key={r.id} className="history-item">
                    <p className="history-head">
                      <strong>{ACCIONES[r.accion] ?? r.accion}</strong>
                      <span className="client-meta">
                        {r.usuario ? ` por ${r.usuario}` : ""} · {fecha(r.fecha)}
                      </span>
                    </p>
                    {r.cambios.length > 0 && (
                      <ul className="history-changes">
                        {r.cambios.map((c) => (
                          <li key={c.campo}>
                            <span className="client-meta">{ETIQUETAS[c.campo] ?? c.campo}:</span>{" "}
                            <del>{c.anterior ?? "vacío"}</del> → <ins>{c.nuevo ?? "vacío"}</ins>
                          </li>
                        ))}
                      </ul>
                    )}
                  </li>
                ))}
              </ol>
            )}
          </section>
        </div>
      )}
    </main>
  );
}
