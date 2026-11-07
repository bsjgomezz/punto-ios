# Graph Report - Punto iOS  (2026-11-07)

## Corpus Check
- 179 files · ~36,663 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 8 file(s) not represented in the graph (top: (none) 5, .example 1, .ico 1)

## Summary
- 1260 nodes · 3279 edges · 63 communities (46 shown, 17 thin omitted)
- Extraction: 93% EXTRACTED · 7% INFERRED · 0% AMBIGUOUS · INFERRED: 217 edges (avg confidence: 0.83)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `aa415f34`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- Punto iOS — Software Design Document (SDD)
- PuntoIOS.ArchitectureTests.csproj
- 3. Fases
- 0001-decisiones-iniciales.md
- package.json
- Fact
- use-remote-checks.ts
- DispositivoHandlersTests
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
- TestApi.cs
- Task
- DateTimeOffset
- PuntoIOS.Domain.Identidad
- Resultado
- puntoios_application_common_pagination
- RegistroAuditoria
- ClientesController
- Rol
- clientes-page.tsx
- FakesClientes.cs
- login-form.tsx
- api.ts
- ServiceCollectionExtensions.AddCasosDeUso
- cliente-detalle.tsx
- dispositivo-form.tsx
- CancellationToken
- Dispositivo
- LocalUnico
- Program.cs
- ObtenerUsuarioActualHandler
- Cliente
- ClientesConfiguraciones.cs
- Theory
- CurrentUser
- InlineData
- IDirectorioDeUsuarios.ObtenerNombresAsync
- DependencyInjection.AddInfrastructure
- IdentificarUsuarioHandler.HandleAsync
- PagedResult
- DependencyInjection.cs
- Cliente.Registrar
- cliente-detalle.test.tsx
- ClienteValidacion.Validar
- CrearClienteHandler.HandleAsync
- ClienteRepoEnMemoria
- IdentidadConfiguraciones.cs
- PermisoConfiguration
- UsuarioConfiguration

## God Nodes (most connected - your core abstractions)
1. `Dispositivo` - 42 edges
2. `Usuario` - 39 edges
3. `PuntoIosDbContext` - 35 edges
4. `PuntoIOS.Domain.Identidad` - 31 edges
5. `Cliente` - 31 edges
6. `Rol` - 27 edges
7. `RegistroAuditoria` - 26 edges
8. `PuntoIOS.Domain.Clientes` - 21 edges
9. `useAuth()` - 21 edges
10. `PuntoIOS.Domain.Auditoria` - 20 edges

## Surprising Connections (you probably didn't know these)
- `TestApi` --references--> `Program`  [EXTRACTED]
  backend/tests/PuntoIOS.IntegrationTests/TestApi.cs → backend/src/PuntoIOS.Api/Program.cs
- `ClientesController` --references--> `CrearClienteHandler`  [EXTRACTED]
  backend/src/PuntoIOS.Api/Controllers/ClientesController.cs → backend/src/PuntoIOS.Application/Features/Clientes/CrearCliente/CrearClienteHandler.cs
- `ClientesController` --references--> `HistorialClienteHandler`  [EXTRACTED]
  backend/src/PuntoIOS.Api/Controllers/ClientesController.cs → backend/src/PuntoIOS.Application/Features/Clientes/HistorialCliente/HistorialClienteHandler.cs
- `RegistroHistorialDto` --references--> `CambioDeCampo`  [EXTRACTED]
  backend/src/PuntoIOS.Application/Features/Auditoria/HistorialDeAuditoria.cs → backend/src/PuntoIOS.Domain/Auditoria/RegistroAuditoria.cs
- `DispositivoRepositorio` --implements--> `IDispositivoRepositorio`  [EXTRACTED]
  backend/src/PuntoIOS.Infrastructure/Persistence/DispositivoRepositorio.cs → backend/src/PuntoIOS.Application/Features/Dispositivos/Puertos/IDispositivoRepositorio.cs

## Import Cycles
- None detected.

## Communities (63 total, 17 thin omitted)

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
Nodes (40): CORS, eslintConfig, react-dom@19.3.0, devDependencies, eslint@^9, eslint-config-next@16.4.5, jsdom@^30.1.3, @playwright/test@^1.64.1 (+32 more)

