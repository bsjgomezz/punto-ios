import { apiGet, apiGetBlob, apiPost, apiPostForm, apiPut } from "@/lib/api/client";
import type { Pagina } from "@/features/clientes/api";
import type { Dispositivo } from "@/features/dispositivos/api";

/** Mismo catálogo que el backend (que es quien valida). */
export const TIPOS_DE_REPARACION: { codigo: string; nombre: string }[] = [
  { codigo: "diagnostico", nombre: "Diagnóstico" },
  { codigo: "pantalla", nombre: "Pantalla" },
  { codigo: "bateria", nombre: "Batería" },
  { codigo: "conector_carga", nombre: "Conector de carga" },
  { codigo: "camara", nombre: "Cámara" },
  { codigo: "audio", nombre: "Audio" },
  { codigo: "software", nombre: "Software" },
  { codigo: "microsoldadura", nombre: "Microsoldadura" },
  { codigo: "face_id", nombre: "Face ID" },
  { codigo: "cambio_ic", nombre: "Cambio de IC" },
  { codigo: "placa", nombre: "Placa" },
  { codigo: "recuperacion_datos", nombre: "Recuperación de datos" },
  { codigo: "otro", nombre: "Otro" },
];

export const nombreDeTipo = (codigo: string) => TIPOS_DE_REPARACION.find((t) => t.codigo === codigo)?.nombre ?? codigo;

/** Mismo catálogo que el backend (`GET /reparaciones/estados`), en el orden del ciclo de la orden. */
export const ESTADOS: { codigo: string; nombre: string }[] = [
  { codigo: "recibido", nombre: "Recibido" },
  { codigo: "en_pruebas", nombre: "En pruebas" },
  { codigo: "en_diagnostico", nombre: "En diagnóstico" },
  { codigo: "esperando_presupuesto", nombre: "Esperando presupuesto" },
  { codigo: "presupuesto_enviado", nombre: "Presupuesto enviado" },
  { codigo: "esperando_aprobacion", nombre: "Esperando aprobación" },
  { codigo: "presupuesto_rechazado", nombre: "Presupuesto rechazado" },
  { codigo: "aprobado", nombre: "Aprobado" },
  { codigo: "en_reparacion", nombre: "En reparación" },
  { codigo: "esperando_repuesto", nombre: "Esperando repuesto" },
  { codigo: "reparacion_completada", nombre: "Reparación completada" },
  { codigo: "listo_para_retirar", nombre: "Listo para retirar" },
  { codigo: "entregado", nombre: "Entregado" },
  { codigo: "cancelado", nombre: "Cancelado" },
  { codigo: "irreparable", nombre: "Irreparable" },
];

export const nombreDeEstado = (codigo: string) => ESTADOS.find((e) => e.codigo === codigo)?.nombre ?? codigo;

/** Mismo catálogo que el backend (TiposDeItemPresupuesto). */
export const TIPOS_DE_ITEM_PRESUPUESTO: { codigo: string; nombre: string }[] = [
  { codigo: "mano_obra", nombre: "Mano de obra" },
  { codigo: "repuesto", nombre: "Repuesto" },
];

export const nombreDeTipoItemPresupuesto = (codigo: string) =>
  TIPOS_DE_ITEM_PRESUPUESTO.find((t) => t.codigo === codigo)?.nombre ?? codigo;

export type ItemPresupuesto = { descripcion: string; tipo: string; monto: number };

/** Una versión del presupuesto. Nunca se edita: para cambiarlo se crea una versión nueva. */
export type VersionDePresupuesto = {
  id: string;
  numero: number;
  items: ItemPresupuesto[];
  total: number;
  creadoPor: string | null;
  creadoEn: string;
  /** true = aprobada, false = rechazada, null = todavía sin respuesta. */
  respuestaAprobada: boolean | null;
  respondidoEn: string | null;
  /** Nombre de quién del staff registró la respuesta. */
  respondidoPor: string | null;
};

export type TipoDesbloqueo = "pin" | "contrasena" | "patron";

export const NOMBRES_DESBLOQUEO: Record<TipoDesbloqueo, string> = {
  pin: "PIN",
  contrasena: "Contraseña",
  patron: "Patrón",
};

export type Persona = { id: string; nombre: string };

