# Graph Report - Punto iOS  (2026-11-07)

## Corpus Check
- 112 files · ~17,013 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 8 file(s) not represented in the graph (top: (none) 5, .example 1, .ico 1)

## Summary
- 736 nodes · 1496 edges · 34 communities (23 shown, 11 thin omitted)
- Extraction: 94% EXTRACTED · 6% INFERRED · 0% AMBIGUOUS · INFERRED: 93 edges (avg confidence: 0.83)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `0416e1b5`
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
- Rol
- PuntoIOS.Application/AssemblyMarker.cs
- PuntoIOS.Domain/AssemblyMarker.cs
- ServiceCollectionExtensions.cs
- login-form.tsx
- Fact
- MeEndpointTests.cs
- LayerDependencyTests
- 20261107150624_InicialIdentidad.Designer.cs
- PuntoIOS.Domain.Identidad
- Resultado
- UsuariosController.cs
- Program.cs
- PuntoIOS.Infrastructure.Persistence
- IdentificarUsuarioHandler.cs
- HealthEndpointTests
- Permisos.cs

## God Nodes (most connected - your core abstractions)
1. `Usuario` - 42 edges
2. `Rol` - 30 edges
3. `PuntoIOS.Domain.Identidad` - 27 edges
4. `PuntoIosDbContext` - 20 edges
5. `Punto iOS — Software Design Document (SDD)` - 19 edges
6. `CurrentUser` - 18 edges
7. `compilerOptions` - 16 edges
8. `IUsuarioRepositorio` - 15 edges
9. `UsuarioRepositorioEnMemoria` - 15 edges
10. `UsuarioRepositorio` - 14 edges

## Surprising Connections (you probably didn't know these)
- `TestApi` --references--> `Program`  [EXTRACTED]
  backend/tests/PuntoIOS.IntegrationTests/TestApi.cs → backend/src/PuntoIOS.Api/Program.cs
- `MeController` --references--> `ObtenerUsuarioActualHandler`  [EXTRACTED]
  backend/src/PuntoIOS.Api/Controllers/MeController.cs → backend/src/PuntoIOS.Application/Features/Identidad/ObtenerUsuarioActual/ObtenerUsuarioActualHandler.cs
- `Usuario` --references--> `Rol`  [EXTRACTED]
  backend/src/PuntoIOS.Domain/Identidad/Usuario.cs → backend/src/PuntoIOS.Domain/Identidad/Rol.cs
- `GestionDeUsuariosTests` --references--> `Rol`  [EXTRACTED]
  backend/tests/PuntoIOS.UnitTests/Identidad/GestionDeUsuariosTests.cs → backend/src/PuntoIOS.Domain/Identidad/Rol.cs
- `UsuarioConfiguration` --references--> `Usuario`  [EXTRACTED]
  backend/src/PuntoIOS.Infrastructure/Persistence/Configurations/IdentidadConfiguraciones.cs → backend/src/PuntoIOS.Domain/Identidad/Usuario.cs

## Import Cycles
- None detected.

## Communities (34 total, 11 thin omitted)

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
Cohesion: 0.05
Nodes (42): CORS, eslintConfig, react-dom@19.3.0, devDependencies, eslint@^9, eslint-config-next@16.4.5, jsdom@^30.1.3, @playwright/test@^1.64.1 (+34 more)

### Community 5 - "TestApi.CrearAsync"
Cohesion: 0.08
Nodes (8): RegistrarUsuarioRequest, AutorizacionTests, Endpoints, MeEndpointTests, PostgresFixture, ConnectionString, TestApi, UsuariosEndpointTests

### Community 6 - "use-remote-checks.ts"
Cohesion: 0.15
Nodes (14): Home(), REMOTE_CHECKS, RemoteCheckId, Row, RowState, StatusBoard(), subscribeToConnection(), useOnline() (+6 more)

### Community 7 - "PageRequest"
Cohesion: 0.13
Nodes (10): PagedResult, TotalPages, PageRequest, Page, PageSize, Skip, PagedResultTests, PageRequestTests (+2 more)

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
Cohesion: 0.06
Nodes (34): MeController, RolesController, ActualizarUsuarioRequest, UsuariosController, ServiceCollectionExtensions, ActualizarUsuarioHandler, IdentificarUsuarioHandler, UsuarioAutorizado (+26 more)

### Community 16 - "Rol"
Cohesion: 0.06
Nodes (23): Permiso, Codigo, Descripcion, Rol, Descripcion, EsDeSistema, Id, Nombre (+15 more)

