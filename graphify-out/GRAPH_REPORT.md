# Graph Report - Punto iOS  (2026-11-07)

## Corpus Check
- 148 files · ~26,120 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 8 file(s) not represented in the graph (top: (none) 5, .example 1, .ico 1)

## Summary
- 1001 nodes · 2324 edges · 48 communities (33 shown, 15 thin omitted)
- Extraction: 94% EXTRACTED · 6% INFERRED · 0% AMBIGUOUS · INFERRED: 148 edges (avg confidence: 0.83)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `6cec5a6a`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- Punto iOS — Software Design Document (SDD)
- PuntoIOS.ArchitectureTests.csproj
- 3. Fases
- 0001-decisiones-iniciales.md
- package.json
- TestApi.CrearAsync
- use-remote-checks.ts
- PageRequest
- Instrucciones para Codex / Claude Code — Punto iOS
- Punto iOS
- PostgresReadyCheck
- postgres
- CLAUDE.md
- compilerOptions
- http
- Task
- PuntoIosDbContext
- PuntoIOS.Application/AssemblyMarker.cs
- PuntoIOS.Domain/AssemblyMarker.cs
- ServiceCollectionExtensions.cs
- auth-provider.tsx
- CurrentUser
- MeEndpointTests.cs
- PuntoIOS.Domain.Clientes
- DateTimeOffset
- PuntoIOS.Domain.Identidad
- Resultado
- ServiceCollectionExtensions.cs
- RegistroAuditoria
- LocalUnico
- Rol
- server-identity.tsx
- FakesClientes.cs
- login-form.tsx
- microsoft_entityframeworkcore
- DependencyInjection.AddInfrastructure
- Local
- LayerDependencyTests
- microsoft_entityframeworkcore
- PermisoConfiguration
- scripts
- RegistroAuditoria.cs
- RolConfiguration
- Cliente
- @playwright/test
- react

## God Nodes (most connected - your core abstractions)
1. `Cliente` - 40 edges
2. `Usuario` - 39 edges
3. `PuntoIosDbContext` - 32 edges
4. `PuntoIOS.Domain.Identidad` - 29 edges
5. `Rol` - 27 edges
6. `RegistroAuditoria` - 25 edges
7. `Punto iOS — Software Design Document (SDD)` - 19 edges
8. `CurrentUser` - 18 edges
9. `ActualizarClienteHandlerTests` - 16 edges
10. `compilerOptions` - 16 edges

## Surprising Connections (you probably didn't know these)
- `ClientesController` --references--> `ActualizarClienteHandler`  [EXTRACTED]
  backend/src/PuntoIOS.Api/Controllers/ClientesController.cs → backend/src/PuntoIOS.Application/Features/Clientes/ActualizarCliente/ActualizarClienteHandler.cs
- `ClientesController` --references--> `CrearClienteHandler`  [EXTRACTED]
  backend/src/PuntoIOS.Api/Controllers/ClientesController.cs → backend/src/PuntoIOS.Application/Features/Clientes/CrearCliente/CrearClienteHandler.cs
- `ClientesController` --references--> `HistorialClienteHandler`  [EXTRACTED]
  backend/src/PuntoIOS.Api/Controllers/ClientesController.cs → backend/src/PuntoIOS.Application/Features/Clientes/HistorialCliente/HistorialClienteHandler.cs
- `AuditoriaRepositorio` --implements--> `IAuditoriaRepositorio`  [EXTRACTED]
  backend/src/PuntoIOS.Infrastructure/Persistence/AuditoriaRepositorio.cs → backend/src/PuntoIOS.Application/Features/Auditoria/Puertos/IAuditoriaRepositorio.cs
- `AuditoriaRepoEnMemoria` --implements--> `IAuditoriaRepositorio`  [EXTRACTED]
  backend/tests/PuntoIOS.UnitTests/Clientes/FakesClientes.cs → backend/src/PuntoIOS.Application/Features/Auditoria/Puertos/IAuditoriaRepositorio.cs

## Import Cycles
- None detected.

## Communities (48 total, 15 thin omitted)

### Community 0 - "Punto iOS — Software Design Document (SDD)"
Cohesion: 0.09
Nodes (21): 10. Notificaciones internas, 11. PWA y offline, 12. API, 13. Seguridad y auditoría, 14. Pruebas, 15. Diseño visual, 16. Observabilidad, 17. Definition of Done por slice (+13 more)

### Community 1 - "PuntoIOS.ArchitectureTests.csproj"
Cohesion: 0.06
Nodes (35): net10.0, Microsoft.EntityFrameworkCore.Design (10.0.14), net10.0, Microsoft.NET.Sdk, net10.0, Microsoft.NET.Sdk, net10.0, Microsoft.EntityFrameworkCore.Design (10.0.14) (+27 more)

