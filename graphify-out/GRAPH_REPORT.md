# Graph Report - Punto iOS  (2026-11-07)

## Corpus Check
- 152 files · ~27,642 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 8 file(s) not represented in the graph (top: (none) 5, .example 1, .ico 1)

## Summary
- 1065 nodes · 2486 edges · 63 communities (45 shown, 18 thin omitted)
- Extraction: 94% EXTRACTED · 6% INFERRED · 0% AMBIGUOUS · INFERRED: 148 edges (avg confidence: 0.83)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `f312fca8`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- Punto iOS — Software Design Document (SDD)
- PuntoIOS.ArchitectureTests.csproj
- 3. Fases
- 0001-decisiones-iniciales.md
- devDependencies
- Fact
- use-remote-checks.ts
- PageRequest
- Instrucciones para Codex / Claude Code — Punto iOS
- Punto iOS
- PostgresReadyCheck
- postgres
- CLAUDE.md
- compilerOptions
- http
- Usuario
- PuntoIosDbContext
- PuntoIOS.Application/AssemblyMarker.cs
- PuntoIOS.Domain/AssemblyMarker.cs
- ServiceCollectionExtensions.cs
- auth-provider.tsx
- UsuarioLocalClaims.AsociarAsync
- Task
- DateTimeOffset
- PuntoIOS.Domain.Identidad
- Resultado
- puntoios_application_common_pagination
- RegistroAuditoria
- ClientesController
- Rol
- useAuth
- FakesClientes.cs
- login-form.tsx
- api.ts
- ServiceCollectionExtensions.AddCasosDeUso
- cliente-detalle.tsx
- LayerDependencyTests
- ClientesEndpointTests
- ClientesEndpointTests.cs
- scripts
- Program.cs
- ObtenerUsuarioActualHandler
- Cliente
- @playwright/test
- react
- CurrentUser
- ClientesPage
- IDirectorioDeUsuarios.ObtenerNombresAsync
- DependencyInjection.AddInfrastructure
- IdentificarUsuarioHandler.HandleAsync
- ActualizarClienteHandlerTests
- TestApi
- package.json
- cliente-detalle.test.tsx
- react
- ActualizarClienteHandler
- HealthEndpointTests
- eslint.config.mjs
- PuntoIOS.Application.Common.Security
- vitest.config.mts

## God Nodes (most connected - your core abstractions)
1. `Cliente` - 40 edges
2. `Usuario` - 39 edges
3. `PuntoIosDbContext` - 32 edges
4. `PuntoIOS.Domain.Identidad` - 29 edges
5. `Rol` - 27 edges
6. `RegistroAuditoria` - 25 edges
7. `Punto iOS — Software Design Document (SDD)` - 19 edges
8. `CurrentUser` - 18 edges
9. `useAuth()` - 17 edges
10. `ActualizarClienteHandlerTests` - 16 edges

## Surprising Connections (you probably didn't know these)
- `Detalle()` --calls--> `ClienteDetalle()`  [EXTRACTED]
  frontend/src/app/clientes/[id]/page.tsx → frontend/src/features/clientes/cliente-detalle.tsx
- `ClientesRoute()` --calls--> `ClientesPage()`  [EXTRACTED]
  frontend/src/app/clientes/page.tsx → frontend/src/features/clientes/clientes-page.tsx
- `PanelPage()` --calls--> `RequireAuth()`  [EXTRACTED]
  frontend/src/app/panel/page.tsx → frontend/src/features/auth/require-auth.tsx
- `ServerIdentity()` --calls--> `apiGet()`  [EXTRACTED]
  frontend/src/features/auth/server-identity.tsx → frontend/src/lib/api/client.ts
- `useMe()` --calls--> `useAuth()`  [EXTRACTED]
  frontend/src/features/auth/use-me.ts → frontend/src/features/auth/auth-provider.tsx

## Import Cycles
- None detected.

## Communities (63 total, 18 thin omitted)

### Community 0 - "Punto iOS — Software Design Document (SDD)"
Cohesion: 0.09
Nodes (21): 10. Notificaciones internas, 11. PWA y offline, 12. API, 13. Seguridad y auditoría, 14. Pruebas, 15. Diseño visual, 16. Observabilidad, 17. Definition of Done por slice (+13 more)

### Community 1 - "PuntoIOS.ArchitectureTests.csproj"
Cohesion: 0.06
Nodes (35): net10.0, Microsoft.EntityFrameworkCore.Design (10.0.14), net10.0, Microsoft.NET.Sdk, net10.0, Microsoft.NET.Sdk, net10.0, Microsoft.EntityFrameworkCore.Design (10.0.14) (+27 more)

