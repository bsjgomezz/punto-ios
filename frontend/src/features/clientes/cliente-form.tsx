"use client";

import { useState, type FormEvent } from "react";
import { useAuth } from "@/features/auth/auth-provider";
import { ApiError } from "@/lib/api/client";
import { AvisoImeiDuplicado } from "@/features/dispositivos/aviso-imei-duplicado";
import { CamposEquipo } from "@/features/dispositivos/campos-equipo";
import type { Cliente, Coincidencia, DatosCliente, EquipoNuevo } from "./api";

type Props = {
  titulo: string;
  etiqueta: string;
  etiquetaGuardar: string;
  inicial?: Cliente;
  mensajeSinPermiso: string;
  /** Muestra la sección opcional para cargar el equipo que trae el cliente (solo en el alta). */
  conEquipo?: boolean;
  guardar: (token: string, datos: DatosCliente) => Promise<Cliente>;
  onGuardado: (cliente: Cliente) => void;
  onCancelar: () => void;
};

type Aviso =
  | { kind: "error"; mensaje: string }
  | { kind: "duplicado"; coincidencias: Coincidencia[] }
  | { kind: "imei"; duenio: Coincidencia | null };

/** Arma el equipo solo si se completó algo más que la marca (que viene con "Apple" por defecto). */
function leerEquipo(texto: (campo: string) => string): EquipoNuevo | undefined {
  const equipo = {
    marca: texto("equipo_marca"),
    modelo: texto("equipo_modelo"),
    almacenamiento: texto("equipo_almacenamiento") || undefined,
    color: texto("equipo_color") || undefined,
    imeiSerie: texto("equipo_imeiSerie") || undefined,
  };
  const cargado = equipo.modelo || equipo.almacenamiento || equipo.color || equipo.imeiSerie;
  return cargado ? equipo : undefined;
}

/** Formulario de cliente para alta y edición. Si el teléfono ya existe muestra quién lo tiene y deja guardarlo igual. */
export function ClienteForm({
  titulo,
  etiqueta,
  etiquetaGuardar,
  inicial,
  mensajeSinPermiso,
  conEquipo = false,
  guardar,
  onGuardado,
  onCancelar,
}: Props) {
  const { user } = useAuth();
  const [enviando, setEnviando] = useState(false);
  const [aviso, setAviso] = useState<Aviso | null>(null);

  async function enviar(formulario: HTMLFormElement, permitirDuplicado: boolean) {
    if (!user) return;
    const datos = new FormData(formulario);
    const texto = (campo: string) => String(datos.get(campo) ?? "").trim();

    setEnviando(true);
    setAviso(null);
    try {
      const token = await user.getIdToken();
      const cliente = await guardar(token, {
        nombre: texto("nombre"),
        apellido: texto("apellido"),
        telefono: texto("telefono"),
        email: texto("email") || undefined,
        direccion: texto("direccion") || undefined,
        observaciones: texto("observaciones") || undefined,
        permitirDuplicado,
        equipo: conEquipo ? leerEquipo(texto) : undefined,
      });
      onGuardado(cliente);
    } catch (error) {
      if (error instanceof ApiError && error.problem?.codigo === "imei_duplicado") {
        setAviso({ kind: "imei", duenio: (error.problem.cliente as Coincidencia | null) ?? null });
      } else if (error instanceof ApiError && error.problem?.codigo === "telefono_duplicado") {
        setAviso({ kind: "duplicado", coincidencias: (error.problem.coincidencias as Coincidencia[]) ?? [] });
      } else if (error instanceof ApiError && error.status === 403) {
        setAviso({ kind: "error", mensaje: mensajeSinPermiso });
      } else if (error instanceof ApiError && error.status >= 400 && error.status < 500) {
        setAviso({ kind: "error", mensaje: error.message });
      } else {
        setAviso({ kind: "error", mensaje: "No se pudo guardar el cliente. Verificá la conexión e intentá de nuevo." });
      }
    } finally {
      setEnviando(false);
    }
  }

  function onSubmit(evento: FormEvent<HTMLFormElement>) {
    evento.preventDefault();
    void enviar(evento.currentTarget, false);
  }

  return (
    <form className="client-form" onSubmit={onSubmit} aria-label={etiqueta}>
      <h2 className="client-form-title">{titulo}</h2>

      <div className="form-grid">
        <div className="field">
          <label htmlFor="cliente-nombre">Nombre</label>
          <input id="cliente-nombre" name="nombre" autoComplete="given-name" required maxLength={100} defaultValue={inicial?.nombre} />
        </div>
        <div className="field">
          <label htmlFor="cliente-apellido">Apellido</label>
          <input id="cliente-apellido" name="apellido" autoComplete="family-name" required maxLength={100} defaultValue={inicial?.apellido} />
        </div>
        <div className="field">
          <label htmlFor="cliente-telefono">Teléfono</label>
          <input id="cliente-telefono" name="telefono" type="tel" inputMode="tel" autoComplete="tel" required maxLength={50} defaultValue={inicial?.telefono} />
        </div>
        <div className="field">
          <label htmlFor="cliente-email">Email (opcional)</label>
          <input id="cliente-email" name="email" type="email" autoComplete="email" maxLength={254} defaultValue={inicial?.email ?? ""} />
        </div>
        <div className="field field-wide">
          <label htmlFor="cliente-direccion">Dirección (opcional)</label>
          <input id="cliente-direccion" name="direccion" autoComplete="street-address" maxLength={200} defaultValue={inicial?.direccion ?? ""} />
        </div>
        <div className="field field-wide">
          <label htmlFor="cliente-observaciones">Observaciones (opcional)</label>
          <textarea id="cliente-observaciones" name="observaciones" rows={3} maxLength={1000} defaultValue={inicial?.observaciones ?? ""} />
        </div>
      </div>

      {conEquipo && (
        <fieldset className="equipo-fieldset">
          <legend>Equipo que trae (opcional)</legend>
          <p className="fieldset-hint">Si lo completás, se guarda junto con el cliente. Podés agregar más equipos después desde su ficha.</p>
          <CamposEquipo prefijo="alta-equipo" nombre="equipo_" />
        </fieldset>
      )}

      {aviso?.kind === "imei" && <AvisoImeiDuplicado duenio={aviso.duenio} />}

      {aviso?.kind === "error" && (
        <p className="form-error" role="alert">
          {aviso.mensaje}
        </p>
      )}

      {aviso?.kind === "duplicado" && (
        <div className="dup-warning" role="alert">
          <p>
            Ya hay {aviso.coincidencias.length === 1 ? "un cliente" : "clientes"} con ese teléfono:
          </p>
          <ul>
            {aviso.coincidencias.map((c) => (
              <li key={c.id}>
                {c.apellido}, {c.nombre} <span className="client-meta">{c.telefono}</span>
              </li>
            ))}
          </ul>
          <p>Si es otra persona que comparte el número, podés guardarlo igual.</p>
        </div>
      )}

      <div className="form-actions">
        {aviso?.kind === "duplicado" && (
          <button
            type="button"
            className="btn"
            disabled={enviando}
            onClick={(e) => void enviar(e.currentTarget.form!, true)}
          >
            Guardar igual
          </button>
        )}
        {aviso?.kind !== "duplicado" && (
          <button type="submit" className="btn" disabled={enviando}>
            {enviando ? "Guardando…" : etiquetaGuardar}
          </button>
        )}
        <button type="button" className="btn btn-quiet" onClick={onCancelar}>
          Cancelar
        </button>
      </div>
    </form>
  );
}
