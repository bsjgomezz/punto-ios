"use client";

import { useEffect, useState } from "react";
import { useAuth } from "@/features/auth/auth-provider";
import { ApiError } from "@/lib/api/client";
import { listarDispositivos, revelarImei, type Dispositivo } from "./api";
import { DispositivoForm } from "./dispositivo-form";

type Props = { clienteId: string; permisos: string[] };

type Lista =
  | { clave: string; kind: "listo"; items: Dispositivo[] }
  | { clave: string; kind: "error"; mensaje: string };

/** Equipos del cliente: alta, edición y consulta del IMEI completo (queda registrada). */
export function EquiposCliente({ clienteId, permisos }: Props) {
  const { user } = useAuth();
  const puedeCrear = permisos.includes("dispositivos.crear");
  const puedeEditar = permisos.includes("dispositivos.editar");
  const puedeVerImei = permisos.includes("dispositivos.ver_imei");

  const [recarga, setRecarga] = useState(0);
  const [lista, setLista] = useState<Lista | null>(null);
  const [agregando, setAgregando] = useState(false);
  const [editandoId, setEditandoId] = useState<string | null>(null);
  const [imeis, setImeis] = useState<Record<string, string | null>>({});
  const [errorImei, setErrorImei] = useState<string | null>(null);
  const [mensaje, setMensaje] = useState<string | null>(null);

  const clave = `${clienteId}|${recarga}`;

  useEffect(() => {
    if (!user) return;
    const controller = new AbortController();

    user
      .getIdToken()
      .then((token) => listarDispositivos(token, clienteId, controller.signal))
      .then((items) => setLista({ clave, kind: "listo", items }))
      .catch((error: unknown) => {
        if (controller.signal.aborted) return;
        setLista({
          clave,
          kind: "error",
          mensaje:
            error instanceof ApiError && error.status === 403
              ? "Tu rol no permite ver los equipos."
              : "No se pudieron cargar los equipos.",
        });
      });

    return () => controller.abort();
  }, [user, clienteId, clave]);

  function alGuardar(d: Dispositivo, accion: "agregado" | "actualizado") {
    setAgregando(false);
    setEditandoId(null);
    setImeis((actual) => {
      const copia = { ...actual };
      delete copia[d.id];
      return copia;
    });
    setMensaje(`Equipo ${accion}: ${d.marca} ${d.modelo}.`);
    setRecarga((n) => n + 1);
  }

  async function verImei(id: string) {
    if (!user) return;
    setErrorImei(null);
    try {
      const { imeiSerie } = await revelarImei(await user.getIdToken(), id);
      setImeis((actual) => ({ ...actual, [id]: imeiSerie }));
    } catch {
      setErrorImei("No se pudo obtener el IMEI. Intentá de nuevo.");
    }
  }

  function ocultarImei(id: string) {
    setImeis((actual) => {
      const copia = { ...actual };
      delete copia[id];
      return copia;
    });
  }

  const items = lista?.kind === "listo" ? lista.items : null;

  return (
    <section aria-label="Equipos del cliente" className="history">
      <div className="section-head">
        <h2 className="client-form-title">Equipos</h2>
        {puedeCrear && !agregando && (
          <button
            type="button"
            className="btn btn-quiet"
            onClick={() => {
              setMensaje(null);
              setEditandoId(null);
              setAgregando(true);
            }}
          >
            Agregar equipo
          </button>
        )}
      </div>

      {mensaje && (
        <p className="identity" data-state="ok" role="status">
          {mensaje}
        </p>
      )}
      {errorImei && (
        <p className="form-error" role="alert">
          {errorImei}
        </p>
      )}

      {agregando && (
        <DispositivoForm
          clienteId={clienteId}
          onGuardado={(d) => alGuardar(d, "agregado")}
          onCancelar={() => setAgregando(false)}
        />
      )}

      {lista?.kind === "error" && (
        <p className="form-error" role="alert">
          {lista.mensaje}
        </p>
      )}
      {!lista && <p className="list-count">Cargando equipos…</p>}
      {items && items.length === 0 && <p className="list-count">Todavía no tiene equipos registrados.</p>}

      {items && items.length > 0 && (
        <ul className="client-list">
          {items.map((d) =>
            editandoId === d.id ? (
              <li key={d.id} className="device-row-edit">
                <DispositivoForm
                  clienteId={clienteId}
                  inicial={d}
                  onGuardado={(g) => alGuardar(g, "actualizado")}
                  onCancelar={() => setEditandoId(null)}
                />
              </li>
            ) : (
              <li key={d.id} className="device-row">
                <div>
                  <p className="client-name">
                    {d.marca} {d.modelo}
                  </p>
                  <p className="client-meta">{[d.almacenamiento, d.color].filter(Boolean).join(" · ") || "Sin más datos"}</p>
                  {d.observaciones && <p className="client-meta">{d.observaciones}</p>}
                </div>
                <div className="device-imei">
                  {d.imeiSerieEnmascarado ? (
                    <>
                      <code aria-label={d.id in imeis ? "IMEI completo" : "IMEI enmascarado"}>
                        {d.id in imeis ? imeis[d.id] : d.imeiSerieEnmascarado}
                      </code>
                      {puedeVerImei &&
                        (d.id in imeis ? (
                          <button type="button" className="btn btn-quiet btn-small" onClick={() => ocultarImei(d.id)}>
                            Ocultar
                          </button>
                        ) : (
                          <button
                            type="button"
                            className="btn btn-quiet btn-small"
                            onClick={() => void verImei(d.id)}
                            aria-label={`Ver IMEI completo de ${d.marca} ${d.modelo}`}
                          >
                            Ver IMEI
                          </button>
                        ))}
                    </>
                  ) : (
                    <span className="client-meta">Sin IMEI</span>
                  )}
                  {puedeEditar && (
                    <button
                      type="button"
                      className="btn btn-quiet btn-small"
                      onClick={() => {
                        setMensaje(null);
                        setAgregando(false);
                        setEditandoId(d.id);
                      }}
                      aria-label={`Editar ${d.marca} ${d.modelo}`}
                    >
                      Editar
                    </button>
                  )}
                </div>
              </li>
            ),
          )}
        </ul>
      )}
    </section>
  );
}
