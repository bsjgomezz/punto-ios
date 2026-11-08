"use client";

import { useRouter } from "next/navigation";
import { useEffect, useState, type FormEvent } from "react";
import { BackLink } from "@/components/back-link";
import { useAuth } from "@/features/auth/auth-provider";
import { SessionBar } from "@/features/auth/session-bar";
import { useMe } from "@/features/auth/use-me";
import { buscarClientes, type Cliente } from "@/features/clientes/api";
import { ApiError } from "@/lib/api/client";
import { listarOrdenes, type OrdenResumen } from "./api";
import { ListaOrdenes } from "./lista-ordenes";
import { NuevaOrdenForm } from "./nueva-orden-form";

type Lista =
  | { pagina: number; kind: "listo"; items: OrdenResumen[]; totalPages: number; total: number }
  | { pagina: number; kind: "sinPermiso" }
  | { pagina: number; kind: "error" };

/** Todas las órdenes y el punto de entrada para recibir un equipo (eligiendo primero al cliente). */
export function ReparacionesPage() {
  const { user } = useAuth();
  const me = useMe();
  const router = useRouter();
  const puedeRecibir = me.kind === "listo" && me.me.permisos.includes("reparaciones.crear");
  const esTecnico = me.kind === "listo" && me.me.rol === "Técnico";
  const perfilCargando = me.kind === "cargando";

  const [pagina, setPagina] = useState(1);
  const [lista, setLista] = useState<Lista | null>(null);
  const [recibiendo, setRecibiendo] = useState(false);
  const [busqueda, setBusqueda] = useState("");
  const [candidatos, setCandidatos] = useState<Cliente[] | null>(null);
  const [cliente, setCliente] = useState<Cliente | null>(null);
  // Filtros de la bandeja: por defecto solo las activas y, si es técnico, solo las suyas.
  const [soloMias, setSoloMias] = useState(true);
  const [soloActivas, setSoloActivas] = useState(true);
  const tecnicoId = esTecnico && soloMias && me.kind === "listo" && me.me.usuarioId ? me.me.usuarioId : undefined;

  useEffect(() => {
    // Se espera al perfil para saber si es técnico antes del primer pedido (y no pedir dos veces).
    if (!user || perfilCargando) return;
    const controller = new AbortController();
    user
      .getIdToken()
      .then((token) => listarOrdenes(token, { page: pagina, tecnicoId, soloActivas }, controller.signal))
      .then((r) => setLista({ pagina, kind: "listo", items: r.items, totalPages: r.totalPages, total: r.totalCount }))
      .catch((error: unknown) => {
        if (controller.signal.aborted) return;
        setLista({ pagina, kind: error instanceof ApiError && error.status === 403 ? "sinPermiso" : "error" });
      });
    return () => controller.abort();
  }, [user, perfilCargando, pagina, tecnicoId, soloActivas]);

  function cambiarSoloMias(valor: boolean) {
    setSoloMias(valor);
    setPagina(1);
  }

  function cambiarSoloActivas(valor: boolean) {
    setSoloActivas(valor);
    setPagina(1);
  }

  async function buscar(evento: FormEvent) {
    evento.preventDefault();
    if (!user || !busqueda.trim()) return;
    const resultado = await buscarClientes(await user.getIdToken(), { q: busqueda, page: 1 });
    setCandidatos(resultado.items);
  }

  function cancelar() {
    setRecibiendo(false);
    setCliente(null);
    setCandidatos(null);
    setBusqueda("");
  }

  return (
    <main className="shell shell-wide">
      <header className="panel-head">
        <p className="brand">
          <span className="brand-mark" aria-hidden="true" />
          Punto iOS
        </p>
        <SessionBar />
      </header>

      <BackLink href="/panel">Volver al panel</BackLink>

      <div className="section-head">
        <h1 className="page-title">Reparaciones</h1>
        {puedeRecibir && !recibiendo && (
          <button type="button" className="btn" onClick={() => setRecibiendo(true)}>
            Recibir equipo
          </button>
        )}
      </div>

      {recibiendo && !cliente && (
        <div className="client-form">
          <h2 className="client-form-title">¿De quién es el equipo?</h2>
          <form className="search" role="search" onSubmit={(e) => void buscar(e)}>
            <label htmlFor="buscar-cliente-orden" className="sr-only">
              Buscar cliente
            </label>
            <input
              id="buscar-cliente-orden"
              type="search"
              value={busqueda}
              onChange={(e) => setBusqueda(e.target.value)}
              placeholder="Nombre, apellido o teléfono"
              autoFocus
            />
            <button type="submit" className="btn btn-quiet">
              Buscar
            </button>
          </form>
          {candidatos?.length === 0 && (
            <p className="list-count">
              No hay clientes con esa búsqueda. Si es nuevo, registralo desde Clientes (podés cargar el equipo ahí mismo).
            </p>
          )}
          {candidatos && candidatos.length > 0 && (
            <ul className="client-list">
              {candidatos.map((c) => (
                <li key={c.id} className="client-row">
                  <button type="button" className="link-button client-name" onClick={() => setCliente(c)}>
                    {c.apellido}, {c.nombre}
                  </button>
                  <span className="client-phone">{c.telefono}</span>
                </li>
              ))}
            </ul>
          )}
          <div className="form-actions">
            <button type="button" className="btn btn-quiet" onClick={cancelar}>
              Cancelar
            </button>
          </div>
        </div>
      )}

      {cliente && (
        <NuevaOrdenForm cliente={cliente} onCreada={(orden) => router.push(`/reparaciones/${orden.id}`)} onCancelar={cancelar} />
      )}

      <div className="chips" role="group" aria-label="Filtros de reparaciones">
        {esTecnico && (
          <label className="chip">
            <input type="checkbox" checked={soloMias} onChange={(e) => cambiarSoloMias(e.target.checked)} />
            <span>Solo mis reparaciones</span>
          </label>
        )}
        <label className="chip">
          <input type="checkbox" checked={soloActivas} onChange={(e) => cambiarSoloActivas(e.target.checked)} />
          <span>Solo activas</span>
        </label>
      </div>

      {lista?.kind === "sinPermiso" && (
        <p className="form-error" role="alert">
          Tu rol no permite ver reparaciones.
        </p>
      )}
      {lista?.kind === "error" && (
        <p className="form-error" role="alert">
          No se pudo cargar la lista. Verificá que el backend esté corriendo e intentá de nuevo.
        </p>
      )}
      {!lista && (
        <p className="gate-message" role="status">
          Cargando reparaciones…
        </p>
      )}

      {lista?.kind === "listo" && (
        <>
          <p className="list-count" role="status">
            {lista.total === 0
              ? "Todavía no se recibió ningún equipo."
              : `${lista.total} ${lista.total === 1 ? "orden" : "órdenes"}`}
          </p>
          <ListaOrdenes ordenes={lista.items} />
          {lista.totalPages > 1 && (
            <nav className="pager" aria-label="Páginas de reparaciones">
              <button type="button" className="btn btn-quiet" disabled={pagina <= 1} onClick={() => setPagina((p) => p - 1)}>
                Anterior
              </button>
              <span>
                Página {pagina} de {lista.totalPages}
              </span>
              <button
                type="button"
                className="btn btn-quiet"
                disabled={pagina >= lista.totalPages}
                onClick={() => setPagina((p) => p + 1)}
              >
                Siguiente
              </button>
            </nav>
          )}
        </>
      )}
    </main>
  );
}
