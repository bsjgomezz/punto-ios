import { apiGet, apiPost, apiPut } from "@/lib/api/client";

export type Dispositivo = {
  id: string;
  clienteId: string;
  marca: string;
  modelo: string;
  almacenamiento: string | null;
  color: string | null;
  imeiSerieEnmascarado: string | null;
  observaciones: string | null;
  creadoEn: string;
};

export type DatosDispositivo = {
  marca: string;
  modelo: string;
  almacenamiento?: string;
  color?: string;
  observaciones?: string;
};

export type NuevoDispositivo = DatosDispositivo & { imeiSerie?: string };

/** Si `actualizarImei` es falso el IMEI queda como está (la pantalla no lo conoce completo). */
export type EdicionDispositivo = DatosDispositivo & { actualizarImei: boolean; imeiSerie?: string };

export function listarDispositivos(token: string, clienteId: string, signal?: AbortSignal): Promise<Dispositivo[]> {
  return apiGet<Dispositivo[]>(`/api/v1/clientes/${encodeURIComponent(clienteId)}/dispositivos`, token, signal);
}

export function crearDispositivo(token: string, clienteId: string, datos: NuevoDispositivo): Promise<Dispositivo> {
  return apiPost<Dispositivo>(`/api/v1/clientes/${encodeURIComponent(clienteId)}/dispositivos`, token, datos);
}

export function actualizarDispositivo(token: string, id: string, datos: EdicionDispositivo): Promise<Dispositivo> {
  return apiPut<Dispositivo>(`/api/v1/dispositivos/${encodeURIComponent(id)}`, token, datos);
}

/** Devuelve el IMEI completo. El backend deja registrado quién lo consultó. */
export function revelarImei(token: string, id: string): Promise<{ imeiSerie: string | null }> {
  return apiPost<{ imeiSerie: string | null }>(`/api/v1/dispositivos/${encodeURIComponent(id)}/imei`, token, {});
}
