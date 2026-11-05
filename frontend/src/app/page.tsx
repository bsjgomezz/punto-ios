import Link from "next/link";
import { StatusBoard } from "@/features/sistema/status-board";

export default function Home() {
  return (
    <main className="shell">
      <p className="brand">
        <span className="brand-mark" aria-hidden="true" />
        Taller y local de iPhone
      </p>
      <h1 className="hero-title">Punto iOS</h1>
      <p className="hero-lead">
        Reparaciones, ventas, stock y caja del local, en un solo lugar.
      </p>
      <p className="hero-actions">
        <Link className="btn" href="/ingresar">
          Ingresar
        </Link>
      </p>

      <StatusBoard />

      <p className="footnote">
        Versión de desarrollo. Esta pantalla confirma que la base del sistema
        funciona antes de sumar clientes y reparaciones.
      </p>
    </main>
  );
}