### Community 19 - "ServiceCollectionExtensions.cs"
Cohesion: 0.11
Nodes (9): FirebaseClaims, UsuarioLocalClaims, PuntoIOS.Application.Features.Identidad.IdentificarUsuario, PuntoIOS.Application.Common.Security, PuntoIOS.Api.Authentication, PuntoIOS.Application.Features.Identidad.ObtenerUsuarioActual, PuntoIOS.Application.Features.Identidad.ListarUsuarios, PuntoIOS.Application.Features.Identidad.ListarRoles (+1 more)

### Community 20 - "login-form.tsx"
Cohesion: 0.06
Nodes (42): nextConfig, dependencies, firebase@^13.2.0, next@16.4.5, react@19.3.0, @testing-library/react@^16.3.3, IngresarPage(), body (+34 more)

### Community 22 - "Fact"
Cohesion: 0.08
Nodes (14): HttpContextCurrentUserAccessor, CurrentUser, ICurrentUserAccessor, ActualizarUsuarioCommand, ObtenerUsuarioActualHandler, RegistrarUsuarioCommand, RelojFijo, Ahora (+6 more)

### Community 25 - "20261107150624_InicialIdentidad.Designer.cs"
Cohesion: 0.15
Nodes (3): InicialIdentidad, PuntoIosDbContextModelSnapshot, PuntoIOS.Infrastructure.Persistence.Migrations

### Community 26 - "PuntoIOS.Domain.Identidad"
Cohesion: 0.22
Nodes (5): RolesDeSistema, PuntoIOS.UnitTests.Identidad, PuntoIOS.Application.Features.Identidad.ActualizarUsuario, PuntoIOS.Application.Features.Identidad.Puertos, PuntoIOS.Domain.Identidad

### Community 27 - "Resultado"
Cohesion: 0.16
Nodes (10): ResultadoExtensions, Error, Resultado, Error, EsExitoso, Valor, TipoError, Conflicto (+2 more)

### Community 28 - "UsuariosController.cs"
Cohesion: 0.24
Nodes (4): PuntoIOS.Application.Common.Results, PuntoIOS.Api.Controllers, PuntoIOS.Application.Features.Identidad.RegistrarUsuario, PuntoIOS.Application.Features.Identidad

### Community 29 - "Program.cs"
Cohesion: 0.17
Nodes (5): DotEnv, Program, AssemblyMarker, PuntoIOS.Api.Configuration, PuntoIOS.Infrastructure

### Community 31 - "IdentificarUsuarioHandler.cs"
Cohesion: 0.29
Nodes (7): EstadoIdentificacion, Autorizado, Desactivado, NoRegistrado, ResultadoIdentificacion, Desactivado, NoRegistrado

## Knowledge Gaps
- **78 isolated node(s):** `net10.0`, `Microsoft.AspNetCore.Authentication.JwtBearer (10.0.14)`, `Microsoft.AspNetCore.OpenApi (10.0.13)`, `Microsoft.EntityFrameworkCore.Design (10.0.14)`, `Microsoft.NET.Sdk.Web` (+73 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 265 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **11 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `PuntoIOS.Domain.Identidad` connect `PuntoIOS.Domain.Identidad` to `Permisos.cs`, `Task`, `Rol`, `ServiceCollectionExtensions.cs`, `MeEndpointTests.cs`, `UsuariosController.cs`, `PuntoIOS.Infrastructure.Persistence`, `IdentificarUsuarioHandler.cs`?**
  _High betweenness centrality (0.050) - this node is a cross-community bridge._
- **What connects `net10.0`, `Microsoft.AspNetCore.Authentication.JwtBearer (10.0.14)`, `Microsoft.AspNetCore.OpenApi (10.0.13)` to the rest of the system?**
  _78 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Punto iOS — Software Design Document (SDD)` be split into smaller, more focused modules?**
  _Cohesion score 0.09090909090909091 - nodes in this community are weakly interconnected._
- **Why does `Usuario` connect `Task` to `Rol`, `Fact`?**
  _High betweenness centrality (0.037) - this node is a cross-community bridge._
- **Should `PuntoIOS.ArchitectureTests.csproj` be split into smaller, more focused modules?**
  _Cohesion score 0.059800664451827246 - nodes in this community are weakly interconnected._
- **Why does `PuntoIosDbContext` connect `Rol` to `20261107150624_InicialIdentidad.Designer.cs`, `TestApi.CrearAsync`, `PuntoIOS.Infrastructure.Persistence`, `Task`?**
  _High betweenness centrality (0.030) - this node is a cross-community bridge._
- **Should `3. Fases` be split into smaller, more focused modules?**
  _Cohesion score 0.09523809523809523 - nodes in this community are weakly interconnected._