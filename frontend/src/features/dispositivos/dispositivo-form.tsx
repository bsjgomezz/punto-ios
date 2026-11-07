"use client";

import { useState, type FormEvent } from "react";
import { useAuth } from "@/features/auth/auth-provider";
import { ApiError } from "@/lib/api/client";
import type { Coincidencia } from "@/features/clientes/api";
import { actualizarDispositivo, crearDispositivo, type Dispositivo } from "./api";
import { AvisoImeiDuplicado } from "./aviso-imei-duplicado";
import { CamposEquipo } from "./campos-equipo";

type Props = {
  clienteId: string;
  /** Si viene, el formulario edita ese equipo; si no, da de alta uno nuevo. */
  inicial?: Dispositivo;
  onGuardado: (dispositivo: Dispositivo) => void;
  onCancelar: () => void;
};

type Aviso = { kind: "error"; mensaje: string } | { kind: "duplicado"; duenio: Coincidencia | null };

/** Alta y edición de un equipo. Un IMEI ya registrado en otro equipo no se puede guardar. */
export function DispositivoForm({ clienteId, inicial, onGuardado, onCancelar }: Props) {
  const { user } = useAuth();
  const [enviando, setEnviando] = useState(false);
  const [aviso, setAviso] = useState<Aviso | null>(null);
  const editando = inicial !== undefined;
  const prefijo = inicial ? `equipo-${inicial.id}` : "equipo-nuevo";

  async function onSubmit(evento: FormEvent<HTMLFormElement>) {
    evento.preventDefault();
    if (!user) return;
    const datos = new FormData(evento.currentTarget);
    const texto = (campo: string) => String(datos.get(campo) ?? "").trim();
    const base = {
      marca: texto("marca"),
      modelo: texto("modelo"),
      almacenamiento: texto("almacenamiento") || undefined,
      color: texto("color") || undefined,
      observaciones: texto("observaciones") || undefined,
    };
    const imei = texto("imeiSerie");

    setEnviando(true);
    setAviso(null);
    try {
      const token = await user.getIdToken();
      const guardado = inicial
        ? await actualizarDispositivo(token, inicial.id, { ...base, actualizarImei: imei !== "", imeiSerie: imei || undefined })
        : await crearDispositivo(token, clienteId, { ...base, imeiSerie: imei || undefined });
      onGuardado(guardado);
    } catch (error) {
      if (error instanceof ApiError && error.problem?.codigo === "imei_duplicado") {
        setAviso({ kind: "duplicado", duenio: (error.problem.cliente as Coincidencia | null) ?? null });
      } else if (error instanceof ApiError && error.status === 403) {
        setAviso({ kind: "error", mensaje: "Tu rol no permite guardar equipos." });
      } else if (error instanceof ApiError && error.status >= 400 && error.status < 500) {
        setAviso({ kind: "error", mensaje: error.message });
      } else {
        setAviso({ kind: "error", mensaje: "No se pudo guardar el equipo. Verificá la conexión e intentá de nuevo." });
      }
    } finally {
      setEnviando(false);
    }
  }

  const titulo = editando ? "Editar equipo" : "Nuevo equipo";

  return (
    <form className="client-form" onSubmit={onSubmit} aria-label={titulo}>
      <h3 className="client-form-title">{titulo}</h3>

      <CamposEquipo prefijo={prefijo} inicial={inicial} conObservaciones obligatorio />

      {aviso?.kind === "error" && (
        <p className="form-error" role="alert">
          {aviso.mensaje}
        </p>
      )}
      {aviso?.kind === "duplicado" && <AvisoImeiDuplicado duenio={aviso.duenio} />}

      <div className="form-actions">
        <button type="submit" className="btn" disabled={enviando}>
          {enviando ? "Guardando…" : editando ? "Guardar cambios" : "Guardar equipo"}
        </button>
        <button type="button" className="btn btn-quiet" onClick={onCancelar}>
          Cancelar
        </button>
      </div>
    </form>
  );
}
