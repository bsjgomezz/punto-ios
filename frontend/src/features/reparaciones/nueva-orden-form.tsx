"use client";

import { useEffect, useState, type FormEvent } from "react";
import { useAuth } from "@/features/auth/auth-provider";
import type { Coincidencia } from "@/features/clientes/api";
import { listarDispositivos, type Dispositivo } from "@/features/dispositivos/api";
import { AvisoImeiDuplicado } from "@/features/dispositivos/aviso-imei-duplicado";
import { CamposEquipo } from "@/features/dispositivos/campos-equipo";
import { ApiError } from "@/lib/api/client";
import {
  AVISO_FECHA_ESTIMADA,
  NOMBRES_DESBLOQUEO,
  TIPOS_DE_REPARACION,
  listarTecnicos,
  recibirEquipo,
  type NuevaOrden,
  type Orden,
  type Persona,
  type TipoDesbloqueo,
} from "./api";
import { PatronDesbloqueo } from "./patron-desbloqueo";

type Props = {
  cliente: { id: string; nombre: string; apellido: string };
  onCreada: (orden: Orden) => void;
  onCancelar: () => void;
};

type Aviso = { kind: "error"; mensaje: string } | { kind: "imei"; duenio: Coincidencia | null };

const OTRO_EQUIPO = "otro";

function hoy() {
  const ahora = new Date();
  const dosDigitos = (n: number) => String(n).padStart(2, "0");
  return `${ahora.getFullYear()}-${dosDigitos(ahora.getMonth() + 1)}-${dosDigitos(ahora.getDate())}`;
}

