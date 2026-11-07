import Link from "next/link";
import { formatearFecha, nombreDeEstado, type OrdenResumen } from "./api";

/** Filas de órdenes: número, estado, cliente, equipo, técnico y entrega estimada. */
export function ListaOrdenes({ ordenes, mostrarCliente = true }: { ordenes: OrdenResumen[]; mostrarCliente?: boolean }) {
  return (
    <ul className="client-list">
      {ordenes.map((o) => (
        <li key={o.id} className="orden-row">
          <Link className="orden-row-numero" href={`/reparaciones/${o.id}`} aria-label={`Orden ${o.numero}`}>
            {o.numero}
          </Link>
          <div className="orden-row-cuerpo">
            <p className="client-name">
              {o.equipo}
              {mostrarCliente && <span className="client-meta"> · {o.cliente}</span>}
            </p>
            <p className="client-meta">
              {o.tecnico ?? "Sin técnico"}
              {o.fechaEstimadaEntrega ? ` · entrega estimada ${formatearFecha(o.fechaEstimadaEntrega)}` : ""}
            </p>
          </div>
          <span className="estado-chip" data-estado={o.estado}>
            {nombreDeEstado(o.estado)}
          </span>
        </li>
      ))}
    </ul>
  );
}
