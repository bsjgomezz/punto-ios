import type { Dispositivo } from "./api";

const MODELOS_SUGERIDOS = [
  "iPhone 11",
  "iPhone 11 Pro",
  "iPhone 11 Pro Max",
  "iPhone SE (2.ª gen.)",
  "iPhone 12",
  "iPhone 12 mini",
  "iPhone 12 Pro",
  "iPhone 12 Pro Max",
  "iPhone 13",
  "iPhone 13 mini",
  "iPhone 13 Pro",
  "iPhone 13 Pro Max",
  "iPhone SE (3.ª gen.)",
  "iPhone 14",
  "iPhone 14 Plus",
  "iPhone 14 Pro",
  "iPhone 14 Pro Max",
  "iPhone 15",
  "iPhone 15 Plus",
  "iPhone 15 Pro",
  "iPhone 15 Pro Max",
  "iPhone 16",
  "iPhone 16 Plus",
  "iPhone 16 Pro",
  "iPhone 16 Pro Max",
  "iPhone 16e",
  "iPhone 17",
  "iPhone Air",
  "iPhone 17 Pro",
  "iPhone 17 Pro Max",
];

const ALMACENAMIENTOS = ["64 GB", "128 GB", "256 GB", "512 GB", "1 TB", "2 TB"];

type Props = {
  /** Prefijo de los id de los campos (debe ser único en la pantalla). */
  prefijo: string;
  /** Prefijo de los name, para distinguirlos de otros campos del mismo formulario. */
  nombre?: string;
  inicial?: Dispositivo;
  conObservaciones?: boolean;
  /** Marca y modelo obligatorios (en la ficha). En el alta del cliente todo el equipo es opcional. */
  obligatorio?: boolean;
};

/** Campos de un equipo: marca (Apple por defecto), modelo con sugerencias, almacenamiento, color e IMEI. */
export function CamposEquipo({ prefijo, nombre = "", inicial, conObservaciones = false, obligatorio = false }: Props) {
  const editando = inicial !== undefined;

  return (
    <>
      <div className="form-grid">
        <div className="field">
          <label htmlFor={`${prefijo}-marca`}>Marca</label>
          <input id={`${prefijo}-marca`} name={`${nombre}marca`} required={obligatorio} maxLength={50} defaultValue={inicial?.marca ?? "Apple"} />
        </div>
        <div className="field">
          <label htmlFor={`${prefijo}-modelo`}>Modelo</label>
          <input
            id={`${prefijo}-modelo`}
            name={`${nombre}modelo`}
            required={obligatorio}
            maxLength={100}
            list="modelos-sugeridos"
            defaultValue={inicial?.modelo}
            placeholder="iPhone 13 Pro"
          />
        </div>
        <div className="field">
          <label htmlFor={`${prefijo}-almacenamiento`}>Almacenamiento (opcional)</label>
          <input
            id={`${prefijo}-almacenamiento`}
            name={`${nombre}almacenamiento`}
            maxLength={30}
            list="almacenamientos"
            defaultValue={inicial?.almacenamiento ?? ""}
          />
        </div>
        <div className="field">
          <label htmlFor={`${prefijo}-color`}>Color (opcional)</label>
          <input id={`${prefijo}-color`} name={`${nombre}color`} maxLength={50} defaultValue={inicial?.color ?? ""} />
        </div>
        <div className="field field-wide">
          <label htmlFor={`${prefijo}-imei`}>
            {editando ? "IMEI o número de serie nuevo (opcional)" : "IMEI o número de serie (opcional)"}
          </label>
          <input
            id={`${prefijo}-imei`}
            name={`${nombre}imeiSerie`}
            maxLength={40}
            autoComplete="off"
            spellCheck={false}
            placeholder={
              editando && inicial?.imeiSerieEnmascarado
                ? `Sin cambios (termina en ${inicial.imeiSerieEnmascarado.slice(-4)})`
                : "Marcá *#06# en el equipo para verlo"
            }
          />
        </div>
        {conObservaciones && (
          <div className="field field-wide">
            <label htmlFor={`${prefijo}-observaciones`}>Observaciones (opcional)</label>
            <textarea
              id={`${prefijo}-observaciones`}
              name={`${nombre}observaciones`}
              rows={2}
              maxLength={1000}
              defaultValue={inicial?.observaciones ?? ""}
            />
          </div>
        )}
      </div>

      <datalist id="modelos-sugeridos">
        {MODELOS_SUGERIDOS.map((m) => (
          <option key={m} value={m} />
        ))}
      </datalist>
      <datalist id="almacenamientos">
        {ALMACENAMIENTOS.map((a) => (
          <option key={a} value={a} />
        ))}
      </datalist>
    </>
  );
}