### Community 2 - "3. Fases"
Cohesion: 0.10
Nodes (20): 1. Ciclo de cada slice, 2. Hitos de uso real, 3. Fases, 4. Flujos E2E mínimos (Playwright), 5. Definition of Done, 6. Estrategia Git, Fase 10 — Accesorios, repuestos e inventario, Fase 11 — Finanzas → Hito C (+12 more)

### Community 4 - "devDependencies"
Cohesion: 0.17
Nodes (11): devDependencies, jsdom@^30.1.3, @tailwindcss/turbopack@^4, @testing-library/jest-dom@^7.0.1, @types/react-dom@^19, typescript@^5, jsdom, @tailwindcss/turbopack (+3 more)

### Community 5 - "Fact"
Cohesion: 0.11
Nodes (7): RegistrarUsuarioRequest, AutorizacionTests, Endpoints, MeEndpointTests, PostgresFixture, ConnectionString, UsuariosEndpointTests

### Community 6 - "use-remote-checks.ts"
Cohesion: 0.15
Nodes (14): Home(), REMOTE_CHECKS, RemoteCheckId, Row, RowState, StatusBoard(), subscribeToConnection(), useOnline() (+6 more)

### Community 7 - "PageRequest"
Cohesion: 0.08
Nodes (13): PagedResult, TotalPages, PageRequest, Page, PageSize, Skip, CrearClienteCommand, CrearClienteHandlerTests (+5 more)

### Community 8 - "Instrucciones para Codex / Claude Code — Punto iOS"
Cohesion: 0.50
Nodes (3): Instrucciones para Codex / Claude Code — Punto iOS, Primer encargo, Reglas de trabajo

### Community 9 - "Punto iOS"
Cohesion: 0.25
Nodes (7): Cómo ejecutar, Documentación, Estado, Estructura, Pruebas, Punto iOS, Requisitos

### Community 11 - "postgres"
Cohesion: 0.11
Nodes (18): POSTGRES_DB, POSTGRES_PASSWORD, POSTGRES_USER, interval, retries, test, timeout, container_name (+10 more)

### Community 13 - "compilerOptions"
Cohesion: 0.11
Nodes (18): compilerOptions, allowJs, esModuleInterop, incremental, isolatedModules, jsx, lib, module (+10 more)

### Community 14 - "http"
Cohesion: 0.20
Nodes (9): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, profiles, http (+1 more)

### Community 15 - "Usuario"
Cohesion: 0.10
Nodes (17): ListarUsuariosHandler, IUsuarioRepositorio, Usuario, Activo, CreadoEn, Email, FirebaseUid, Id (+9 more)

### Community 16 - "PuntoIosDbContext"
Cohesion: 0.06
Nodes (28): ILocalActual, LocalId, Local, Id, Nombre, LocalesDeSistema, RegistroAuditoriaConfiguration, ClienteConfiguration (+20 more)

### Community 19 - "ServiceCollectionExtensions.cs"
Cohesion: 0.19
Nodes (3): PuntoIOS.Application.Features.Clientes.ActualizarCliente, PuntoIOS.Application.Features.Clientes.CrearCliente, PuntoIOS.Application.Features.Clientes.HistorialCliente

### Community 20 - "auth-provider.tsx"
Cohesion: 0.17
Nodes (13): body, display, metadata, RootLayout(), viewport, AuthContext, AuthContextValue, AuthProvider() (+5 more)

### Community 22 - "UsuarioLocalClaims.AsociarAsync"
Cohesion: 0.14
Nodes (5): FirebaseClaims, HttpContextCurrentUserAccessor, UsuarioLocalClaims, PuntoIOS.Application.Features.Identidad.IdentificarUsuario, PuntoIOS.Api.Authentication

### Community 25 - "DateTimeOffset"
Cohesion: 0.10
Nodes (8): InicialIdentidad, ClientesYLocales, AuditoriaYEdicionClientes, PuntoIosDbContextModelSnapshot, RelojFijo, Ahora, PuntoIOS.Infrastructure.Persistence.Migrations, PuntoIOS.Infrastructure.Persistence

