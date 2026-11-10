"use client";

import Link from "next/link";
import { useEffect, useRef, useState } from "react";
import { BackLink } from "@/components/back-link";
import { useAuth } from "@/features/auth/auth-provider";
import { SessionBar } from "@/features/auth/session-bar";
import { useMe } from "@/features/auth/use-me";
import type { CambioDeCampo } from "@/features/clientes/api";
import { ApiError, apiGet } from "@/lib/api/client";
import {
  AVISO_FECHA_ESTIMADA,
  CHECKLIST_CATALOGO,
  NOMBRES_DESBLOQUEO,
  RAZON_SIN_FECHA,
  TIPOS_DE_ITEM_PRESUPUESTO,
  actualizarDatosTecnicos,
  cambiarEstadoDeOrden,
  crearVersionPresupuesto,
  formatearFecha,
  formatearMonto,
  listarFotos,
  listarVersionesPresupuesto,
  nombreDeTipoItemPresupuesto,
  nombreDeEstado,
  nombreDeTipo,
  obtenerFotoUrl,
  obtenerOrden,
  subirFoto,
  verDesbloqueo,
  type DatosTecnicos,
  type Foto,
  type ItemPresupuesto,
  type Orden,
  type TipoDesbloqueo,
  type VersionDePresupuesto,
} from "./api";
import { PatronDesbloqueo } from "./patron-desbloqueo";

type Registro = { id: string; accion: string; usuario: string | null; fecha: string; cambios?: CambioDeCampo[] };

type Estado =
  | { kind: "listo"; orden: Orden; historial: Registro[] }
  | { kind: "noEncontrada" }
  | { kind: "sinPermiso" }
  | { kind: "error" };

const ACCIONES: Record<string, string> = {
  creado: "Equipo recibido",
  desbloqueo_consultado: "Se vio el código de desbloqueo",
  estado_cambiado: "Cambió el estado",
  editado: "Editó datos de la orden",
  presupuesto_creado: "Presupuesto creado",
};

/** Nombre para mostrar de cada campo auditado de la orden (snake_case en el backend). */
const NOMBRES_DE_CAMPO: Record<string, string> = {
  pruebas: "Pruebas",
  diagnostico: "Diagnóstico",
  mediciones: "Mediciones",
  componentes_afectados: "Componentes afectados",
  notas_internas: "Notas internas",
  version_presupuesto: "Versión del presupuesto",
  total_presupuesto: "Total del presupuesto",
};

function historialDeOrden(token: string, id: string, signal?: AbortSignal) {
  return apiGet<{ items: Registro[] }>(`/api/v1/reparaciones/${encodeURIComponent(id)}/historial?pageSize=50`, token, signal);
}

function mensajeDeErrorDeEstado(error: unknown): string {
  if (error instanceof ApiError && error.status === 403) return "Tu rol no permite cambiar el estado de la orden.";
  if (error instanceof ApiError && error.problem?.title) return error.problem.title;
  return "No se pudo cambiar el estado. Verificá la conexión e intentá de nuevo.";
}

function mensajeDeErrorDeDatosTecnicos(error: unknown): string {
  if (error instanceof ApiError && error.status === 403) return "Tu rol no permite editar los datos técnicos de la orden.";
  if (error instanceof ApiError && error.problem?.title) return error.problem.title;
  return "No se pudieron guardar los datos técnicos. Verificá la conexión e intentá de nuevo.";
}

function mensajeDeErrorDePresupuesto(error: unknown): string {
  if (error instanceof ApiError && error.status === 403) return "Tu rol no permite crear presupuestos de la orden.";
  if (error instanceof ApiError && error.problem?.title) return error.problem.title;
  return "No se pudo guardar el presupuesto. Verificá la conexión e intentá de nuevo.";
}

/** Una fila del editor de ítems del presupuesto: el monto va como string, tal cual está en el input. */
type FilaPresupuesto = { clave: number; tipo: string; descripcion: string; monto: string };

const filaVacia = (clave: number): FilaPresupuesto => ({ clave, tipo: TIPOS_DE_ITEM_PRESUPUESTO[0].codigo, descripcion: "", monto: "" });

const montoDeFila = (fila: FilaPresupuesto) => {
  const monto = Number(fila.monto);
  return fila.monto.trim() !== "" && Number.isFinite(monto) ? monto : 0;
};

