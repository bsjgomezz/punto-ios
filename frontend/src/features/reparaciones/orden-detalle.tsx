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
  ESTADOS_ENTREGABLES,
  METODOS_DE_PAGO,
  NOMBRES_DESBLOQUEO,
  OPCIONES_DE_GARANTIA,
  RAZON_SIN_FECHA,
  TIPOS_DE_ITEM_PRESUPUESTO,
  actualizarDatosTecnicos,
  cambiarEstadoDeOrden,
  crearVersionPresupuesto,
  entregarOrden,
  formatearFecha,
  formatearMonto,
  listarFotos,
  listarVersionesPresupuesto,
  nombreDeTipoItemPresupuesto,
  nombreDeEstado,
  nombreDeGarantia,
  nombreDeMetodoDePago,
  nombreDeTipo,
  obtenerFotoUrl,
  obtenerOrden,
  responderPresupuesto,
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
  presupuesto_respondido: "Presupuesto respondido",
  entrega_registrada: "Registró la entrega",
};

/** Nombre para mostrar de cada campo auditado de la orden (snake_case en el backend). */
const NOMBRES_DE_CAMPO: Record<string, string> = {
  pruebas: "Pruebas",
  diagnostico: "Diagnóstico",
  mediciones: "Mediciones",
  componentes_afectados: "Componentes afectados",
  trabajo_realizado: "Trabajo realizado",
  notas_internas: "Notas internas",
  version_presupuesto: "Versión del presupuesto",
  total_presupuesto: "Total del presupuesto",
  garantia: "Garantía",
  monto_cobrado: "Monto cobrado",
  metodo_pago: "Método de pago",
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

function mensajeDeErrorDeRespuesta(error: unknown): string {
  if (error instanceof ApiError && error.status === 403) return "Tu rol no permite aprobar o rechazar presupuestos.";
  if (error instanceof ApiError && error.problem?.title) return error.problem.title;
  return "No se pudo registrar la respuesta. Verificá la conexión e intentá de nuevo.";
}

function mensajeDeErrorDeEntrega(error: unknown): string {
  if (error instanceof ApiError && error.status === 403) return "Tu rol no permite entregar equipos ni registrar cobros.";
  if (error instanceof ApiError && error.problem?.title) return error.problem.title;
  return "No se pudo registrar la entrega. Verificá la conexión e intentá de nuevo.";
}

/** El monto de la entrega va como string (tal cual el input): vale si es un número >= 0 (0 es válido). */
function montoDeEntrega(texto: string): number | null {
  if (texto.trim() === "") return null;
  const monto = Number(texto);
  return Number.isFinite(monto) && monto >= 0 ? monto : null;
}

/**
 * Monto a prellenar al entregar: el total del presupuesto si el cliente lo aprobó; si no, vacío (hay que cargarlo a
 * conciencia, por ejemplo el cargo de revisión de una orden irreparable o rechazada).
 */
const montoInicialDeEntrega = (orden: Orden) =>
  orden.presupuestoVigente?.respuestaAprobada === true ? String(orden.presupuestoVigente.total) : "";

/**
 * Valor de un cambio del registro de entrega para mostrar. El backend guarda el nombre de la garantía y del método
 * (no el código) y el monto como número con punto decimal; si llegara un código se traduce igual.
 */
function valorDeEntrega(campo: string, valor: string | null): string {
  if (valor === null) return "—";
  if (campo === "garantia") return nombreDeGarantia(valor);
  if (campo === "metodo_pago") return nombreDeMetodoDePago(valor);
  if (campo === "monto_cobrado") {
    const monto = Number(valor);
    return valor.trim() !== "" && Number.isFinite(monto) ? formatearMonto(monto) : valor;
  }
  return valor;
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
  trabajoRealizado: string;
  notasInternas: string;
};

function formDeOrden(orden: Orden): FormDatosTecnicos {
  return {
    pruebas: orden.pruebas ?? "",
    diagnostico: orden.diagnostico ?? "",
    mediciones: orden.mediciones ?? "",
    componentesAfectados: orden.componentesAfectados ?? [],
    trabajoRealizado: orden.trabajoRealizado ?? "",
    notasInternas: orden.notasInternas ?? "",
  };
}

const textoONull = (texto: string) => (texto.trim() === "" ? null : texto);

/** El backend reemplaza los seis campos juntos: se mandan siempre todos, tal cual están en pantalla. */
function datosDelForm(form: FormDatosTecnicos): DatosTecnicos {
  return {
    pruebas: textoONull(form.pruebas),
    diagnostico: textoONull(form.diagnostico),
    mediciones: textoONull(form.mediciones),
    componentesAfectados: form.componentesAfectados,
    trabajoRealizado: textoONull(form.trabajoRealizado),
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
  if (registro.accion === "presupuesto_respondido" && registro.cambios && registro.cambios.length > 0) {
    const version = registro.cambios.find((c) => c.campo === "version_presupuesto")?.nuevo;
    const respuesta = registro.cambios.find((c) => c.campo === "respuesta")?.nuevo;
    return (
      <span>
        {respuesta === "aprobado" ? "Aprobó" : "Rechazó"} la versión <strong>{version ?? "?"}</strong> del presupuesto
      </span>
    );
  }
  if (registro.accion === "entrega_registrada" && registro.cambios && registro.cambios.length > 0) {
    return (
      <span>
        Registró la entrega:{" "}
        {registro.cambios.map((c, i) => (
          <span key={c.campo}>
            {i > 0 && " · "}
            {NOMBRES_DE_CAMPO[c.campo] ?? c.campo}: <strong>{valorDeEntrega(c.campo, c.nuevo)}</strong>
          </span>
        ))}
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
  const puedeEntregar = me.kind === "listo" && me.me.permisos.includes("reparaciones.entregar");

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
  // Qué respuesta se está registrando (null si ninguna): sirve para deshabilitar los dos botones y rotular el que se apretó.
  const [respondiendo, setRespondiendo] = useState<"aprobar" | "rechazar" | null>(null);
  const [errorRespuesta, setErrorRespuesta] = useState<string | null>(null);
  const [garantiaEntrega, setGarantiaEntrega] = useState("");
  // null hasta que carga la orden: así una recarga (p. ej. al ver el desbloqueo) no pisa lo que ya se escribió.
  const [montoEntregaCargado, setMontoEntrega] = useState<string | null>(null);
  const [metodoPagoEntrega, setMetodoPagoEntrega] = useState("");
  const [entregando, setEntregando] = useState(false);
  const [errorEntrega, setErrorEntrega] = useState<string | null>(null);

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
        setMontoEntrega((actual) => actual ?? montoInicialDeEntrega(orden));
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

  function cambiarTextoTecnico(campo: "pruebas" | "diagnostico" | "mediciones" | "trabajoRealizado" | "notasInternas", valor: string) {
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

  async function responder(aprobado: boolean) {
    if (!user) return;
    setErrorRespuesta(null);
    setRespondiendo(aprobado ? "aprobar" : "rechazar");
    try {
      const t = token ?? (await user.getIdToken());
      const actualizada = await responderPresupuesto(t, id, aprobado);
      setEstado((actual) => (actual?.kind === "listo" ? { ...actual, orden: actualizada } : actual));
      // La lista de versiones anteriores quedó vieja (la vigente ahora tiene respuesta): se vuelve a pedir al desplegarla.
      setVersiones(null);
      setMostrandoVersiones(false);
      // La respuesta (y el cambio de estado) quedan en el historial: se recarga solo eso, sin pisar la orden.
      const historial = await historialDeOrden(t, id).catch(() => null);
      if (historial) setEstado((actual) => (actual?.kind === "listo" ? { ...actual, historial: historial.items } : actual));
    } catch (error) {
      setErrorRespuesta(mensajeDeErrorDeRespuesta(error));
    } finally {
      setRespondiendo(null);
    }
  }

  async function entregar() {
    const montoCobrado = montoDeEntrega(montoEntregaCargado ?? "");
    if (!user || !garantiaEntrega || !metodoPagoEntrega || montoCobrado === null) return;
    setErrorEntrega(null);
    setEntregando(true);
    try {
      const t = token ?? (await user.getIdToken());
      const actualizada = await entregarOrden(t, id, {
        garantiaCodigo: garantiaEntrega,
        montoCobrado,
        metodoPago: metodoPagoEntrega,
      });
      setEstado((actual) => (actual?.kind === "listo" ? { ...actual, orden: actualizada } : actual));
      // La entrega y el cambio de estado quedan en el historial: se recarga solo eso, sin pisar la orden.
      const historial = await historialDeOrden(t, id).catch(() => null);
      if (historial) setEstado((actual) => (actual?.kind === "listo" ? { ...actual, historial: historial.items } : actual));
    } catch (error) {
      setErrorEntrega(mensajeDeErrorDeEntrega(error));
    } finally {
      setEntregando(false);
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
  const montoEntrega = montoEntregaCargado ?? "";
  // `!= null`: una orden sin los campos de entrega (undefined) cuenta como no entregada.
  const yaEntregada = orden?.entregadoEn != null;
  const esEntregable = orden !== null && ESTADOS_ENTREGABLES.includes(orden.estado);
  const puedeEnviarEntrega =
    !entregando && garantiaEntrega !== "" && metodoPagoEntrega !== "" && montoDeEntrega(montoEntrega) !== null;

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
              {orden.estado === "esperando_aprobacion" ? (
                // El combo genérico no ofrece aprobado / presupuesto_rechazado: eso se hace respondiendo el presupuesto.
                <p className="list-count">Para aprobar o rechazar, usá la sección &quot;Presupuesto&quot; más abajo.</p>
              ) : (
                <>
                  {esEntregable && puedeEntregar && (
                    // "entregado" no viene en estadosDisponibles: se llega solo registrando la entrega (con su cobro y garantía).
                    <p className="list-count">Para entregar el equipo, usá la sección &quot;Entrega&quot; más abajo.</p>
                  )}
                  {orden.estadosDisponibles.length === 0 ? (
                    !esEntregable ? (
                      <p className="list-count">{nombreDeEstado(orden.estado)} es un estado final: la orden ya no cambia de estado.</p>
                    ) : (
                      !puedeEntregar && <p className="list-count">No hay otros cambios de estado disponibles.</p>
                    )
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
                <label htmlFor="dt-trabajo-realizado">Trabajo realizado</label>
                <textarea
                  id="dt-trabajo-realizado"
                  rows={3}
                  value={datosTecnicos.trabajoRealizado}
                  disabled={guardandoDatosTecnicos}
                  onChange={(e) => cambiarTextoTecnico("trabajoRealizado", e.target.value)}
                />
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
                {presupuestoVigente.respuestaAprobada !== null && (
                  <p className={presupuestoVigente.respuestaAprobada ? "client-meta" : "form-error"}>
                    {presupuestoVigente.respuestaAprobada ? "Aprobado" : "Rechazado"}
                    {presupuestoVigente.respondidoPor ? ` por ${presupuestoVigente.respondidoPor}` : ""}
                    {presupuestoVigente.respondidoEn
                      ? ` el ${formatoFechaHora.format(new Date(presupuestoVigente.respondidoEn))}`
                      : ""}
                  </p>
                )}
                {puedePresupuestar && orden.estado === "esperando_aprobacion" && presupuestoVigente.respuestaAprobada === null && (
                  <div className="form-actions" role="group" aria-label="Respuesta del cliente">
                    <button type="button" className="btn" disabled={respondiendo !== null} onClick={() => void responder(true)}>
                      {respondiendo === "aprobar" ? "Aprobando…" : "Aprobar"}
                    </button>
                    <button
                      type="button"
                      className="btn btn-quiet"
                      disabled={respondiendo !== null}
                      onClick={() => void responder(false)}
                    >
                      {respondiendo === "rechazar" ? "Rechazando…" : "Rechazar"}
                    </button>
                  </div>
                )}
                {errorRespuesta && (
                  <p className="form-error" role="alert">
                    {errorRespuesta}
                  </p>
                )}
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

          {(yaEntregada || (puedeEntregar && esEntregable)) && (
            <section aria-label="Entrega" className="history">
              <h2 className="client-form-title">Entrega</h2>
              {yaEntregada ? (
                <dl className="detail-list">
                  <div>
                    <dt>Entregado</dt>
                    <dd>{formatoFechaHora.format(new Date(orden.entregadoEn!))}</dd>
                  </div>
                  <div>
                    <dt>Monto cobrado</dt>
                    <dd>{orden.montoCobrado != null ? formatearMonto(orden.montoCobrado) : "—"}</dd>
                  </div>
                  <div>
                    <dt>Método de pago</dt>
                    <dd>{orden.metodoPago ? nombreDeMetodoDePago(orden.metodoPago) : "—"}</dd>
                  </div>
                  <div>
                    <dt>Garantía</dt>
                    <dd>
                      {orden.garantiaCodigo ? nombreDeGarantia(orden.garantiaCodigo) : "—"}
                      {orden.garantiaVenceEn && (
                        <span className="client-meta"> · vence el {formatearFecha(orden.garantiaVenceEn)}</span>
                      )}
                    </dd>
                  </div>
                </dl>
              ) : (
                <>
                  <div className="field">
                    <label htmlFor="entrega-garantia">Garantía</label>
                    <select
                      id="entrega-garantia"
                      value={garantiaEntrega}
                      disabled={entregando}
                      onChange={(e) => setGarantiaEntrega(e.target.value)}
                    >
                      <option value="">Elegí la garantía</option>
                      {OPCIONES_DE_GARANTIA.map((g) => (
                        <option key={g.codigo} value={g.codigo}>
                          {g.nombre}
                        </option>
                      ))}
                    </select>
                  </div>
                  <div className="field">
                    <label htmlFor="entrega-monto">Monto cobrado</label>
                    <input
                      id="entrega-monto"
                      type="number"
                      min="0"
                      step="0.01"
                      inputMode="decimal"
                      value={montoEntrega}
                      disabled={entregando}
                      onChange={(e) => setMontoEntrega(e.target.value)}
                    />
                  </div>
                  <div className="field">
                    <label htmlFor="entrega-metodo-pago">Método de pago</label>
                    <select
                      id="entrega-metodo-pago"
                      value={metodoPagoEntrega}
                      disabled={entregando}
                      onChange={(e) => setMetodoPagoEntrega(e.target.value)}
                    >
                      <option value="">Elegí el método de pago</option>
                      {METODOS_DE_PAGO.map((m) => (
                        <option key={m.codigo} value={m.codigo}>
                          {m.nombre}
                        </option>
                      ))}
                    </select>
                  </div>
                  <div className="form-actions">
                    <button type="button" className="btn" disabled={!puedeEnviarEntrega} onClick={() => void entregar()}>
                      {entregando ? "Entregando…" : "Entregar equipo"}
                    </button>
                  </div>
                  {errorEntrega && (
                    <p className="form-error" role="alert">
                      {errorEntrega}
                    </p>
                  )}
                </>
              )}
            </section>
          )}

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
