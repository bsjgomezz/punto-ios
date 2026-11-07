import type { Metadata } from "next";

export const metadata: Metadata = {
  title: "Seguimiento · Punto iOS",
  robots: { index: false, follow: false },
};

/**
 * Destino del link que reciben los clientes. Por ahora es provisoria: la consulta del estado de las
 * reparaciones llega con el portal público (no lee el código todavía, así que no expone nada).
 */
export default function SeguimientoPage() {
  return (
    <main className="shell shell-narrow">
      <p className="brand">
        <span className="brand-mark" aria-hidden="true" />
        Punto iOS
      </p>
      <h1 className="page-title">Seguimiento de tus equipos</h1>
      <p className="hero-lead">
        Guardá este link: muy pronto vas a poder ver acá el estado de tus reparaciones, sin necesidad de crear una cuenta.
      </p>
      <p className="hero-lead">Mientras tanto, cualquier consulta la respondemos por WhatsApp.</p>
    </main>
  );
}
