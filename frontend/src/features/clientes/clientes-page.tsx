"use client";

import Link from "next/link";
import { useEffect, useState, type FormEvent } from "react";
import { useAuth } from "@/features/auth/auth-provider";
import { SessionBar } from "@/features/auth/session-bar";
import { useMe } from "@/features/auth/use-me";
import { ApiError } from "@/lib/api/client";
import { buscarClientes, type Cliente, type Pagina } from "./api";
import { NuevoClienteForm } from "./nuevo-cliente-form";

type Resultado =
  | { clave: string; kind: "listo"; pagina: Pagina<Cliente> }
  | { clave: string; kind: "sinPermiso" }
  | { clave: string; kind: "error" };

export function ClientesPage() {
  const { user } = useAuth();
  const me = useMe();
  const puedeCrear = me.kind === "listo" && me.me.permisos.includes("clientes.crear");

  const [texto, setTexto] = useState("");
  const [consulta, setConsulta] = useState({ q: "", page: 1 });
  const [recarga, setRecarga] = useState(0);
  const [resultado, setResultado] = useState<Resultado | null>(null);
  const [creando, setCreando] = useState(false);
  const [guardado, setGuardado] = useState<string | null>(null);

  const clave = `${consulta.q}|${consulta.page}|${recarga}`;
  const cargando = resultado?.clave !== clave;

  useEffect(() => {
    if (!user) return;
    const controller = new AbortController();

    user
      .getIdToken()
      .then((token) => buscarClientes(token, consulta, controller.signal))
      .then((pagina) => setResultado({ clave, kind: "listo", pagina }))
      .catch((error: unknown) => {
        if (controller.signal.aborted) return;
        setResultado({ clave, kind: error instanceof ApiError && error.status === 403 ? "sinPermiso" : "error" });
      });

    return () => controller.abort();
  }, [user, consulta, clave]);

  function buscar(evento: FormEvent) {
    evento.preventDefault();
    setConsulta({ q: texto, page: 1 });
  }

  function limpiar() {
    setTexto("");
    setConsulta({ q: "", page: 1 });
  }

  function alCrear(cliente: Cliente) {
    setCreando(false);
    setGuardado(`Cliente guardado: ${cliente.apellido}, ${cliente.nombre}.`);
    setRecarga((n) => n + 1);
  }

  const pagina = resultado?.kind === "listo" ? resultado.pagina : null;

  return (
    <main className="shell shell-wide">
      <header className="panel-head">
        <p className="brand">
          <span className="brand-mark" aria-hidden="true" />
          Punto iOS
        </p>
        <SessionBar />
      </header>

      <p className="back-link">
        <Link href="/panel">Volver al panel</Link>
      </p>
      <h1 className="page-title">Clientes</h1>

      <div className="toolbar">
        <form className="search" role="search" onSubmit={buscar}>
          <label htmlFor="busqueda" className="sr-only">
            Buscar clientes
          </label>
          <input
            id="busqueda"
            type="search"
            value={texto}
            onChange={(e) => setTexto(e.target.value)}
            placeholder="Nombre, apellido, email o teléfono"
          />
          <button type="submit" className="btn btn-quiet">
            Buscar
          </button>
          {consulta.q && (
            <button type="button" className="btn btn-quiet" onClick={limpiar}>
              Quitar filtro
            </button>
          )}
        </form>

        {puedeCrear && !creando && (
          <button type="button" className="btn" onClick={() => { setGuardado(null); setCreando(true); }}>
            Nuevo cliente
          </button>
        )}
      </div>

      {guardado && (
        <p className="identity" data-state="ok" role="status">
          {guardado}
        </p>
      )}

      {creando && <NuevoClienteForm onCreado={alCrear} onCancelar={() => setCreando(false)} />}

      {resultado?.kind === "sinPermiso" && (
        <p className="form-error" role="alert">
          Tu rol no permite ver clientes.
        </p>
      )}
      {resultado?.kind === "error" && (
        <p className="form-error" role="alert">
          No se pudo cargar la lista. Verificá que el backend esté corriendo e intentá de nuevo.
        </p>
      )}

      {pagina && (
        <>
          <p className="list-count" role="status">
            {pagina.totalCount === 0
              ? consulta.q
                ? "Ningún cliente coincide con la búsqueda."
                : "Todavía no hay clientes cargados."
              : `${pagina.totalCount} ${pagina.totalCount === 1 ? "cliente" : "clientes"}`}
          </p>

          <ul className="client-list" aria-busy={cargando}>
            {pagina.items.map((c) => (
              <li key={c.id} className="client-row">
                <Link className="client-name" href={`/clientes/${c.id}`}>
                  {c.apellido}, {c.nombre}
                </Link>
                <span className="client-phone">{c.telefono}</span>
                {c.email && <span className="client-meta">{c.email}</span>}
              </li>
            ))}
          </ul>

          {pagina.totalPages > 1 && (
            <nav className="pager" aria-label="Páginas de clientes">
              <button
                type="button"
                className="btn btn-quiet"
                disabled={pagina.page <= 1 || cargando}
                onClick={() => setConsulta((c) => ({ ...c, page: c.page - 1 }))}
              >
                Anterior
              </button>
              <span>
                Página {pagina.page} de {pagina.totalPages}
              </span>
              <button
                type="button"
                className="btn btn-quiet"
                disabled={pagina.page >= pagina.totalPages || cargando}
                onClick={() => setConsulta((c) => ({ ...c, page: c.page + 1 }))}
              >
                Siguiente
              </button>
            </nav>
          )}
        </>
      )}

      {!resultado && (
        <p className="gate-message" role="status">
          Cargando clientes…
        </p>
      )}
    </main>
  );
}
