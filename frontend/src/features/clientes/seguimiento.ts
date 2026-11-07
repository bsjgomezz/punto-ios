import { apiPost } from "@/lib/api/client";

export type CodigoSeguimiento = { codigo: string; formateado: string; generadoEn: string };

/** Genera el código, o lo regenera: el link anterior deja de funcionar. */
export function generarCodigoSeguimiento(token: string, clienteId: string): Promise<CodigoSeguimiento> {
  return apiPost<CodigoSeguimiento>(`/api/v1/clientes/${encodeURIComponent(clienteId)}/seguimiento`, token, {});
}

/** Código vigente. El backend deja registrado quién lo consultó. */
export function consultarCodigoSeguimiento(token: string, clienteId: string): Promise<CodigoSeguimiento> {
  return apiPost<CodigoSeguimiento>(`/api/v1/clientes/${encodeURIComponent(clienteId)}/seguimiento/consultar`, token, {});
}

/** Dirección pública del sistema; si no está configurada, la de esta pestaña. */
export function urlPortal(): string {
  const configurada = process.env.NEXT_PUBLIC_PORTAL_URL?.trim();
  return (configurada || window.location.origin).replace(/\/+$/, "");
}

export function linkDeSeguimiento(codigo: string, base = urlPortal()): string {
  return `${base}/seguimiento/${codigo}`;
}

/**
 * Número para WhatsApp (formato internacional de Argentina: 54 9 + característica + número, sin 0 ni 15).
 * Solo se arma si quedan exactamente 10 dígitos; si no, devuelve null y WhatsApp deja elegir el contacto.
 */
export function numeroWhatsapp(telefono: string): string | null {
  let digitos = telefono.replace(/\D/g, "");
  if (digitos.startsWith("00")) digitos = digitos.slice(2);
  if (digitos.startsWith("54")) {
    digitos = digitos.slice(2);
    if (digitos.startsWith("9")) digitos = digitos.slice(1);
  } else if (digitos.startsWith("0")) {
    digitos = digitos.slice(1);
  }
  return digitos.length === 10 ? `549${digitos}` : null;
}

export function mensajeDeSeguimiento(nombre: string, link: string): string {
  return `¡Hola ${nombre}! Desde este link podés seguir el estado de tus equipos en Punto iOS: ${link}`;
}

export function urlWhatsapp(telefono: string, mensaje: string): string {
  const numero = numeroWhatsapp(telefono);
  const texto = encodeURIComponent(mensaje);
  return numero ? `https://wa.me/${numero}?text=${texto}` : `https://wa.me/?text=${texto}`;
}