### Community 26 - "PuntoIOS.Domain.Identidad"
Cohesion: 0.14
Nodes (9): Permiso, Codigo, Descripcion, RolesDeSistema, PuntoIOS.Application.Common.Results, PuntoIOS.UnitTests.Identidad, PuntoIOS.Application.Features.Identidad.ActualizarUsuario, PuntoIOS.Application.Features.Identidad.Puertos (+1 more)

### Community 27 - "Resultado"
Cohesion: 0.17
Nodes (10): ResultadoExtensions, Error, Resultado, Error, EsExitoso, Valor, TipoError, Conflicto (+2 more)

### Community 28 - "puntoios_application_common_pagination"
Cohesion: 0.15
Nodes (8): BuscarClientesHandler, ObtenerClienteHandler, PuntoIOS.Application.Features.Clientes.Puertos, PuntoIOS.Application.Features.Clientes.BuscarClientes, PuntoIOS.Application.Features.Identidad.RegistrarUsuario, PuntoIOS.Application.Features.Identidad, PuntoIOS.Application.Features.Identidad.ListarUsuarios, PuntoIOS.Application.Features.Clientes.ObtenerCliente

### Community 29 - "RegistroAuditoria"
Cohesion: 0.13
Nodes (15): AccionesAuditadas, CambioDeCampo, EntidadesAuditadas, RegistroAuditoria, Accion, Cambios, Entidad, EntidadId (+7 more)

### Community 30 - "ClientesController"
Cohesion: 0.18
Nodes (7): ActualizarClienteRequest, ClientesController, RolesController, ActualizarUsuarioRequest, UsuariosController, ActualizarUsuarioHandler, UsuarioDto

### Community 31 - "Rol"
Cohesion: 0.06
Nodes (21): ActualizarUsuarioCommand, ListarRolesHandler, IRolRepositorio, RegistrarUsuarioCommand, RolDto, Rol, Descripcion, EsDeSistema (+13 more)

### Community 32 - "useAuth"
Cohesion: 0.29
Nodes (7): PanelPage(), useAuth(), ServerIdentity(), State, mocks, SessionBar(), UsuarioActual

### Community 33 - "FakesClientes.cs"
Cohesion: 0.18
Nodes (5): PuntoIOS.UnitTests.Clientes, PuntoIOS.Application.Features.Clientes, PuntoIOS.Application.Features.Auditoria.Puertos, PuntoIOS.Domain.Clientes, PuntoIOS.Domain.Auditoria

### Community 34 - "login-form.tsx"
Cohesion: 0.18
Nodes (13): nextConfig, vitest@^5.1.0, IngresarPage(), describeAuthError(), LoginForm(), onGoogle(), onSubmit(), noSubscribe() (+5 more)

### Community 35 - "api.ts"
Cohesion: 0.15
Nodes (18): CambioDeCampo, Cliente, Coincidencia, crearCliente(), DatosCliente, NuevoCliente, Pagina, RegistroHistorial (+10 more)

### Community 37 - "cliente-detalle.tsx"
Cohesion: 0.18
Nodes (15): EstadoMe, useMe(), actualizarCliente(), historialDeCliente(), obtenerCliente(), ClienteDetalle(), Datos, ETIQUETAS (+7 more)

### Community 39 - "ClientesEndpointTests"
Cohesion: 0.28
Nodes (3): CrearClienteRequest, RegistroHistorialDto, ClientesEndpointTests

### Community 40 - "ClientesEndpointTests.cs"
Cohesion: 0.21
Nodes (3): PuntoIOS.Api.Controllers, PuntoIOS.IntegrationTests, PuntoIOS.Application.Features.Identidad.ObtenerUsuarioActual

### Community 41 - "scripts"
Cohesion: 0.25
Nodes (8): scripts, build, dev, lint, start, test, test:e2e, typecheck

### Community 42 - "Program.cs"
Cohesion: 0.17
Nodes (5): DotEnv, AssemblyMarker, PuntoIOS.Api.Configuration, PuntoIOS.Infrastructure, PuntoIOS.Api.Extensions

### Community 43 - "ObtenerUsuarioActualHandler"
Cohesion: 0.29
Nodes (5): MeController, ObtenerUsuarioActualHandler, UsuarioActualDto, FakeAccessor, ObtenerUsuarioActualHandlerTests

### Community 44 - "Cliente"
Cohesion: 0.06
Nodes (23): EmailValidator, ActualizarClienteCommand, ClienteResumenDto, ClienteValidacion, IClienteRepositorio, Cliente, Apellido, CreadoEn (+15 more)