const filaCompleta = (fila: FilaPresupuesto) => fila.descripcion.trim() !== "" && montoDeFila(fila) > 0;

/** Las filas que quedaron del todo vacías no se mandan; el resto va tal cual y lo valida el backend. */
function itemsDeFilas(filas: FilaPresupuesto[]): ItemPresupuesto[] {
  return filas
    .filter((f) => f.descripcion.trim() !== "" || f.monto.trim() !== "")
    .map((f) => ({ tipo: f.tipo, descripcion: f.descripcion.trim(), monto: montoDeFila(f) }));
}

/** Estado del formulario de datos técnicos: los textos van como string ("" si no hay nada). */
type FormDatosTecnicos = {
  pruebas: string;
  diagnostico: string;
  mediciones: string;
  componentesAfectados: string[];
  notasInternas: string;
};

function formDeOrden(orden: Orden): FormDatosTecnicos {
  return {
    pruebas: orden.pruebas ?? "",
    diagnostico: orden.diagnostico ?? "",
    mediciones: orden.mediciones ?? "",
    componentesAfectados: orden.componentesAfectados ?? [],
    notasInternas: orden.notasInternas ?? "",
  };
}

const textoONull = (texto: string) => (texto.trim() === "" ? null : texto);

/** El backend reemplaza los cinco campos juntos: se mandan siempre todos, tal cual están en pantalla. */
function datosDelForm(form: FormDatosTecnicos): DatosTecnicos {
  return {
    pruebas: textoONull(form.pruebas),
    diagnostico: textoONull(form.diagnostico),
    mediciones: textoONull(form.mediciones),
    componentesAfectados: form.componentesAfectados,
    notasInternas: textoONull(form.notasInternas),
  };
}

const formatoFechaHora = new Intl.DateTimeFormat("es-AR", { dateStyle: "medium", timeStyle: "short" });

function FotoMiniatura({ token, ordenId, foto }: { token: string; ordenId: string; foto: Foto }) {
  const [url, setUrl] = useState<string | null>(null);

  useEffect(() => {
    let objectUrl: string | null = null;
    const controller = new AbortController();
    obtenerFotoUrl(token, ordenId, foto.id, controller.signal)
      .then((u) => {
        objectUrl = u;
        setUrl(u);
      })
      .catch(() => {});
    return () => {
      controller.abort();
      if (objectUrl) URL.revokeObjectURL(objectUrl);
    };
  }, [token, ordenId, foto.id]);

  if (!url) return <div className="foto-miniatura" aria-busy="true" />;
  return (
    <a href={url} target="_blank" rel="noreferrer">
      {/* eslint-disable-next-line @next/next/no-img-element -- es una blob: URL autenticada, next/image no aplica */}
      <img src={url} alt="Foto del equipo" className="foto-miniatura" />
    </a>
  );
}

function TextoDeRegistro({ registro }: { registro: Registro }) {
  const cambio = registro.cambios?.find((c) => c.campo === "estado");
  if (registro.accion === "estado_cambiado" && cambio) {
    return (
      <span>
        Cambió el estado de <strong>{cambio.anterior ? nombreDeEstado(cambio.anterior) : "sin estado"}</strong> a{" "}
        <strong>{cambio.nuevo ? nombreDeEstado(cambio.nuevo) : "sin estado"}</strong>
      </span>
    );
  }
  if (registro.accion === "presupuesto_creado" && registro.cambios && registro.cambios.length > 0) {
    const version = registro.cambios.find((c) => c.campo === "version_presupuesto")?.nuevo;
    const total = registro.cambios.find((c) => c.campo === "total_presupuesto")?.nuevo;
    return (
      <span>
        Creó la versión <strong>{version ?? "?"}</strong> del presupuesto
        {total && (
          <>
            {" "}
            por <strong>{formatearMonto(Number(total))}</strong>
          </>
        )}
      </span>
    );
  }
  if (registro.accion === "editado" && registro.cambios && registro.cambios.length > 0) {
    const campos = registro.cambios.map((c) => NOMBRES_DE_CAMPO[c.campo] ?? c.campo).join(", ");
    return (
      <span>
        Editó: <strong>{campos}</strong>
      </span>
    );
  }
  return <strong>{ACCIONES[registro.accion] ?? registro.accion}</strong>;
}

