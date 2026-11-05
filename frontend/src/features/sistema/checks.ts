export const REMOTE_CHECKS = [
  {
    id: "servidor",
    label: "Servidor",
    hint: "Procesa las operaciones del negocio",
    path: "/health",
    okText: "Responde",
    errorText: "No responde",
    help: "Iniciá el servidor (dotnet run en backend/src/PuntoIOS.Api) y revisá la dirección en NEXT_PUBLIC_API_BASE_URL.",
  },
  {
    id: "baseDeDatos",
    label: "Base de datos",
    hint: "Guarda clientes, reparaciones y ventas",
    path: "/health/ready",
    okText: "Conectada",
    errorText: "No disponible",
    help: "El servidor no llega a PostgreSQL. Verificá que Docker esté abierto y ejecutá docker compose up -d.",
  },
] as const;

export type RemoteCheckId = (typeof REMOTE_CHECKS)[number]["id"];