### Community 5 - "Fact"
Cohesion: 0.05
Nodes (17): ActualizarClienteRequest, CrearClienteRequest, EquipoNuevoRequest, ActualizarDispositivoRequest, CrearDispositivoRequest, RegistrarUsuarioRequest, RegistrarUsuarioCommand, LayerDependencyTests (+9 more)

### Community 6 - "use-remote-checks.ts"
Cohesion: 0.15
Nodes (14): Home(), REMOTE_CHECKS, RemoteCheckId, Row, RowState, StatusBoard(), subscribeToConnection(), useOnline() (+6 more)

### Community 7 - "DispositivoHandlersTests"
Cohesion: 0.13
Nodes (11): CrearClienteCommand, EquipoNuevo, ActualizarDispositivoCommand, CrearDispositivoCommand, CrearClienteHandlerTests, DispositivoHandlersTests, LocalFijo, LocalId (+3 more)

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
Cohesion: 0.09
Nodes (19): UsuariosController, ActualizarUsuarioHandler, ListarUsuariosHandler, IUsuarioRepositorio, Usuario, Activo, CreadoEn, Email (+11 more)

### Community 16 - "PuntoIosDbContext"
Cohesion: 0.13
Nodes (11): DesignTimeDbContextFactory, PuntoIosDbContext, Auditoria, Clientes, Dispositivos, LocalActualId, Locales, Permisos (+3 more)

### Community 20 - "auth-provider.tsx"
Cohesion: 0.13
Nodes (17): dependencies, firebase@^13.2.0, next@16.4.5, react@19.3.0, body, display, metadata, RootLayout() (+9 more)

### Community 22 - "UsuarioLocalClaims.AsociarAsync"
Cohesion: 0.14
Nodes (6): FirebaseClaims, HttpContextCurrentUserAccessor, UsuarioLocalClaims, ICurrentUserAccessor, PuntoIOS.Application.Common.Security, PuntoIOS.Api.Authentication

### Community 25 - "DateTimeOffset"
Cohesion: 0.10
Nodes (7): InicialIdentidad, ClientesYLocales, AuditoriaYEdicionClientes, DispositivosDeClientes, PuntoIosDbContextModelSnapshot, PuntoIOS.Infrastructure.Persistence.Migrations, PuntoIOS.Infrastructure.Persistence

### Community 26 - "PuntoIOS.Domain.Identidad"
Cohesion: 0.11
Nodes (12): Permiso, Codigo, Descripcion, Permisos, RolesDeSistema, PuntoIOS.Application.Common.Results, PuntoIOS.Api.Controllers, PuntoIOS.UnitTests.Identidad (+4 more)

### Community 27 - "Resultado"
Cohesion: 0.17
Nodes (10): ResultadoExtensions, Error, Resultado, Error, EsExitoso, Valor, TipoError, Conflicto (+2 more)

### Community 28 - "puntoios_application_common_pagination"
Cohesion: 0.12
Nodes (11): BuscarClientesHandler, ListarRolesHandler, IDirectorioDeUsuarios, DirectorioDeUsuarios, PuntoIOS.Application.Features.Clientes.Puertos, PuntoIOS.Application.Features.Clientes.BuscarClientes, PuntoIOS.Application.Features.Identidad.Puertos, PuntoIOS.Application.Features.Identidad.ListarUsuarios (+3 more)

### Community 29 - "RegistroAuditoria"
Cohesion: 0.14
Nodes (15): AccionesAuditadas, CambioDeCampo, EntidadesAuditadas, RegistroAuditoria, Accion, Cambios, Entidad, EntidadId (+7 more)

### Community 30 - "ClientesController"
Cohesion: 0.21
Nodes (4): ClientesController, ActualizarUsuarioRequest, ActualizarClienteCommand, UsuarioDto

### Community 31 - "Rol"
Cohesion: 0.09
Nodes (14): ActualizarUsuarioCommand, Rol, Descripcion, EsDeSistema, Id, Nombre, Permisos, RolPermiso (+6 more)