### Community 45 - "@playwright/test"
Cohesion: 0.29
Nodes (3): CORS, @playwright/test@^1.64.1, @playwright/test

### Community 46 - "react"
Cohesion: 0.33
Nodes (6): dependencies, firebase@^13.2.0, next@16.4.5, react@19.3.0, react-dom@19.3.0, react-dom

### Community 49 - "ClientesPage"
Cohesion: 0.15
Nodes (5): buscarClientes(), ClientesPage(), ana, Handler, mocks

### Community 50 - "IDirectorioDeUsuarios.ObtenerNombresAsync"
Cohesion: 0.26
Nodes (4): IDirectorioDeUsuarios, Permisos, DirectorioDeUsuarios, DirectorioFijo

### Community 52 - "IdentificarUsuarioHandler.HandleAsync"
Cohesion: 0.25
Nodes (9): EstadoIdentificacion, Autorizado, Desactivado, NoRegistrado, IdentificarUsuarioHandler, ResultadoIdentificacion, Desactivado, NoRegistrado (+1 more)

### Community 55 - "package.json"
Cohesion: 0.20
Nodes (9): tailwindcss@^4, @types/node@^24.19.2, @types/react@^19, name, private, version, tailwindcss, @types/node (+1 more)

### Community 56 - "cliente-detalle.test.tsx"
Cohesion: 0.20
Nodes (6): @testing-library/react@^16.3.3, ana, Handler, historial, mocks, @testing-library/react

### Community 57 - "react"
Cohesion: 0.36
Nodes (6): ClienteRoute(), Detalle(), ClientesRoute(), RequireAuth(), mocks, react

### Community 58 - "ActualizarClienteHandler"
Cohesion: 0.33
Nodes (6): IAuditoriaRepositorio, ActualizarClienteHandler, CrearClienteHandler, HistorialClienteHandler, LocalFijo, LocalId

### Community 60 - "eslint.config.mjs"
Cohesion: 0.33
Nodes (5): eslintConfig, eslint@^9, eslint-config-next@16.4.5, eslint, eslint-config-next

## Knowledge Gaps
- **109 isolated node(s):** `Microsoft.EntityFrameworkCore.Design (10.0.14)`, `Microsoft.EntityFrameworkCore.Design (10.0.14)`, `coverlet.collector (6.0.4)`, `Microsoft.NET.Test.Sdk (17.14.1)`, `xunit (2.9.3)` (+104 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 353 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **18 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `PuntoIosDbContext` connect `PuntoIosDbContext` to `FakesClientes.cs`, `Cliente`, `Usuario`, `IDirectorioDeUsuarios.ObtenerNombresAsync`, `DependencyInjection.AddInfrastructure`, `Task`, `DateTimeOffset`, `ActualizarClienteHandler`, `RegistroAuditoria`, `Rol`?**
  _High betweenness centrality (0.050) - this node is a cross-community bridge._
- **What connects `Microsoft.EntityFrameworkCore.Design (10.0.14)`, `Microsoft.EntityFrameworkCore.Design (10.0.14)`, `coverlet.collector (6.0.4)` to the rest of the system?**
  _109 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Punto iOS — Software Design Document (SDD)` be split into smaller, more focused modules?**
  _Cohesion score 0.09090909090909091 - nodes in this community are weakly interconnected._
- **Why does `PuntoIOS.Domain.Identidad` connect `PuntoIOS.Domain.Identidad` to `FakesClientes.cs`, `ClientesEndpointTests.cs`, `Usuario`, `PuntoIosDbContext`, `IDirectorioDeUsuarios.ObtenerNombresAsync`, `ServiceCollectionExtensions.cs`, `IdentificarUsuarioHandler.HandleAsync`, `MeEndpointTests.cs`, `DateTimeOffset`, `puntoios_application_common_pagination`, `Rol`?**
  _High betweenness centrality (0.046) - this node is a cross-community bridge._
- **Should `PuntoIOS.ArchitectureTests.csproj` be split into smaller, more focused modules?**
  _Cohesion score 0.059800664451827246 - nodes in this community are weakly interconnected._
- **Why does `Cliente` connect `Cliente` to `FakesClientes.cs`, `PuntoIosDbContext`, `ActualizarClienteHandlerTests`, `Task`, `DateTimeOffset`, `Rol`?**
  _High betweenness centrality (0.034) - this node is a cross-community bridge._
- **Should `3. Fases` be split into smaller, more focused modules?**
  _Cohesion score 0.09523809523809523 - nodes in this community are weakly interconnected._