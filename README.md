# Punto iOS

PWA para gestionar reparaciones de iPhone, venta de celulares, accesorios, inventario y finanzas.
Stack: Next.js + React + TypeScript (frontend), C# ASP.NET Core (backend, arquitectura hexagonal), PostgreSQL, Firebase Authentication.

## Documentación
- `docs/SDD.md` — diseño, dominio, seguridad, API y pruebas.
- `docs/ROADMAP.md` — fases y slices.
- `docs/ADR/` — decisiones de arquitectura.
- `AGENTS.md` — reglas para Codex / Claude Code.

## Estado
**Fase 1 (cimientos).** Hay health check del backend, página inicial que consulta su estado y pruebas base. Sin autenticación ni lógica de negocio todavía.

## Requisitos
.NET SDK 10, Node 24 + npm, Docker Desktop (abierto).

## Cómo ejecutar

```powershell
# 1. Configuración (una sola vez)
copy .env.example .env        # completá POSTGRES_PASSWORD y la contraseña en ConnectionStrings__Default

# 2. Base de datos
docker compose up -d
docker compose ps             # debe decir "healthy"

# 3. Backend  (http://localhost:5080)
cd backend
dotnet run --project src/PuntoIOS.Api
#   http://localhost:5080/health        -> vivo
#   http://localhost:5080/health/ready  -> vivo + PostgreSQL

# 4. Frontend (http://localhost:3000), en otra terminal
cd frontend
npm install
npm run dev
```
El backend lee `.env` de la raíz solo en `Development`. El frontend usa `http://localhost:5080` por defecto; para cambiarlo creá `frontend/.env.local` con `NEXT_PUBLIC_API_BASE_URL=...`.

## Pruebas

```powershell
cd backend
dotnet test                   # unitarias + arquitectura + integración (las de integración necesitan Docker abierto)

cd ../frontend
npm run lint
npm run typecheck
npm test                      # Vitest
npm run test:e2e              # Playwright (levanta el frontend; la API se simula)
```

## Estructura
```
backend/   src/{Api,Application,Domain,Infrastructure}  tests/{Unit,Integration,Architecture}Tests
frontend/  src/{app,features,lib}  e2e/
docs/      SDD.md, ROADMAP.md, ADR/
```