export type Orden = {
  id: string;
  numero: number;
  estado: string;
  cliente: { id: string; nombre: string; apellido: string; telefono: string };
  equipo: Dispositivo;
  problemaInformado: string;
  tiposReparacion: string[];
  otroTipoDescripcion: string | null;
  tecnico: Persona | null;
  fechaEstimadaEntrega: string | null;
  desbloqueoTipo: TipoDesbloqueo | null;
  creadoPor: string | null;
  creadoEn: string;
  checklist: Checklist;
  noEnciende: boolean;
  /** Códigos de estado a los que se puede pasar desde el actual (vacío si es un estado final). */
  estadosDisponibles: string[];
  pruebas: string | null;
  diagnostico: string | null;
  mediciones: string | null;
  componentesAfectados: string[];
  trabajoRealizado: string | null;
  /** Privadas del taller: no las ve el cliente. */
  notasInternas: string | null;
  /** La versión de presupuesto de número más alto, o null si todavía no se presupuestó. */
  presupuestoVigente: VersionDePresupuesto | null;
};

/** Datos técnicos que carga el técnico. El backend los reemplaza todos juntos: un campo en `null` se borra. */
export type DatosTecnicos = {
  pruebas: string | null;
  diagnostico: string | null;
  mediciones: string | null;
  componentesAfectados: string[];
  trabajoRealizado: string | null;
  notasInternas: string | null;
};

export type OrdenResumen = {
  id: string;
  numero: number;
  estado: string;
  clienteId: string;
  cliente: string;
  equipo: string;
  tecnico: string | null;
  fechaEstimadaEntrega: string | null;
  creadoEn: string;
};

export type NuevaOrden = {
  clienteId: string;
  dispositivoId?: string;
  equipoNuevo?: { marca: string; modelo: string; almacenamiento?: string; color?: string; imeiSerie?: string };
  problemaInformado: string;
  tiposReparacion: string[];
  otroTipoDescripcion?: string;
  tecnicoId?: string;
  fechaEstimadaEntrega?: string;
  desbloqueoTipo?: TipoDesbloqueo;
  desbloqueoValor?: string;
  checklist?: { items: Record<string, boolean>; saludBateria?: number; notas?: string };
};

export function listarOrdenes(
  token: string,
  {
    clienteId,
    page = 1,
    tecnicoId,
    soloActivas,
  }: { clienteId?: string; page?: number; tecnicoId?: string; soloActivas?: boolean },
  signal?: AbortSignal,
): Promise<Pagina<OrdenResumen>> {
  const params = new URLSearchParams({ page: String(page), pageSize: "20" });
  if (clienteId) params.set("clienteId", clienteId);
  if (tecnicoId) params.set("tecnicoId", tecnicoId);
  if (soloActivas !== undefined) params.set("soloActivas", String(soloActivas));
  return apiGet<Pagina<OrdenResumen>>(`/api/v1/reparaciones?${params}`, token, signal);
}

export function obtenerOrden(token: string, id: string, signal?: AbortSignal): Promise<Orden> {
  return apiGet<Orden>(`/api/v1/reparaciones/${encodeURIComponent(id)}`, token, signal);
}

export function recibirEquipo(token: string, datos: NuevaOrden): Promise<Orden> {
  return apiPost<Orden>("/api/v1/reparaciones", token, datos);
}

/** Pasa la orden a otro estado; el backend valida que la transición esté permitida y la deja en el historial. */
export function cambiarEstadoDeOrden(token: string, ordenId: string, nuevoEstado: string): Promise<Orden> {
  return apiPost<Orden>(`/api/v1/reparaciones/${encodeURIComponent(ordenId)}/estado`, token, { nuevoEstado });
}

/** Reemplaza los datos técnicos de la orden: hay que mandar siempre los seis campos (lo que no se manda se borra). */
export function actualizarDatosTecnicos(token: string, ordenId: string, datos: DatosTecnicos): Promise<Orden> {
  return apiPut<Orden>(`/api/v1/reparaciones/${encodeURIComponent(ordenId)}/datos-tecnicos`, token, datos);
}

/** Crea una versión nueva del presupuesto (nunca se edita una existente); el backend puede mover el estado de la orden. */
export function crearVersionPresupuesto(token: string, ordenId: string, items: ItemPresupuesto[]): Promise<Orden> {
  return apiPost<Orden>(`/api/v1/reparaciones/${encodeURIComponent(ordenId)}/presupuestos`, token, { items });
}

/** Todas las versiones de presupuesto de la orden, de la más nueva a la más vieja. */
export function listarVersionesPresupuesto(token: string, ordenId: string, signal?: AbortSignal): Promise<VersionDePresupuesto[]> {
  return apiGet<VersionDePresupuesto[]>(`/api/v1/reparaciones/${encodeURIComponent(ordenId)}/presupuestos`, token, signal);
}

/** Aprueba o rechaza la versión vigente del presupuesto; la orden puede cambiar de estado. */
export function responderPresupuesto(token: string, ordenId: string, aprobado: boolean): Promise<Orden> {
  return apiPost<Orden>(`/api/v1/reparaciones/${encodeURIComponent(ordenId)}/presupuestos/respuesta`, token, { aprobado });
}

