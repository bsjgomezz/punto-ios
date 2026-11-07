import { apiGet, apiPost, apiPut } from "@/lib/api/client";

export type Cliente = {
  id: string;
  nombre: string;
  apellido: string;
  telefono: string;
  email: string | null;
  direccion: string | null;
  observaciones: string | null;
  creadoEn: string;
};

export type Coincidencia = Pick<Cliente, "id" | "nombre" | "apellido" | "telefono">;

export type Pagina<T> = { items: T[]; page: number; pageSize: number; totalCount: number; totalPages: number };

export type NuevoCliente = {
  nombre: string;
  apellido: string;
  telefono: string;
  email?: string;
  direccion?: string;
  observaciones?: string;
  permitirDuplicado?: boolean;
  /** Equipo que trae el cliente; se guarda junto con el cliente (solo en el alta). */
  equipo?: EquipoNuevo;
};

export type EquipoNuevo = {
  marca: string;
  modelo: string;
  almacenamiento?: string;
  color?: string;
  imeiSerie?: string;
};

export const TAMANO_PAGINA = 20;

export function buscarClientes(
  token: string,
  { q, page }: { q: string; page: number },
  signal?: AbortSignal,
): Promise<Pagina<Cliente>> {
  const params = new URLSearchParams({ page: String(page), pageSize: String(TAMANO_PAGINA) });
  if (q.trim()) params.set("q", q.trim());
  return apiGet<Pagina<Cliente>>(`/api/v1/clientes?${params}`, token, signal);
}

export function crearCliente(token: string, cliente: NuevoCliente): Promise<Cliente> {
  return apiPost<Cliente>("/api/v1/clientes", token, cliente);
}

export type CambioDeCampo = { campo: string; anterior: string | null; nuevo: string | null };

export type RegistroHistorial = {
  id: string;
  accion: "creado" | "editado" | string;
  usuarioId: string | null;
  usuario: string | null;
  fecha: string;
  cambios: CambioDeCampo[];
};

export type DatosCliente = NuevoCliente;

export function obtenerCliente(token: string, id: string, signal?: AbortSignal): Promise<Cliente> {
  return apiGet<Cliente>(`/api/v1/clientes/${encodeURIComponent(id)}`, token, signal);
}

export function actualizarCliente(token: string, id: string, datos: DatosCliente): Promise<Cliente> {
  return apiPut<Cliente>(`/api/v1/clientes/${encodeURIComponent(id)}`, token, datos);
}

export function historialDeCliente(token: string, id: string, signal?: AbortSignal): Promise<Pagina<RegistroHistorial>> {
  return apiGet<Pagina<RegistroHistorial>>(
    `/api/v1/clientes/${encodeURIComponent(id)}/historial?pageSize=50`,
    token,
    signal,
  );
}
