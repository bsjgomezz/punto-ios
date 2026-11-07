"use client";

import { useState } from "react";
import { useAuth } from "@/features/auth/auth-provider";
import type { Cliente } from "./api";
import {
  consultarCodigoSeguimiento,
  generarCodigoSeguimiento,
  linkDeSeguimiento,
  mensajeDeSeguimiento,
  numeroWhatsapp,
  urlWhatsapp,
  type CodigoSeguimiento,
} from "./seguimiento";

type Props = { cliente: Cliente; onCambio: () => void };

const formatoFecha = new Intl.DateTimeFormat("es-AR", { dateStyle: "medium", timeStyle: "short" });

/** Código de seguimiento del cliente: generar, ver, enviar por WhatsApp y regenerar. Todo queda registrado. */
export function SeguimientoCliente({ cliente, onCambio }: Props) {
  const { user } = useAuth();
  const [codigo, setCodigo] = useState<CodigoSeguimiento | null>(null);
  const [trabajando, setTrabajando] = useState(false);
  const [confirmarRegenerar, setConfirmarRegenerar] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [aviso, setAviso] = useState<string | null>(null);

  // Recién generado, el código ya está en pantalla aunque los datos del cliente todavía no se hayan recargado.
  const generadoEn = codigo?.generadoEn ?? cliente.codigoSeguimientoGeneradoEn ?? null;
  const tieneCodigo = generadoEn !== null;
  const sinNumero = numeroWhatsapp(cliente.telefono) === null;

  async function conToken<T>(accion: (token: string) => Promise<T>): Promise<T | null> {
    if (!user) return null;
    setTrabajando(true);
    setError(null);
    try {
      return await accion(await user.getIdToken());
    } catch {
      setError("No se pudo completar la acción. Verificá la conexión e intentá de nuevo.");
      return null;
    } finally {
      setTrabajando(false);
    }
  }

  async function generar(regenerar: boolean) {
    setConfirmarRegenerar(false);
    const nuevo = await conToken((t) => generarCodigoSeguimiento(t, cliente.id));
    if (!nuevo) return;
    setCodigo(nuevo);
    setAviso(regenerar ? "Código regenerado. El link anterior ya no funciona." : "Código generado.");
    onCambio();
  }

  async function ver() {
    const actual = await conToken((t) => consultarCodigoSeguimiento(t, cliente.id));
    if (actual) setCodigo(actual);
  }

  async function enviarPorWhatsapp() {
    // La pestaña se abre en el momento del clic para que el navegador no la bloquee; la dirección se completa después.
    const pestana = window.open("", "_blank");
    const actual = codigo ?? (await conToken((t) => consultarCodigoSeguimiento(t, cliente.id)));
    if (!actual) {
      pestana?.close();
      return;
    }
    setCodigo(actual);
    const url = urlWhatsapp(cliente.telefono, mensajeDeSeguimiento(cliente.nombre, linkDeSeguimiento(actual.codigo)));
    if (pestana) {
      pestana.opener = null;
      pestana.location.href = url;
    } else {
      window.location.href = url;
    }
  }

  return (
    <section aria-label="Seguimiento del cliente" className="history">
      <h2 className="client-form-title">Seguimiento</h2>

      {!tieneCodigo ? (
        <>
          <p className="list-count">
            Todavía no tiene código de seguimiento. Con el código, el cliente puede ver el estado de sus equipos desde un link,
            sin cuenta.
          </p>
          <button type="button" className="btn" disabled={trabajando} onClick={() => void generar(false)}>
            Generar código
          </button>
        </>
      ) : (
        <>
          <p className="list-count">
            Código generado el {formatoFecha.format(new Date(generadoEn!))}.
          </p>

          {codigo && (
            <div className="tracking-code">
              <code aria-label="Código de seguimiento">{codigo.formateado}</code>
              <p className="client-meta">{linkDeSeguimiento(codigo.codigo)}</p>
            </div>
          )}

          <div className="form-actions">
            <button type="button" className="btn btn-whatsapp" disabled={trabajando} onClick={() => void enviarPorWhatsapp()}>
              <svg aria-hidden="true" viewBox="0 0 24 24" width="18" height="18" fill="currentColor">
                <path d="M12 2a10 10 0 0 0-8.6 15.1L2 22l5-1.3A10 10 0 1 0 12 2Zm0 18.2c-1.5 0-3-.4-4.3-1.2l-.3-.2-3 .8.8-2.9-.2-.3A8.2 8.2 0 1 1 12 20.2Zm4.5-6.1c-.2-.1-1.5-.7-1.7-.8-.2-.1-.4-.1-.6.1l-.8 1c-.1.2-.3.2-.5.1a6.7 6.7 0 0 1-3.3-2.9c-.2-.4.2-.4.7-1.3.1-.2 0-.3 0-.4l-.8-1.8c-.2-.5-.4-.4-.6-.4h-.5a1 1 0 0 0-.7.3 3 3 0 0 0-.9 2.2 5.2 5.2 0 0 0 1.1 2.8 11.9 11.9 0 0 0 4.6 4c1.7.7 2.3.8 3.2.6.5-.1 1.5-.6 1.8-1.2.2-.6.2-1.1.1-1.2l-.5-.3Z" />
              </svg>
              Enviar por WhatsApp
            </button>
            {!codigo && (
              <button type="button" className="btn btn-quiet" disabled={trabajando} onClick={() => void ver()}>
                Ver código
              </button>
            )}
            {!confirmarRegenerar && (
              <button type="button" className="btn btn-quiet" disabled={trabajando} onClick={() => setConfirmarRegenerar(true)}>
                Regenerar
              </button>
            )}
          </div>

          {sinNumero && (
            <p className="client-meta">
              El teléfono no tiene 10 dígitos (característica + número, sin 0 ni 15): WhatsApp te va a pedir que elijas el
              contacto.
            </p>
          )}

          {confirmarRegenerar && (
            <div className="dup-warning" role="alert">
              <p>Si lo regenerás, el link que ya le enviaste al cliente deja de funcionar. ¿Querés regenerarlo?</p>
              <div className="form-actions">
                <button type="button" className="btn" disabled={trabajando} onClick={() => void generar(true)}>
                  Sí, regenerar
                </button>
                <button type="button" className="btn btn-quiet" onClick={() => setConfirmarRegenerar(false)}>
                  Cancelar
                </button>
              </div>
            </div>
          )}
        </>
      )}

      {aviso && (
        <p className="identity" data-state="ok" role="status">
          {aviso}
        </p>
      )}
      {error && (
        <p className="form-error" role="alert">
          {error}
        </p>
      )}
    </section>
  );
}
