import { apiGet, apiPost } from "@/lib/api/client";
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

export const ESTADOS: Record<string, string> = { recibido: "Recibido" };

export const nombreDeEstado = (estado: string) => ESTADOS[estado] ?? estado;

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
};

export function listarOrdenes(
  token: string,
  { clienteId, page = 1 }: { clienteId?: string; page?: number },
  signal?: AbortSignal,
): Promise<Pagina<OrdenResumen>> {
  const params = new URLSearchParams({ page: String(page), pageSize: "20" });
  if (clienteId) params.set("clienteId", clienteId);
  return apiGet<Pagina<OrdenResumen>>(`/api/v1/reparaciones?${params}`, token, signal);
}

export function obtenerOrden(token: string, id: string, signal?: AbortSignal): Promise<Orden> {
  return apiGet<Orden>(`/api/v1/reparaciones/${encodeURIComponent(id)}`, token, signal);
}

export function recibirEquipo(token: string, datos: NuevaOrden): Promise<Orden> {
  return apiPost<Orden>("/api/v1/reparaciones", token, datos);
}

export function listarTecnicos(token: string, signal?: AbortSignal): Promise<Persona[]> {
  return apiGet<Persona[]>("/api/v1/reparaciones/tecnicos", token, signal);
}

/** El backend deja registrado quién lo vio. */
export function verDesbloqueo(token: string, id: string): Promise<{ tipo: TipoDesbloqueo; valor: string }> {
  return apiPost<{ tipo: TipoDesbloqueo; valor: string }>(`/api/v1/reparaciones/${encodeURIComponent(id)}/desbloqueo`, token, {});
}

export function formatearFecha(fecha: string): string {
  const [anio, mes, dia] = fecha.split("-").map(Number);
  return new Intl.DateTimeFormat("es-AR", { dateStyle: "long" }).format(new Date(anio, mes - 1, dia));
}

export const AVISO_FECHA_ESTIMADA =
  "Es orientativa: se calcula con lo que cuenta el cliente. Si al revisar el equipo aparecen otros problemas (por ejemplo, si llegó sin encender y no se pudo probar), la fecha puede cambiar y se le avisa.";
