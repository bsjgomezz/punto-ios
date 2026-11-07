"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { BackLink } from "@/components/back-link";
import { useAuth } from "@/features/auth/auth-provider";
import { SessionBar } from "@/features/auth/session-bar";
import { useMe } from "@/features/auth/use-me";
import { ApiError, apiGet } from "@/lib/api/client";
import {
  AVISO_FECHA_ESTIMADA,
  NOMBRES_DESBLOQUEO,
  formatearFecha,
  nombreDeEstado,
  nombreDeTipo,
  obtenerOrden,
  verDesbloqueo,
  type Orden,
  type TipoDesbloqueo,
} from "./api";
import { PatronDesbloqueo } from "./patron-desbloqueo";

type Registro = { id: string; accion: string; usuario: string | null; fecha: string };

type Estado =
  | { kind: "listo"; orden: Orden; historial: Registro[] }
  | { kind: "noEncontrada" }
  | { kind: "sinPermiso" }
  | { kind: "error" };

const ACCIONES: Record<string, string> = {
  creado: "Equipo recibido",
  desbloqueo_consultado: "Se vio el código de desbloqueo",
};

const formatoFechaHora = new Intl.DateTimeFormat("es-AR", { dateStyle: "medium", timeStyle: "short" });

export function OrdenDetalle({ id }: { id: string }) {
  const { user } = useAuth();
  const me = useMe();
  const puedeVerDesbloqueo = me.kind === "listo" && me.me.permisos.includes("reparaciones.ver_desbloqueo");

  const [estado, setEstado] = useState<Estado | null>(null);
  const [recarga, setRecarga] = useState(0);
  const [desbloqueo, setDesbloqueo] = useState<{ tipo: TipoDesbloqueo; valor: string } | null>(null);
  const [errorDesbloqueo, setErrorDesbloqueo] = useState<string | null>(null);

  useEffect(() => {
    if (!user) return;
    const controller = new AbortController();
    user
      .getIdToken()
      .then((token) =>
        Promise.all([
          obtenerOrden(token, id, controller.signal),
          apiGet<{ items: Registro[] }>(`/api/v1/reparaciones/${encodeURIComponent(id)}/historial?pageSize=50`, token, controller.signal),
        ]),
      )
      .then(([orden, historial]) => setEstado({ kind: "listo", orden, historial: historial.items }))
      .catch((error: unknown) => {
        if (controller.signal.aborted) return;
        if (error instanceof ApiError && error.status === 404) setEstado({ kind: "noEncontrada" });
        else if (error instanceof ApiError && error.status === 403) setEstado({ kind: "sinPermiso" });
        else setEstado({ kind: "error" });
      });
    return () => controller.abort();
  }, [user, id, recarga]);

  async function mostrarDesbloqueo() {
    if (!user) return;
    setErrorDesbloqueo(null);
    try {
      setDesbloqueo(await verDesbloqueo(await user.getIdToken(), id));
      setRecarga((n) => n + 1);
    } catch {
      setErrorDesbloqueo("No se pudo obtener el código de desbloqueo.");
    }
  }

  const orden = estado?.kind === "listo" ? estado.orden : null;

  return (
    <main className="shell shell-wide">
      <header className="panel-head">
        <p className="brand">
          <span className="brand-mark" aria-hidden="true" />
          Punto iOS
        </p>
        <SessionBar />
      </header>

      <BackLink href="/reparaciones">Volver a reparaciones</BackLink>

      {!estado && (
        <p className="gate-message" role="status">
          Cargando orden…
        </p>
      )}
      {estado?.kind === "noEncontrada" && (
        <p className="form-error" role="alert">
          No se encontró la orden.
        </p>
      )}
      {estado?.kind === "sinPermiso" && (
        <p className="form-error" role="alert">
          Tu rol no permite ver reparaciones.
        </p>
      )}
      {estado?.kind === "error" && (
        <p className="form-error" role="alert">
          No se pudo cargar la orden. Verificá que el backend esté corriendo e intentá de nuevo.
        </p>
      )}

      {orden && estado?.kind === "listo" && (
        <>
          <div className="orden-cabecera">
            <h1 className="orden-numero">Orden {orden.numero}</h1>
            <span className="estado-chip" data-estado={orden.estado}>
              {nombreDeEstado(orden.estado)}
            </span>
          </div>
          <p className="orden-sub">
            <Link href={`/clientes/${orden.cliente.id}`}>
              {orden.cliente.apellido}, {orden.cliente.nombre}
            </Link>{" "}
            <span className="client-meta">{orden.cliente.telefono}</span>
          </p>

          <dl className="detail-list">
            <div>
              <dt>Equipo</dt>
              <dd>
                {orden.equipo.marca} {orden.equipo.modelo}
                <span className="client-meta">
                  {" "}
                  {[orden.equipo.almacenamiento, orden.equipo.color, orden.equipo.imeiSerieEnmascarado].filter(Boolean).join(" · ")}
                </span>
              </dd>
            </div>
            <div>
              <dt>Problema que cuenta el cliente</dt>
              <dd className="texto-largo">{orden.problemaInformado}</dd>
            </div>
            <div>
              <dt>Tipo de reparación</dt>
              <dd>
                {orden.tiposReparacion
                  .map((t) => (t === "otro" && orden.otroTipoDescripcion ? `Otro: ${orden.otroTipoDescripcion}` : nombreDeTipo(t)))
                  .join(", ")}
              </dd>
            </div>
            <div>
              <dt>Técnico</dt>
              <dd>{orden.tecnico?.nombre ?? "Sin asignar"}</dd>
            </div>
            <div>
              <dt>Entrega estimada</dt>
              <dd>
                {orden.fechaEstimadaEntrega ? (
                  <>
                    {formatearFecha(orden.fechaEstimadaEntrega)}
                    <span className="client-meta aviso-fecha">{AVISO_FECHA_ESTIMADA}</span>
                  </>
                ) : (
                  "Sin fecha estimada"
                )}
              </dd>
            </div>
            <div>
              <dt>Desbloqueo</dt>
              <dd>
                {!orden.desbloqueoTipo ? (
                  "No lo dejó"
                ) : desbloqueo ? (
                  desbloqueo.tipo === "patron" ? (
                    <PatronDesbloqueo puntos={desbloqueo.valor.split("-").map(Number)} etiqueta="Patrón guardado" />
                  ) : (
                    <code className="desbloqueo-valor" aria-label="Código de desbloqueo">
                      {desbloqueo.valor}
                    </code>
                  )
                ) : (
                  <span className="desbloqueo-oculto">
                    {NOMBRES_DESBLOQUEO[orden.desbloqueoTipo]} guardado
                    {puedeVerDesbloqueo && (
                      <button type="button" className="btn btn-quiet btn-small" onClick={() => void mostrarDesbloqueo()}>
                        Ver {NOMBRES_DESBLOQUEO[orden.desbloqueoTipo].toLowerCase()}
                      </button>
                    )}
                  </span>
                )}
                {errorDesbloqueo && (
                  <span className="form-error" role="alert">
                    {errorDesbloqueo}
                  </span>
                )}
              </dd>
            </div>
            <div>
              <dt>Ingreso</dt>
              <dd>
                {formatoFechaHora.format(new Date(orden.creadoEn))}
                {orden.creadoPor ? ` por ${orden.creadoPor}` : ""}
              </dd>
            </div>
          </dl>

          <section aria-label="Historial de la orden" className="history">
            <h2 className="client-form-title">Historial</h2>
            <ol className="history-list">
              {estado.historial.map((r) => (
                <li key={r.id} className="history-item">
                  <p className="history-head">
                    <strong>{ACCIONES[r.accion] ?? r.accion}</strong>
                    <span className="client-meta">
                      {r.usuario ? ` por ${r.usuario}` : ""} · {formatoFechaHora.format(new Date(r.fecha))}
                    </span>
                  </p>
                </li>
              ))}
            </ol>
          </section>
        </>
      )}
    </main>
  );
}
