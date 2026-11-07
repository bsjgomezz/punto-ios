import Link from "next/link";
import type { ReactNode } from "react";

/** Botón para volver a la pantalla anterior, con flecha y fondo propio para que se note que se puede tocar. */
export function BackLink({ href, children }: { href: string; children: ReactNode }) {
  return (
    <p className="back-link">
      <Link href={href} className="back-button">
        <svg aria-hidden="true" viewBox="0 0 20 20" width="16" height="16" fill="none">
          <path d="M12.5 4.5 7 10l5.5 5.5" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
        </svg>
        {children}
      </Link>
    </p>
  );
}