### Community 32 - "clientes-page.tsx"
Cohesion: 0.18
Nodes (12): nextConfig, ClientesRoute(), PanelPage(), useAuth(), RequireAuth(), mocks, ServerIdentity(), SessionBar() (+4 more)

### Community 33 - "FakesClientes.cs"
Cohesion: 0.14
Nodes (11): PuntoIOS.UnitTests.Clientes, PuntoIOS.Application.Features.Clientes.ActualizarCliente, PuntoIOS.Application.Features.Dispositivos.CrearDispositivo, PuntoIOS.Application.Features.Dispositivos, PuntoIOS.Application.Features.Dispositivos.ActualizarDispositivo, PuntoIOS.Application.Features.Dispositivos.ListarDispositivos, PuntoIOS.Application.Features.Dispositivos.RevelarImei, PuntoIOS.Application.Features.Clientes.CrearCliente (+3 more)

### Community 34 - "login-form.tsx"
Cohesion: 0.14
Nodes (16): vitest@^5.1.0, IngresarPage(), BackLink(), OjoAbierto(), OjoCerrado(), PasswordInput(), Props, describeAuthError() (+8 more)

### Community 35 - "api.ts"
Cohesion: 0.15
Nodes (17): CambioDeCampo, Cliente, crearCliente(), DatosCliente, EquipoNuevo, NuevoCliente, Pagina, RegistroHistorial (+9 more)

### Community 36 - "ServiceCollectionExtensions.AddCasosDeUso"
Cohesion: 0.16
Nodes (11): DispositivosController, ActualizarClienteHandler, CrearClienteHandler, HistorialClienteHandler, ActualizarDispositivoHandler, CrearDispositivoHandler, HistorialDispositivoHandler, ListarDispositivosHandler (+3 more)

### Community 37 - "cliente-detalle.tsx"
Cohesion: 0.17
Nodes (16): ClienteRoute(), Detalle(), State, EstadoMe, useMe(), UsuarioActual, actualizarCliente(), historialDeCliente() (+8 more)

### Community 38 - "dispositivo-form.tsx"
Cohesion: 0.09
Nodes (30): Coincidencia, actualizarDispositivo(), crearDispositivo(), DatosDispositivo, Dispositivo, EdicionDispositivo, listarDispositivos(), NuevoDispositivo (+22 more)

### Community 39 - "CancellationToken"
Cohesion: 0.10
Nodes (9): RolesController, IRolRepositorio, RolDto, DispositivoRepositorio, RolRepositorio, DispositivoRepoEnMemoria, Dispositivos, Guardados (+1 more)

### Community 40 - "Dispositivo"
Cohesion: 0.12
Nodes (14): Dispositivo, Almacenamiento, ClienteId, Color, CreadoEn, CreadoPorUsuarioId, Id, ImeiSerie (+6 more)

### Community 41 - "LocalUnico"
Cohesion: 0.17
Nodes (10): ILocalActual, LocalId, Local, Id, Nombre, LocalesDeSistema, LocalUnico, LocalId (+2 more)

### Community 42 - "Program.cs"
Cohesion: 0.15
Nodes (6): DotEnv, Program, AssemblyMarker, PuntoIOS.Api.Configuration, PuntoIOS.Infrastructure, PuntoIOS.Api.Extensions

### Community 43 - "ObtenerUsuarioActualHandler"
Cohesion: 0.26
Nodes (6): MeController, ObtenerUsuarioActualHandler, UsuarioActualDto, FakeAccessor, ObtenerUsuarioActualHandlerTests, PuntoIOS.Application.Features.Identidad.ObtenerUsuarioActual

### Community 44 - "Cliente"
Cohesion: 0.14
Nodes (13): ClienteResumenDto, Cliente, Apellido, CreadoEn, CreadoPorUsuarioId, Direccion, Email, Id (+5 more)

### Community 45 - "ClientesConfiguraciones.cs"
Cohesion: 0.26
Nodes (4): RegistroAuditoriaConfiguration, ClienteConfiguration, DispositivoConfiguration, LocalConfiguration

### Community 49 - "InlineData"
Cohesion: 0.24
Nodes (5): PageRequest, Page, PageSize, Skip, PageRequestTests