export function OrdenDetalle({ id }: { id: string }) {
  const { user } = useAuth();
  const me = useMe();
  const puedeVerDesbloqueo = me.kind === "listo" && me.me.permisos.includes("reparaciones.ver_desbloqueo");
  const puedeCambiarEstado = me.kind === "listo" && me.me.permisos.includes("reparaciones.cambiar_estado");
  const puedeEditarDatosTecnicos = me.kind === "listo" && me.me.permisos.includes("reparaciones.reparar");
  const puedePresupuestar = me.kind === "listo" && me.me.permisos.includes("presupuestos.gestionar");

  const [estado, setEstado] = useState<Estado | null>(null);
  const [recarga, setRecarga] = useState(0);
  const [desbloqueo, setDesbloqueo] = useState<{ tipo: TipoDesbloqueo; valor: string } | null>(null);
  const [errorDesbloqueo, setErrorDesbloqueo] = useState<string | null>(null);
  const [fotos, setFotos] = useState<Foto[]>([]);
  const [subiendoFoto, setSubiendoFoto] = useState(false);
  const [errorFoto, setErrorFoto] = useState<string | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [estadoElegido, setEstadoElegido] = useState("");
  const [cambiandoEstado, setCambiandoEstado] = useState(false);
  const [errorEstado, setErrorEstado] = useState<string | null>(null);
  const [datosTecnicos, setDatosTecnicos] = useState<FormDatosTecnicos | null>(null);
  const [nuevoComponente, setNuevoComponente] = useState("");
  const [guardandoDatosTecnicos, setGuardandoDatosTecnicos] = useState(false);
  const [errorDatosTecnicos, setErrorDatosTecnicos] = useState<string | null>(null);
  const siguienteClaveDeFila = useRef(1);
  const [filasPresupuesto, setFilasPresupuesto] = useState<FilaPresupuesto[]>(() => [filaVacia(0)]);
  const [guardandoPresupuesto, setGuardandoPresupuesto] = useState(false);
  const [errorPresupuesto, setErrorPresupuesto] = useState<string | null>(null);
  // Las versiones anteriores se piden solo cuando se despliegan (la vigente ya viene con la orden).
  const [versiones, setVersiones] = useState<VersionDePresupuesto[] | null>(null);
  const [mostrandoVersiones, setMostrandoVersiones] = useState(false);
  const [cargandoVersiones, setCargandoVersiones] = useState(false);
  const [errorVersiones, setErrorVersiones] = useState<string | null>(null);

  useEffect(() => {
    if (!user) return;
    const controller = new AbortController();
    user
      .getIdToken()
      .then((t) => {
        setToken(t);
        return Promise.all([
          obtenerOrden(t, id, controller.signal),
          historialDeOrden(t, id, controller.signal),
          listarFotos(t, id, controller.signal).catch(() => [] as Foto[]),
        ]);
      })
      .then(([orden, historial, listaFotos]) => {
        setEstado({ kind: "listo", orden, historial: historial.items });
        setDatosTecnicos(formDeOrden(orden));
        setFotos(listaFotos);
      })
      .catch((error: unknown) => {
        if (controller.signal.aborted) return;
        if (error instanceof ApiError && error.status === 404) setEstado({ kind: "noEncontrada" });
        else if (error instanceof ApiError && error.status === 403) setEstado({ kind: "sinPermiso" });
        else setEstado({ kind: "error" });
      });
    return () => controller.abort();
  }, [user, id, recarga]);

  async function subirFotos(archivos: FileList | null) {
    if (!archivos || archivos.length === 0 || !user) return;
    setErrorFoto(null);
    setSubiendoFoto(true);
    try {
      const t = token ?? (await user.getIdToken());
      for (const archivo of Array.from(archivos)) {
        const foto = await subirFoto(t, id, archivo);
        setFotos((actuales) => [...actuales, foto]);
      }
    } catch {
      setErrorFoto("No se pudo subir alguna foto. Probá de nuevo.");
    } finally {
      setSubiendoFoto(false);
    }
  }

  async function cambiarEstado() {
    if (!estadoElegido || !user) return;
    setErrorEstado(null);
    setCambiandoEstado(true);
    try {
      const t = token ?? (await user.getIdToken());
      const actualizada = await cambiarEstadoDeOrden(t, id, estadoElegido);
      setEstado((actual) => (actual?.kind === "listo" ? { ...actual, orden: actualizada } : actual));
      setEstadoElegido("");
      // El cambio queda en el historial: se recarga solo eso, sin pisar la orden que devolvió el backend.
      const historial = await historialDeOrden(t, id).catch(() => null);
      if (historial) setEstado((actual) => (actual?.kind === "listo" ? { ...actual, historial: historial.items } : actual));
    } catch (error) {
      setErrorEstado(mensajeDeErrorDeEstado(error));
    } finally {
      setCambiandoEstado(false);
    }
  }

  function agregarComponente() {
    const componente = nuevoComponente.trim();
    if (!componente) return;
    setDatosTecnicos((actual) => {
      if (!actual) return actual;
      const yaEsta = actual.componentesAfectados.some((c) => c.toLowerCase() === componente.toLowerCase());
      return yaEsta ? actual : { ...actual, componentesAfectados: [...actual.componentesAfectados, componente] };
    });
    setNuevoComponente("");
  }

  function quitarComponente(componente: string) {
    setDatosTecnicos((actual) =>
      actual ? { ...actual, componentesAfectados: actual.componentesAfectados.filter((c) => c !== componente) } : actual,
    );
  }

  function cambiarTextoTecnico(campo: "pruebas" | "diagnostico" | "mediciones" | "notasInternas", valor: string) {
    setDatosTecnicos((actual) => (actual ? { ...actual, [campo]: valor } : actual));
  }

  async function guardarDatosTecnicos() {
    if (!datosTecnicos || !user) return;
    setErrorDatosTecnicos(null);
    setGuardandoDatosTecnicos(true);
    try {
      const t = token ?? (await user.getIdToken());
      const actualizada = await actualizarDatosTecnicos(t, id, datosDelForm(datosTecnicos));
      setEstado((actual) => (actual?.kind === "listo" ? { ...actual, orden: actualizada } : actual));
      setDatosTecnicos(formDeOrden(actualizada));
      // Si cambió algo queda en el historial: se recarga solo eso, sin pisar la orden que devolvió el backend.
      const historial = await historialDeOrden(t, id).catch(() => null);
      if (historial) setEstado((actual) => (actual?.kind === "listo" ? { ...actual, historial: historial.items } : actual));
    } catch (error) {
      setErrorDatosTecnicos(mensajeDeErrorDeDatosTecnicos(error));
    } finally {
      setGuardandoDatosTecnicos(false);
    }
  }

  function nuevaClaveDeFila() {
    return siguienteClaveDeFila.current++;
  }

  function agregarFilaPresupuesto() {
    setFilasPresupuesto((actuales) => [...actuales, filaVacia(nuevaClaveDeFila())]);
  }

  function quitarFilaPresupuesto(clave: number) {
    setFilasPresupuesto((actuales) => actuales.filter((f) => f.clave !== clave));
  }

  function cambiarFilaPresupuesto(clave: number, campo: "tipo" | "descripcion" | "monto", valor: string) {
    setFilasPresupuesto((actuales) => actuales.map((f) => (f.clave === clave ? { ...f, [campo]: valor } : f)));
  }

  async function guardarPresupuesto() {
    if (!user || !filasPresupuesto.some(filaCompleta)) return;
    setErrorPresupuesto(null);
    setGuardandoPresupuesto(true);
    try {
      const t = token ?? (await user.getIdToken());
      const actualizada = await crearVersionPresupuesto(t, id, itemsDeFilas(filasPresupuesto));
      setEstado((actual) => (actual?.kind === "listo" ? { ...actual, orden: actualizada } : actual));
      setFilasPresupuesto([filaVacia(nuevaClaveDeFila())]);
      // La lista de versiones anteriores quedó vieja: se vuelve a pedir la próxima vez que se despliegue.
      setVersiones(null);
      setMostrandoVersiones(false);
      // La versión nueva (y el cambio de estado, si lo hubo) quedan en el historial: se recarga solo eso.
      const historial = await historialDeOrden(t, id).catch(() => null);
      if (historial) setEstado((actual) => (actual?.kind === "listo" ? { ...actual, historial: historial.items } : actual));
    } catch (error) {
      setErrorPresupuesto(mensajeDeErrorDePresupuesto(error));
    } finally {
      setGuardandoPresupuesto(false);
    }
  }

  async function alternarVersionesAnteriores() {
    if (mostrandoVersiones) {
      setMostrandoVersiones(false);
      return;
    }
    setMostrandoVersiones(true);
    if (versiones || !user) return;
    setErrorVersiones(null);
    setCargandoVersiones(true);
    try {
      const t = token ?? (await user.getIdToken());
      setVersiones(await listarVersionesPresupuesto(t, id));
    } catch {
      setErrorVersiones("No se pudieron cargar las versiones anteriores del presupuesto.");
    } finally {
      setCargandoVersiones(false);
    }
  }

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
  const presupuestoVigente = orden?.presupuestoVigente ?? null;
  const versionesAnteriores = versiones?.filter((v) => v.id !== presupuestoVigente?.id) ?? [];
  const totalPresupuestoNuevo = filasPresupuesto.reduce((suma, f) => suma + montoDeFila(f), 0);

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

          {orden.noEnciende && (
            <p className="aviso-destacado" role="alert">
              El equipo no enciende: no se pudo revisar el resto y no se pudo estimar una fecha de entrega.
            </p>
          )}

          {puedeCambiarEstado && (
            <section aria-label="Estado de la orden" className="history">
              <h2 className="client-form-title">Estado</h2>
              {orden.estadosDisponibles.length === 0 ? (
                <p className="list-count">{nombreDeEstado(orden.estado)} es un estado final: la orden ya no cambia de estado.</p>
              ) : (
                <>
                  <div className="field">
                    <label htmlFor="nuevo-estado">Pasar a</label>
                    <select
                      id="nuevo-estado"
                      value={estadoElegido}
                      disabled={cambiandoEstado}
                      onChange={(e) => setEstadoElegido(e.target.value)}
                    >
                      <option value="">Elegí un estado</option>
                      {orden.estadosDisponibles.map((codigo) => (
                        <option key={codigo} value={codigo}>
                          {nombreDeEstado(codigo)}
                        </option>
                      ))}
                    </select>
                  </div>
                  <div className="form-actions">
                    <button
                      type="button"
                      className="btn"
                      disabled={!estadoElegido || cambiandoEstado}
                      onClick={() => void cambiarEstado()}
                    >
                      {cambiandoEstado ? "Cambiando…" : "Cambiar estado"}
                    </button>
                  </div>
                </>
              )}
              {errorEstado && (
                <p className="form-error" role="alert">
                  {errorEstado}
                </p>
              )}
            </section>
          )}

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
            {orden.checklist && Object.keys(orden.checklist.items).length > 0 && (
              <div>
                <dt>Checklist de ingreso</dt>
                <dd>
                  <ul className="checklist-resultado">
                    {CHECKLIST_CATALOGO.filter((item) => item.codigo in orden.checklist.items).map((item) => (
                      <li key={item.codigo}>
                        {item.nombre}: <strong>{orden.checklist.items[item.codigo] ? "sí" : "no"}</strong>
                      </li>
                    ))}
                  </ul>
                  {orden.checklist.saludBateria !== null && (
                    <p className="client-meta">Salud de batería: {orden.checklist.saludBateria}%</p>
                  )}
                  {orden.checklist.notas && <p className="client-meta texto-largo">Notas: {orden.checklist.notas}</p>}
                </dd>
              </div>
            )}
            <div>
              <dt>Entrega estimada</dt>
              <dd>
                {orden.noEnciende ? (
                  <div className="fecha-no-determinable">
                    <p className="fecha-no-determinable-valor">No se puede determinar</p>
                    <p className="client-meta">Razón: {RAZON_SIN_FECHA}</p>
                  </div>
                ) : orden.fechaEstimadaEntrega ? (
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

          {puedeEditarDatosTecnicos && datosTecnicos && (
            <section aria-label="Datos técnicos" className="history">
              <h2 className="client-form-title">Datos técnicos</h2>
              <div className="field">
                <label htmlFor="dt-pruebas">Pruebas realizadas</label>
                <textarea
                  id="dt-pruebas"
                  rows={3}
                  value={datosTecnicos.pruebas}
                  disabled={guardandoDatosTecnicos}
                  onChange={(e) => cambiarTextoTecnico("pruebas", e.target.value)}
                />
              </div>
              <div className="field">
                <label htmlFor="dt-diagnostico">Diagnóstico</label>
                <textarea
                  id="dt-diagnostico"
                  rows={3}
                  value={datosTecnicos.diagnostico}
                  disabled={guardandoDatosTecnicos}
                  onChange={(e) => cambiarTextoTecnico("diagnostico", e.target.value)}
                />
              </div>
              <div className="field">
                <label htmlFor="dt-mediciones">Mediciones</label>
                <textarea
                  id="dt-mediciones"
                  rows={3}
                  value={datosTecnicos.mediciones}
                  disabled={guardandoDatosTecnicos}
                  onChange={(e) => cambiarTextoTecnico("mediciones", e.target.value)}
                />
              </div>
              <div className="field">
                <label htmlFor="dt-componente">Componentes afectados</label>
                {datosTecnicos.componentesAfectados.length > 0 && (
                  <div className="chips" role="list" aria-label="Componentes afectados">
                    {datosTecnicos.componentesAfectados.map((componente) => (
                      <span key={componente} className="chip" role="listitem">
                        <span>
                          {componente}{" "}
                          <button
                            type="button"
                            className="btn btn-quiet btn-small"
                            aria-label={`Quitar ${componente}`}
                            disabled={guardandoDatosTecnicos}
                            onClick={() => quitarComponente(componente)}
                          >
                            ×
                          </button>
                        </span>
                      </span>
                    ))}
                  </div>
                )}
                <div className="form-actions">
                  <input
                    id="dt-componente"
                    value={nuevoComponente}
                    disabled={guardandoDatosTecnicos}
                    placeholder="Ej.: conector de carga"
                    onChange={(e) => setNuevoComponente(e.target.value)}
                    onKeyDown={(e) => {
                      if (e.key === "Enter") {
                        e.preventDefault();
                        agregarComponente();
                      }
                    }}
                  />
                  <button
                    type="button"
                    className="btn btn-quiet btn-small"
                    disabled={!nuevoComponente.trim() || guardandoDatosTecnicos}
                    onClick={agregarComponente}
                  >
                    Agregar
                  </button>
                </div>
              </div>
              <div className="field">
                <label htmlFor="dt-notas">Notas internas</label>
                <p className="client-meta">Privadas: no las ve el cliente.</p>
                <textarea
                  id="dt-notas"
                  rows={3}
                  value={datosTecnicos.notasInternas}
                  disabled={guardandoDatosTecnicos}
                  onChange={(e) => cambiarTextoTecnico("notasInternas", e.target.value)}
                />
              </div>
              <div className="form-actions">
                <button
                  type="button"
                  className="btn"
                  disabled={guardandoDatosTecnicos}
                  onClick={() => void guardarDatosTecnicos()}
                >
                  {guardandoDatosTecnicos ? "Guardando…" : "Guardar datos técnicos"}
                </button>
              </div>
              {errorDatosTecnicos && (
                <p className="form-error" role="alert">
                  {errorDatosTecnicos}
                </p>
              )}
            </section>
          )}

          <section aria-label="Presupuesto" className="history">
            <h2 className="client-form-title">Presupuesto</h2>
            {presupuestoVigente ? (
              <div>
                <p>
                  <strong>Versión {presupuestoVigente.numero}</strong>
                </p>
                <ul className="checklist-resultado" aria-label="Ítems del presupuesto vigente">
                  {presupuestoVigente.items.map((item, i) => (
                    <li key={i}>
                      {nombreDeTipoItemPresupuesto(item.tipo)}: {item.descripcion} — {formatearMonto(item.monto)}
                    </li>
                  ))}
                </ul>
                <p>
                  Total: <strong>{formatearMonto(presupuestoVigente.total)}</strong>
                </p>
                <p className="client-meta">
                  Creado{presupuestoVigente.creadoPor ? ` por ${presupuestoVigente.creadoPor}` : ""} el{" "}
                  {formatoFechaHora.format(new Date(presupuestoVigente.creadoEn))}
                </p>
                <div className="form-actions">
                  <button
                    type="button"
                    className="btn btn-quiet btn-small"
                    aria-expanded={mostrandoVersiones}
                    onClick={() => void alternarVersionesAnteriores()}
                  >
                    {mostrandoVersiones ? "Ocultar versiones anteriores" : "Ver versiones anteriores"}
                  </button>
                </div>
                {mostrandoVersiones && cargandoVersiones && (
                  <p className="list-count" role="status">
                    Cargando versiones…
                  </p>
                )}
                {mostrandoVersiones && errorVersiones && (
                  <p className="form-error" role="alert">
                    {errorVersiones}
                  </p>
                )}
                {mostrandoVersiones && versiones && (
                  versionesAnteriores.length === 0 ? (
                    <p className="list-count">No hay versiones anteriores.</p>
                  ) : (
                    <ol className="history-list" aria-label="Versiones anteriores del presupuesto">
                      {versionesAnteriores.map((v) => (
                        <li key={v.id} className="history-item">
                          <p className="history-head">
                            <span>
                              Versión {v.numero}: <strong>{formatearMonto(v.total)}</strong>
                            </span>
                            <span className="client-meta">
                              {v.creadoPor ? ` por ${v.creadoPor}` : ""} · {formatoFechaHora.format(new Date(v.creadoEn))}
                            </span>
                          </p>
                        </li>
                      ))}
                    </ol>
                  )
                )}
              </div>
            ) : (
              <p className="list-count">Todavía no se presupuestó esta orden.</p>
            )}

            {puedePresupuestar && (
              <div aria-label="Nueva versión del presupuesto" role="group">
                <h3 className="client-form-title">Nueva versión</h3>
                {filasPresupuesto.map((fila, i) => (
                  <div key={fila.clave} className="form-actions" role="group" aria-label={`Ítem ${i + 1}`}>
                    <select
                      aria-label="Tipo"
                      value={fila.tipo}
                      disabled={guardandoPresupuesto}
                      onChange={(e) => cambiarFilaPresupuesto(fila.clave, "tipo", e.target.value)}
                    >
                      {TIPOS_DE_ITEM_PRESUPUESTO.map((t) => (
                        <option key={t.codigo} value={t.codigo}>
                          {t.nombre}
                        </option>
                      ))}
                    </select>
                    <input
                      aria-label="Descripción"
                      value={fila.descripcion}
                      disabled={guardandoPresupuesto}
                      placeholder="Ej.: cambio de módulo"
                      onChange={(e) => cambiarFilaPresupuesto(fila.clave, "descripcion", e.target.value)}
                    />
                    <input
                      aria-label="Monto"
                      type="number"
                      min="0"
                      step="0.01"
                      inputMode="decimal"
                      value={fila.monto}
                      disabled={guardandoPresupuesto}
                      onChange={(e) => cambiarFilaPresupuesto(fila.clave, "monto", e.target.value)}
                    />
                    <button
                      type="button"
                      className="btn btn-quiet btn-small"
                      disabled={guardandoPresupuesto || filasPresupuesto.length === 1}
                      onClick={() => quitarFilaPresupuesto(fila.clave)}
                    >
                      Quitar fila
                    </button>
                  </div>
                ))}
                <div className="form-actions">
                  <button
                    type="button"
                    className="btn btn-quiet btn-small"
                    disabled={guardandoPresupuesto}
                    onClick={agregarFilaPresupuesto}
                  >
                    Agregar ítem
                  </button>
                </div>
                <p aria-live="polite">
                  Total: <strong>{formatearMonto(totalPresupuestoNuevo)}</strong>
                </p>
                <div className="form-actions">
                  <button
                    type="button"
                    className="btn"
                    disabled={guardandoPresupuesto || !filasPresupuesto.some(filaCompleta)}
                    onClick={() => void guardarPresupuesto()}
                  >
                    {guardandoPresupuesto ? "Guardando…" : "Guardar presupuesto"}
                  </button>
                </div>
                {errorPresupuesto && (
                  <p className="form-error" role="alert">
                    {errorPresupuesto}
                  </p>
                )}
              </div>
            )}
          </section>

          <section aria-label="Fotos del equipo" className="history">
            <h2 className="client-form-title">Fotos</h2>
            {fotos.length > 0 && token && (
              <div className="fotos-galeria">
                {fotos.map((foto) => <FotoMiniatura key={foto.id} token={token} ordenId={id} foto={foto} />)}
              </div>
            )}
            <label className="btn btn-quiet btn-small fotos-boton">
              {subiendoFoto ? "Subiendo…" : "Agregar fotos"}
              <input
                type="file"
                accept="image/jpeg,image/png,image/webp"
                multiple
                disabled={subiendoFoto}
                onChange={(e) => {
                  void subirFotos(e.target.files);
                  e.target.value = "";
                }}
                style={{ display: "none" }}
              />
            </label>
            {errorFoto && (
              <p className="form-error" role="alert">
                {errorFoto}
              </p>
            )}
          </section>

          <section aria-label="Historial de la orden" className="history">
            <h2 className="client-form-title">Historial</h2>
            <ol className="history-list">
              {estado.historial.map((r) => (
                <li key={r.id} className="history-item">
                  <p className="history-head">
                    <TextoDeRegistro registro={r} />
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
