"use client";

import { crearCliente, type Cliente } from "./api";
import { ClienteForm } from "./cliente-form";

type Props = { onCreado: (cliente: Cliente) => void; onCancelar: () => void };

/** Alta de cliente. */
export function NuevoClienteForm({ onCreado, onCancelar }: Props) {
  return (
    <ClienteForm
      titulo="Nuevo cliente"
      etiqueta="Nuevo cliente"
      etiquetaGuardar="Guardar cliente"
      mensajeSinPermiso="Tu rol no permite registrar clientes."
      conEquipo
      guardar={crearCliente}
      onGuardado={onCreado}
      onCancelar={onCancelar}
    />
  );
}
