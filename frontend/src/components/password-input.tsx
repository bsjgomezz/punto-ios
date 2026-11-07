"use client";

import { useState } from "react";

type Props = { id: string; name: string; autoComplete: string };

/**
 * Campo de contraseña con botón para verla. Ojo cerrado mientras está oculta,
 * ojo abierto mientras se muestra.
 */
export function PasswordInput({ id, name, autoComplete }: Props) {
  const [visible, setVisible] = useState(false);

  return (
    <div className="password-field">
      <input id={id} name={name} type={visible ? "text" : "password"} autoComplete={autoComplete} spellCheck={false} />
      <button
        type="button"
        className="password-toggle"
        aria-label={visible ? "Ocultar contraseña" : "Mostrar contraseña"}
        aria-pressed={visible}
        aria-controls={id}
        onClick={() => setVisible((v) => !v)}
      >
        {visible ? <OjoAbierto /> : <OjoCerrado />}
      </button>
    </div>
  );
}

function OjoAbierto() {
  return (
    <svg aria-hidden="true" data-icon="ojo-abierto" viewBox="0 0 24 24" width="20" height="20" fill="none">
      <path
        d="M2 12s3.6-7 10-7 10 7 10 7-3.6 7-10 7S2 12 2 12Z"
        stroke="currentColor"
        strokeWidth="1.8"
        strokeLinejoin="round"
      />
      <circle cx="12" cy="12" r="3" stroke="currentColor" strokeWidth="1.8" />
    </svg>
  );
}

function OjoCerrado() {
  return (
    <svg aria-hidden="true" data-icon="ojo-cerrado" viewBox="0 0 24 24" width="20" height="20" fill="none">
      <path d="M3 10c2.3 3 5.4 4.5 9 4.5s6.7-1.5 9-4.5" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" />
      <path
        d="m5.5 13.2-1.6 2.3M9.4 14.8l-.7 2.7M14.6 14.8l.7 2.7M18.5 13.2l1.6 2.3"
        stroke="currentColor"
        strokeWidth="1.8"
        strokeLinecap="round"
      />
    </svg>
  );
}