### Community 51 - "DependencyInjection.AddInfrastructure"
Cohesion: 0.12
Nodes (3): ServiceCollectionExtensions, RegistrarUsuarioHandler, DependencyInjection

### Community 52 - "IdentificarUsuarioHandler.HandleAsync"
Cohesion: 0.17
Nodes (10): EstadoIdentificacion, Autorizado, Desactivado, NoRegistrado, IdentificarUsuarioHandler, ResultadoIdentificacion, Desactivado, NoRegistrado (+2 more)

### Community 53 - "PagedResult"
Cohesion: 0.22
Nodes (5): PagedResult, TotalPages, PagedResultTests, PuntoIOS.Application.Common.Pagination, PuntoIOS.UnitTests

### Community 54 - "DependencyInjection.cs"
Cohesion: 0.20
Nodes (4): HistorialDeAuditoria, PuntoIOS.Application.Features.Dispositivos.HistorialDispositivo, PuntoIOS.Application.Features.Dispositivos.Puertos, PuntoIOS.Application.Features.Dispositivos.ObtenerDispositivo

### Community 56 - "cliente-detalle.test.tsx"
Cohesion: 0.10
Nodes (10): @testing-library/react@^16.3.3, mocks, ana, Handler, historial, mocks, ana, Handler (+2 more)

### Community 57 - "ClienteValidacion.Validar"
Cohesion: 0.27
Nodes (3): EmailValidator, ClienteValidacion, PuntoIOS.Application.Common.Validation

### Community 58 - "CrearClienteHandler.HandleAsync"
Cohesion: 0.14
Nodes (8): IAuditoriaRepositorio, ClienteDto, ObtenerClienteHandler, IClienteRepositorio, DispositivoDto, ImeiDto, DispositivoValidacion, AuditoriaRepositorio

### Community 59 - "ClienteRepoEnMemoria"
Cohesion: 0.39
Nodes (3): ClienteRepoEnMemoria, Clientes, Guardados

## Knowledge Gaps
- **124 isolated node(s):** `Id`, `LocalId`, `Entidad`, `EntidadId`, `Accion` (+119 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 382 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **17 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `PuntoIosDbContext` connect `PuntoIosDbContext` to `Fact`, `Usuario`, `Task`, `DateTimeOffset`, `puntoios_application_common_pagination`, `RegistroAuditoria`, `ClientesController`, `Rol`, `FakesClientes.cs`, `ServiceCollectionExtensions.AddCasosDeUso`, `CancellationToken`, `Dispositivo`, `ClientesConfiguraciones.cs`, `DependencyInjection.AddInfrastructure`, `CrearClienteHandler.HandleAsync`, `ClienteRepoEnMemoria`, `IdentidadConfiguraciones.cs`, `PermisoConfiguration`, `UsuarioConfiguration`?**
  _High betweenness centrality (0.069) - this node is a cross-community bridge._
- **What connects `Id`, `LocalId`, `Entidad` to the rest of the system?**
  _124 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Punto iOS — Software Design Document (SDD)` be split into smaller, more focused modules?**
  _Cohesion score 0.09090909090909091 - nodes in this community are weakly interconnected._
- **Why does `PuntoIOS.Domain.Identidad` connect `PuntoIOS.Domain.Identidad` to `FakesClientes.cs`, `IdentidadConfiguraciones.cs`, `Usuario`, `ServiceCollectionExtensions.cs`, `IdentificarUsuarioHandler.HandleAsync`, `TestApi.cs`, `DateTimeOffset`, `puntoios_application_common_pagination`, `Rol`?**
  _High betweenness centrality (0.039) - this node is a cross-community bridge._
- **Should `PuntoIOS.ArchitectureTests.csproj` be split into smaller, more focused modules?**
  _Cohesion score 0.059800664451827246 - nodes in this community are weakly interconnected._
- **Why does `TestApi` connect `Fact` to `Program.cs`, `TestApi.cs`?**
  _High betweenness centrality (0.028) - this node is a cross-community bridge._
- **Should `3. Fases` be split into smaller, more focused modules?**
  _Cohesion score 0.09523809523809523 - nodes in this community are weakly interconnected._