/** Recepción de un equipo: crea la orden de reparación. */
export function NuevaOrdenForm({ cliente, onCreada, onCancelar }: Props) {
  const { user } = useAuth();
  const [equipos, setEquipos] = useState<Dispositivo[] | null>(null);
  const [tecnicos, setTecnicos] = useState<Persona[]>([]);
  const [equipoElegido, setEquipoElegido] = useState<string | null>(null);
  const [tipos, setTipos] = useState<string[]>([]);
  const [desbloqueo, setDesbloqueo] = useState<TipoDesbloqueo | "">("");
  const [patron, setPatron] = useState<number[]>([]);
  const [enviando, setEnviando] = useState(false);
  const [aviso, setAviso] = useState<Aviso | null>(null);

  useEffect(() => {
    if (!user) return;
    const controller = new AbortController();
    user
      .getIdToken()
      .then((token) =>
        Promise.all([
          listarDispositivos(token, cliente.id, controller.signal),
          listarTecnicos(token, controller.signal).catch(() => [] as Persona[]),
        ]),
      )
      .then(([lista, personas]) => {
        setEquipos(lista);
        setTecnicos(personas);
        setEquipoElegido((actual) => actual ?? (lista.length === 1 ? lista[0].id : lista.length === 0 ? OTRO_EQUIPO : null));
      })
      .catch(() => {
        if (!controller.signal.aborted) setEquipos([]);
      });
    return () => controller.abort();
  }, [user, cliente.id]);

  function alternarTipo(codigo: string) {
    setTipos((actual) => (actual.includes(codigo) ? actual.filter((t) => t !== codigo) : [...actual, codigo]));
  }

  async function onSubmit(evento: FormEvent<HTMLFormElement>) {
    evento.preventDefault();
    if (!user) return;
    const datos = new FormData(evento.currentTarget);
    const texto = (campo: string) => String(datos.get(campo) ?? "").trim();

    if (!equipoElegido) {
      setAviso({ kind: "error", mensaje: "Elegí qué equipo deja el cliente." });
      return;
    }
    if (tipos.length === 0) {
      setAviso({ kind: "error", mensaje: "Marcá al menos un tipo de reparación." });
      return;
    }

    const orden: NuevaOrden = {
      clienteId: cliente.id,
      problemaInformado: texto("problema"),
      tiposReparacion: tipos,
      otroTipoDescripcion: tipos.includes("otro") ? texto("otroTipo") || undefined : undefined,
      tecnicoId: texto("tecnico") || undefined,
      fechaEstimadaEntrega: texto("fechaEstimada") || undefined,
    };
    if (equipoElegido === OTRO_EQUIPO) {
      orden.equipoNuevo = {
        marca: texto("equipo_marca"),
        modelo: texto("equipo_modelo"),
        almacenamiento: texto("equipo_almacenamiento") || undefined,
        color: texto("equipo_color") || undefined,
        imeiSerie: texto("equipo_imeiSerie") || undefined,
      };
    } else {
      orden.dispositivoId = equipoElegido;
    }
    if (desbloqueo) {
      orden.desbloqueoTipo = desbloqueo;
      orden.desbloqueoValor = desbloqueo === "patron" ? patron.join("-") : texto("desbloqueoValor");
    }

    setEnviando(true);
    setAviso(null);
    try {
      onCreada(await recibirEquipo(await user.getIdToken(), orden));
    } catch (error) {
      if (error instanceof ApiError && error.problem?.codigo === "imei_duplicado") {
        setAviso({ kind: "imei", duenio: (error.problem.cliente as Coincidencia | null) ?? null });
      } else if (error instanceof ApiError && error.status === 403) {
        setAviso({ kind: "error", mensaje: "Tu rol no permite recibir equipos." });
      } else if (error instanceof ApiError && error.status >= 400 && error.status < 500) {
        setAviso({ kind: "error", mensaje: error.message });
      } else {
        setAviso({ kind: "error", mensaje: "No se pudo guardar la orden. Verificá la conexión e intentá de nuevo." });
      }
    } finally {
      setEnviando(false);
    }
  }

  return (
    <form className="client-form" onSubmit={onSubmit} aria-label="Recibir equipo">
      <h2 className="client-form-title">
        Recibir equipo de {cliente.apellido}, {cliente.nombre}
      </h2>

      <fieldset className="orden-bloque">
        <legend>Equipo</legend>
        {equipos === null ? (
          <p className="client-meta">Cargando equipos…</p>
        ) : (
          <div className="opciones-equipo">
            {equipos.map((e) => (
              <label key={e.id} className="opcion">
                <input
                  type="radio"
                  name="equipoElegido"
                  checked={equipoElegido === e.id}
                  onChange={() => setEquipoElegido(e.id)}
                />
                <span>
                  {e.marca} {e.modelo}
                  <span className="client-meta">
                    {" "}
                    {[e.almacenamiento, e.color, e.imeiSerieEnmascarado].filter(Boolean).join(" · ")}
                  </span>
                </span>
              </label>
            ))}
            <label className="opcion">
              <input
                type="radio"
                name="equipoElegido"
                checked={equipoElegido === OTRO_EQUIPO}
                onChange={() => setEquipoElegido(OTRO_EQUIPO)}
              />
              <span>{equipos.length === 0 ? "Cargar el equipo" : "Otro equipo"}</span>
            </label>
          </div>
        )}
        {equipoElegido === OTRO_EQUIPO && <CamposEquipo prefijo="orden-equipo" nombre="equipo_" obligatorio />}
      </fieldset>

      <fieldset className="orden-bloque">
        <legend>Qué le pasa</legend>
        <div className="field field-wide">
          <label htmlFor="orden-problema">Problema que cuenta el cliente</label>
          <textarea id="orden-problema" name="problema" rows={3} required maxLength={2000} />
        </div>

        <p className="chips-titulo" id="tipos-titulo">
          Tipo de reparación
        </p>
        <div className="chips" role="group" aria-labelledby="tipos-titulo">
          {TIPOS_DE_REPARACION.map((t) => (
            <label key={t.codigo} className="chip">
              <input type="checkbox" checked={tipos.includes(t.codigo)} onChange={() => alternarTipo(t.codigo)} />
              <span>{t.nombre}</span>
            </label>
          ))}
        </div>
        {tipos.includes("otro") && (
          <div className="field field-wide">
            <label htmlFor="orden-otro">¿Qué otra reparación?</label>
            <input id="orden-otro" name="otroTipo" required maxLength={200} />
          </div>
        )}
      </fieldset>

      <fieldset className="orden-bloque">
        <legend>Asignación y entrega</legend>
        <div className="form-grid">
          <div className="field">
            <label htmlFor="orden-tecnico">Técnico</label>
            <select id="orden-tecnico" name="tecnico" defaultValue="">
              <option value="">Sin asignar</option>
              {tecnicos.map((t) => (
                <option key={t.id} value={t.id}>
                  {t.nombre}
                </option>
              ))}
            </select>
          </div>
          <div className="field">
            <label htmlFor="orden-fecha">Fecha estimada de entrega (opcional)</label>
            <input id="orden-fecha" name="fechaEstimada" type="date" min={hoy()} aria-describedby="orden-fecha-aviso" />
          </div>
        </div>
        <p className="fieldset-hint" id="orden-fecha-aviso">
          {AVISO_FECHA_ESTIMADA}
        </p>
      </fieldset>

      <fieldset className="orden-bloque">
        <legend>Desbloqueo (opcional)</legend>
        <p className="fieldset-hint">Se guarda cifrado, solo lo ven técnicos y administración, y se borra al entregar el equipo.</p>
        <div className="chips" role="radiogroup" aria-label="Tipo de desbloqueo">
          {(["", "pin", "contrasena", "patron"] as const).map((tipo) => (
            <label key={tipo || "ninguno"} className="chip">
              <input
                type="radio"
                name="desbloqueoTipo"
                checked={desbloqueo === tipo}
                onChange={() => {
                  setDesbloqueo(tipo);
                  setPatron([]);
                }}
              />
              <span>{tipo ? NOMBRES_DESBLOQUEO[tipo] : "No lo deja"}</span>
            </label>
          ))}
        </div>
        {(desbloqueo === "pin" || desbloqueo === "contrasena") && (
          <div className="field">
            <label htmlFor="orden-desbloqueo">{desbloqueo === "pin" ? "PIN" : "Contraseña"}</label>
            <input
              id="orden-desbloqueo"
              name="desbloqueoValor"
              required
              autoComplete="off"
              spellCheck={false}
              inputMode={desbloqueo === "pin" ? "numeric" : "text"}
              maxLength={desbloqueo === "pin" ? 16 : 100}
            />
          </div>
        )}
        {desbloqueo === "patron" && (
          <div className="patron-editor">
            <PatronDesbloqueo puntos={patron} onChange={setPatron} etiqueta="Patrón de desbloqueo" />
            <div>
              <p className="client-meta">Tocá los puntos en el orden en que el cliente dibuja el patrón.</p>
              <button type="button" className="btn btn-quiet btn-small" onClick={() => setPatron([])} disabled={patron.length === 0}>
                Borrar patrón
              </button>
            </div>
          </div>
        )}
      </fieldset>

      {aviso?.kind === "imei" && <AvisoImeiDuplicado duenio={aviso.duenio} />}
      {aviso?.kind === "error" && (
        <p className="form-error" role="alert">
          {aviso.mensaje}
        </p>
      )}

      <div className="form-actions">
        <button type="submit" className="btn" disabled={enviando}>
          {enviando ? "Guardando…" : "Recibir equipo"}
        </button>
        <button type="button" className="btn btn-quiet" onClick={onCancelar}>
          Cancelar
        </button>
      </div>
    </form>
  );
}