/** Igual formato que usa el backend para mostrar montos en pesos argentinos. */
export function formatearMonto(monto: number): string {
  return new Intl.NumberFormat("es-AR", { style: "currency", currency: "ARS" }).format(monto);
}

export function listarTecnicos(token: string, signal?: AbortSignal): Promise<Persona[]> {
  return apiGet<Persona[]>("/api/v1/reparaciones/tecnicos", token, signal);
}

/** El backend deja registrado quién lo vio. */
export function verDesbloqueo(token: string, id: string): Promise<{ tipo: TipoDesbloqueo; valor: string }> {
  return apiPost<{ tipo: TipoDesbloqueo; valor: string }>(`/api/v1/reparaciones/${encodeURIComponent(id)}/desbloqueo`, token, {});
}

/** Sube una foto del equipo (JPG, PNG o WebP, hasta 10 MB; hasta 10 por orden). */
export function subirFoto(token: string, ordenId: string, archivo: File): Promise<Foto> {
  const datos = new FormData();
  datos.append("foto", archivo);
  return apiPostForm<Foto>(`/api/v1/reparaciones/${encodeURIComponent(ordenId)}/fotos`, token, datos);
}

export function listarFotos(token: string, ordenId: string, signal?: AbortSignal): Promise<Foto[]> {
  return apiGet<Foto[]>(`/api/v1/reparaciones/${encodeURIComponent(ordenId)}/fotos`, token, signal);
}

/** Descarga la foto y devuelve una URL de objeto local para mostrarla (hay que revocarla al dejar de usarla). */
export async function obtenerFotoUrl(token: string, ordenId: string, fotoId: string, signal?: AbortSignal): Promise<string> {
  const blob = await apiGetBlob(`/api/v1/reparaciones/${encodeURIComponent(ordenId)}/fotos/${encodeURIComponent(fotoId)}`, token, signal);
  return URL.createObjectURL(blob);
}

export function formatearFecha(fecha: string): string {
  const [anio, mes, dia] = fecha.split("-").map(Number);
  return new Intl.DateTimeFormat("es-AR", { dateStyle: "long" }).format(new Date(anio, mes - 1, dia));
}

export const AVISO_FECHA_ESTIMADA =
  "Es orientativa: se calcula con lo que cuenta el cliente. Si al revisar el equipo aparecen otros problemas (por ejemplo, si llegó sin encender y no se pudo probar), la fecha puede cambiar y se le avisa.";

/** Mismo catálogo que el backend (`ItemsDeChecklist.Catalogo`): código, grupo y texto para la pantalla. */
export const CHECKLIST_CATALOGO: { codigo: string; grupo: string; nombre: string }[] = [
  { codigo: "enciende", grupo: "Encendido", nombre: "Enciende" },
  { codigo: "carga", grupo: "Encendido", nombre: "Carga" },
  { codigo: "pantalla", grupo: "Encendido", nombre: "La pantalla da imagen" },
  { codigo: "tactil", grupo: "Encendido", nombre: "El táctil responde" },
  { codigo: "camara_trasera", grupo: "Funciones", nombre: "Cámara trasera" },
  { codigo: "camara_frontal", grupo: "Funciones", nombre: "Cámara frontal" },
  { codigo: "face_id", grupo: "Funciones", nombre: "Face ID / Touch ID" },
  { codigo: "microfono", grupo: "Funciones", nombre: "Micrófono" },
  { codigo: "parlante", grupo: "Funciones", nombre: "Parlante" },
  { codigo: "auricular", grupo: "Funciones", nombre: "Auricular (llamadas)" },
  { codigo: "senal", grupo: "Funciones", nombre: "Señal" },
  { codigo: "wifi", grupo: "Funciones", nombre: "Wi-Fi" },
  { codigo: "botones", grupo: "Funciones", nombre: "Botones" },
  { codigo: "icloud", grupo: "Batería y cuenta", nombre: "iCloud / Buscar mi iPhone activo" },
  { codigo: "sim", grupo: "Batería y cuenta", nombre: "Trae chip / SIM" },
];

/** Ítem del checklist que, si se marca que no funciona, impide estimar una fecha de entrega. */
export const ITEM_ENCIENDE = "enciende";

/** Mismo texto que el backend (`ItemsDeChecklist.RazonSinFecha`). */
export const RAZON_SIN_FECHA = "No enciende y no se puede determinar la falla en su totalidad.";

export type Checklist = {
  items: Record<string, boolean>;
  saludBateria: number | null;
  notas: string | null;
};

export type Foto = { id: string; creadoEn: string };