### Community 2 - "3. Fases"
Cohesion: 0.10
Nodes (20): 1. Ciclo de cada slice, 2. Hitos de uso real, 3. Fases, 4. Flujos E2E mínimos (Playwright), 5. Definition of Done, 6. Estrategia Git, Fase 10 — Accesorios, repuestos e inventario, Fase 11 — Finanzas → Hito C (+12 more)

### Community 4 - "package.json"
Cohesion: 0.08
Nodes (29): eslintConfig, devDependencies, eslint@^9, eslint-config-next@16.4.5, jsdom@^30.1.3, tailwindcss@^4, @tailwindcss/turbopack@^4, @testing-library/jest-dom@^7.0.1 (+21 more)

### Community 5 - "TestApi.CrearAsync"
Cohesion: 0.06
Nodes (16): CrearClienteRequest, RegistrarUsuarioRequest, Program, ActualizarUsuarioCommand, RegistrarUsuarioCommand, AutorizacionTests, Endpoints, ClientesEndpointTests (+8 more)

### Community 6 - "use-remote-checks.ts"
Cohesion: 0.14
Nodes (15): Home(), REMOTE_CHECKS, RemoteCheckId, Row, RowState, StatusBoard(), subscribeToConnection(), useOnline() (+7 more)

### Community 7 - "PageRequest"
Cohesion: 0.11
Nodes (11): PagedResult, TotalPages, PageRequest, Page, PageSize, Skip, PagedResultTests, PageRequestTests (+3 more)

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

### Community 15 - "Task"
Cohesion: 0.05
Nodes (33): ClientesController, MeController, RolesController, ActualizarUsuarioRequest, UsuariosController, ActualizarClienteCommand, ClienteDto, CrearClienteCommand (+25 more)

### Community 16 - "PuntoIosDbContext"
Cohesion: 0.13
Nodes (10): DesignTimeDbContextFactory, PuntoIosDbContext, Auditoria, Clientes, LocalActualId, Locales, Permisos, Roles (+2 more)

### Community 19 - "ServiceCollectionExtensions.cs"
Cohesion: 0.14
Nodes (6): ActualizarClienteRequest, PuntoIOS.UnitTests.Clientes, PuntoIOS.Application.Features.Clientes.ActualizarCliente, PuntoIOS.Application.Features.Clientes.CrearCliente, PuntoIOS.Application.Features.Clientes, PuntoIOS.Application.Features.Clientes.HistorialCliente

### Community 20 - "auth-provider.tsx"
Cohesion: 0.16
Nodes (14): firebase@^13.2.0, body, display, metadata, RootLayout(), viewport, AuthContext, AuthContextValue (+6 more)

### Community 22 - "CurrentUser"
Cohesion: 0.05
Nodes (27): FirebaseClaims, UsuarioLocalClaims, DotEnv, CurrentUser, ICurrentUserAccessor, EstadoIdentificacion, Autorizado, Desactivado (+19 more)

### Community 24 - "PuntoIOS.Domain.Clientes"
Cohesion: 0.20
Nodes (4): BuscarClientesHandler, PuntoIOS.Application.Features.Clientes.Puertos, PuntoIOS.Application.Features.Clientes.BuscarClientes, PuntoIOS.Domain.Clientes

### Community 25 - "DateTimeOffset"
Cohesion: 0.13
Nodes (6): InicialIdentidad, ClientesYLocales, AuditoriaYEdicionClientes, PuntoIosDbContextModelSnapshot, RelojFijo, Ahora

### Community 26 - "PuntoIOS.Domain.Identidad"
Cohesion: 0.19
Nodes (6): Permiso, Codigo, Descripcion, RolesDeSistema, PuntoIOS.Application.Features.Identidad.Puertos, PuntoIOS.Domain.Identidad

### Community 27 - "Resultado"
Cohesion: 0.17
Nodes (10): ResultadoExtensions, Error, Resultado, Error, EsExitoso, Valor, TipoError, Conflicto (+2 more)

### Community 28 - "ServiceCollectionExtensions.cs"
Cohesion: 0.17
Nodes (7): PuntoIOS.Application.Common.Results, PuntoIOS.Api.Controllers, PuntoIOS.Application.Features.Identidad.ActualizarUsuario, PuntoIOS.Application.Features.Identidad.RegistrarUsuario, PuntoIOS.Application.Features.Identidad, PuntoIOS.Application.Features.Identidad.ListarUsuarios, PuntoIOS.Application.Features.Clientes.ObtenerCliente

