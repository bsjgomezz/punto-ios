"use client";

const PUNTOS = [1, 2, 3, 4, 5, 6, 7, 8, 9];
const TAMANO = 168;
const PASO = TAMANO / 3;

function centro(punto: number) {
  const i = punto - 1;
  return { x: (i % 3) * PASO + PASO / 2, y: Math.floor(i / 3) * PASO + PASO / 2 };
}

type Props = {
  /** Puntos en el orden en que se tocan. */
  puntos: number[];
  /** Sin esta función la grilla es solo para mirar (por ejemplo, al mostrar el patrón guardado). */
  onChange?: (puntos: number[]) => void;
  etiqueta: string;
};

/** Grilla de 3×3 como la del celular: se tocan los puntos en orden y se dibuja el recorrido. */
export function PatronDesbloqueo({ puntos, onChange, etiqueta }: Props) {
  const editable = onChange !== undefined;
  const recorrido = puntos.map(centro);

  return (
    <div className="patron" role="group" aria-label={etiqueta}>
      <svg className="patron-trazo" viewBox={`0 0 ${TAMANO} ${TAMANO}`} aria-hidden="true">
        {recorrido.length > 1 && (
          <polyline
            points={recorrido.map((p) => `${p.x},${p.y}`).join(" ")}
            fill="none"
            stroke="currentColor"
            strokeWidth="5"
            strokeLinecap="round"
            strokeLinejoin="round"
          />
        )}
      </svg>
      {PUNTOS.map((p) => {
        const orden = puntos.indexOf(p);
        const marcado = orden >= 0;
        const contenido = (
          <>
            <span className="patron-punto" aria-hidden="true" />
            {marcado && <span className="patron-orden">{orden + 1}</span>}
          </>
        );
        return editable ? (
          <button
            key={p}
            type="button"
            className="patron-celda"
            data-marcado={marcado}
            aria-label={marcado ? `Punto ${p}, ${orden + 1}.º` : `Punto ${p}`}
            aria-pressed={marcado}
            disabled={marcado}
            onClick={() => onChange([...puntos, p])}
          >
            {contenido}
          </button>
        ) : (
          <span key={p} className="patron-celda" data-marcado={marcado}>
            {contenido}
          </span>
        );
      })}
    </div>
  );
}
