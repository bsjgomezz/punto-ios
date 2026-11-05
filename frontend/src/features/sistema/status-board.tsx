"use client";

import { useSyncExternalStore } from "react";
import { REMOTE_CHECKS } from "./checks";
import { useRemoteChecks } from "./use-remote-checks";

type RowState = "checking" | "ok" | "error";

type Row = {
  id: string;
  label: string;
  hint: string;
  state: RowState;
  statusText: string;
  help?: string;
};

function subscribeToConnection(onChange: () => void) {
  window.addEventListener("online", onChange);
  window.addEventListener("offline", onChange);
  return () => {
    window.removeEventListener("online", onChange);
    window.removeEventListener("offline", onChange);
  };
}

function useOnline() {
  return useSyncExternalStore(
    subscribeToConnection,
    () => navigator.onLine,
    () => true,
  );
}

export function StatusBoard() {
  const online = useOnline();
  const { results, recheck } = useRemoteChecks();

  const rows: Row[] = [
    {
      id: "aplicacion",
      label: "Aplicación web",
      hint: "Esta pantalla",
      state: "ok",
      statusText: "Funcionando",
    },
    {
      id: "internet",
      label: "Internet",
      hint: "Conexión de este dispositivo",
      state: online ? "ok" : "error",
      statusText: online ? "En línea" : "Sin conexión",
      help: "Este dispositivo no tiene internet. Hasta que vuelva, el servidor no va a responder.",
    },
    ...REMOTE_CHECKS.map((check): Row => {
      const result = results[check.id];
      return {
        id: check.id,
        label: check.label,
        hint: check.hint,
        state: result.state,
        statusText:
          result.state === "ok"
            ? check.okText
            : result.state === "error"
              ? check.errorText
              : "Comprobando…",
        help: result.state === "error" ? check.help : undefined,
      };
    }),
  ];

  const checking = rows.some((row) => row.state === "checking");

  return (
    <section className="board" aria-labelledby="board-title">
      <div className="board-head">
        <h2 className="board-title" id="board-title">
          Estado del sistema
        </h2>
        <button type="button" className="btn" onClick={recheck} disabled={checking}>
          Comprobar de nuevo
        </button>
      </div>

      <ul className="checks" aria-live="polite">
        {rows.map((row) => (
          <li className="check" key={row.id}>
            <span className="lamp" data-state={row.state} aria-hidden="true" />
            <div>
              <p className="check-name">{row.label}</p>
              <p className="check-hint">{row.hint}</p>
            </div>
            <p className="check-status" data-state={row.state}>
              {row.statusText}
            </p>
            {row.state === "error" && row.help ? (
              <p className="check-help">{row.help}</p>
            ) : null}
          </li>
        ))}
      </ul>
    </section>
  );
}