### Community 29 - "RegistroAuditoria"
Cohesion: 0.14
Nodes (13): RegistroAuditoria, Accion, Cambios, Entidad, EntidadId, Fecha, Id, LocalId (+5 more)

### Community 30 - "LocalUnico"
Cohesion: 0.22
Nodes (7): ILocalActual, LocalId, LocalesDeSistema, LocalUnico, LocalId, PuntoIOS.Application.Common.Multilocal, PuntoIOS.Domain.Locales

### Community 31 - "Rol"
Cohesion: 0.08
Nodes (17): ListarRolesHandler, IRolRepositorio, RolDto, Rol, Descripcion, EsDeSistema, Id, Nombre (+9 more)

### Community 32 - "server-identity.tsx"
Cohesion: 0.17
Nodes (13): @testing-library/react@^16.3.3, PanelPage(), useAuth(), RequireAuth(), mocks, ServerIdentity(), State, mocks (+5 more)

### Community 34 - "login-form.tsx"
Cohesion: 0.25
Nodes (9): IngresarPage(), describeAuthError(), LoginForm(), onGoogle(), onSubmit(), noSubscribe(), mocks, readRememberedEmail() (+1 more)

### Community 36 - "DependencyInjection.AddInfrastructure"
Cohesion: 0.06
Nodes (16): HttpContextCurrentUserAccessor, ServiceCollectionExtensions, IAuditoriaRepositorio, ActualizarClienteHandler, CrearClienteHandler, HistorialClienteHandler, IDirectorioDeUsuarios, RegistrarUsuarioHandler (+8 more)

### Community 37 - "Local"
Cohesion: 0.24
Nodes (6): Local, Id, Nombre, ClienteConfiguration, LocalConfiguration, UsuarioConfiguration

### Community 41 - "scripts"
Cohesion: 0.25
Nodes (8): scripts, build, dev, lint, start, test, test:e2e, typecheck

### Community 44 - "Cliente"
Cohesion: 0.07
Nodes (20): EmailValidator, ClienteResumenDto, ClienteValidacion, Cliente, Apellido, CreadoEn, CreadoPorUsuarioId, Direccion (+12 more)

### Community 45 - "@playwright/test"
Cohesion: 0.29
Nodes (3): CORS, @playwright/test@^1.64.1, @playwright/test

### Community 46 - "react"
Cohesion: 0.25
Nodes (7): nextConfig, dependencies, next@16.4.5, react@19.3.0, react-dom@19.3.0, next, react-dom

## Knowledge Gaps
- **109 isolated node(s):** `Id`, `LocalId`, `Entidad`, `EntidadId`, `Accion` (+104 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 324 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **15 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `PuntoIosDbContext` connect `PuntoIosDbContext` to `FakesClientes.cs`, `DependencyInjection.AddInfrastructure`, `Local`, `TestApi.CrearAsync`, `PermisoConfiguration`, `RolConfiguration`, `Cliente`, `Task`, `CurrentUser`, `DateTimeOffset`, `RegistroAuditoria`, `Rol`?**
  _High betweenness centrality (0.053) - this node is a cross-community bridge._
- **What connects `Id`, `LocalId`, `Entidad` to the rest of the system?**
  _109 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Punto iOS — Software Design Document (SDD)` be split into smaller, more focused modules?**
  _Cohesion score 0.09090909090909091 - nodes in this community are weakly interconnected._
- **Why does `Cliente` connect `Cliente` to `DependencyInjection.AddInfrastructure`, `Local`, `Task`, `PuntoIosDbContext`, `PuntoIOS.Domain.Clientes`, `DateTimeOffset`?**
  _High betweenness centrality (0.046) - this node is a cross-community bridge._
- **Should `PuntoIOS.ArchitectureTests.csproj` be split into smaller, more focused modules?**
  _Cohesion score 0.059800664451827246 - nodes in this community are weakly interconnected._
- **Why does `PuntoIOS.Domain.Identidad` connect `PuntoIOS.Domain.Identidad` to `FakesClientes.cs`, `DependencyInjection.AddInfrastructure`, `microsoft_entityframeworkcore`, `Task`, `ServiceCollectionExtensions.cs`, `CurrentUser`, `MeEndpointTests.cs`, `ServiceCollectionExtensions.cs`, `Rol`?**
  _High betweenness centrality (0.046) - this node is a cross-community bridge._
- **Should `3. Fases` be split into smaller, more focused modules?**
  _Cohesion score 0.09523809523809523 - nodes in this community are weakly interconnected._