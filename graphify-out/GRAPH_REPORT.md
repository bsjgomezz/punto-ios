# Graph Report - Punto iOS  (2026-11-07)

## Corpus Check
- 219 files · ~52,144 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 8 file(s) not represented in the graph (top: (none) 5, .example 1, .ico 1)

## Summary
- 1589 nodes · 4551 edges · 82 communities (61 shown, 21 thin omitted)
- Extraction: 93% EXTRACTED · 7% INFERRED · 0% AMBIGUOUS · INFERRED: 316 edges (avg confidence: 0.83)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `a0619313`
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
- seguimiento-cliente.tsx
- TestApi.cs
- Task
- DateTimeOffset
- PuntoIOS.Domain.Identidad
- Resultado
- puntoios_application_common_pagination
- TestApi
- equipos-cliente.tsx
- Rol
- clientes-page.tsx
- FakesClientes.cs
- login-form.tsx
- Fact
- nueva-orden-form.tsx
- cliente-detalle.tsx
- clientes/api.ts
- IRolRepositorio.ObtenerPorIdAsync
- Dispositivo
- orden-detalle.tsx
- TestApi.cs
- OrdenReparacion
- Cliente
- DispositivosEndpointTests
- Theory
- IReadOnlyList
- RecibirEquipoHandler.HandleAsync
- LayerDependencyTests
- RegistroAuditoria
- ServiceCollectionExtensions.AddCasosDeUso
- IdentificarUsuarioHandler.HandleAsync
- scripts
- DispositivosController.cs
- CancellationToken
- devDependencies
- OrdenReparacionDto
- Guid
- CrearClienteHandlerTests
- EntityTypeBuilder
- @playwright/test
- HealthEndpointTests
- eslint.config.mjs
- dependencies
- LocalUnico
- RecibirEquipoCommand
- vitest
- @testing-library/jest-dom
- ActualizarClienteHandlerTests
- PagedResult
- InlineData
- Cliente.Registrar
- ClientesConfiguraciones.cs
- TiposDeReparacion.cs
- PermisoConfiguration
- CambioDeCampo
- RolConfiguration
- ICurrentUserAccessor.cs

## God Nodes (most connected - your core abstractions)
1. `OrdenReparacion` - 40 edges
2. `Usuario` - 39 edges
3. `PuntoIosDbContext` - 38 edges
4. `PuntoIOS.Domain.Identidad` - 37 edges
5. `Cliente` - 31 edges
6. `useAuth()` - 31 edges
7. `Rol` - 27 edges
8. `RegistroAuditoria` - 26 edges
9. `PuntoIOS.Domain.Auditoria` - 25 edges
10. `Dispositivo` - 25 edges

## Surprising Connections (you probably didn't know these)
- `ReparacionesController` --references--> `HistorialOrdenHandler`  [EXTRACTED]
  backend/src/PuntoIOS.Api/Controllers/ReparacionesController.cs → backend/src/PuntoIOS.Application/Features/Reparaciones/ConsultarOrdenes/ConsultarOrdenesHandlers.cs
- `ReparacionesController` --references--> `ListarOrdenesHandler`  [EXTRACTED]
  backend/src/PuntoIOS.Api/Controllers/ReparacionesController.cs → backend/src/PuntoIOS.Application/Features/Reparaciones/ConsultarOrdenes/ConsultarOrdenesHandlers.cs
- `ReparacionesController` --references--> `ListarTecnicosHandler`  [EXTRACTED]
  backend/src/PuntoIOS.Api/Controllers/ReparacionesController.cs → backend/src/PuntoIOS.Application/Features/Reparaciones/ConsultarOrdenes/ConsultarOrdenesHandlers.cs
- `ReparacionesController` --references--> `ObtenerOrdenHandler`  [EXTRACTED]
  backend/src/PuntoIOS.Api/Controllers/ReparacionesController.cs → backend/src/PuntoIOS.Application/Features/Reparaciones/ConsultarOrdenes/ConsultarOrdenesHandlers.cs
- `ReparacionesController` --references--> `RecibirEquipoHandler`  [EXTRACTED]
  backend/src/PuntoIOS.Api/Controllers/ReparacionesController.cs → backend/src/PuntoIOS.Application/Features/Reparaciones/RecibirEquipo/RecibirEquipoHandler.cs

## Import Cycles
- None detected.

## Communities (82 total, 21 thin omitted)

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
Cohesion: 0.20
Nodes (9): tailwindcss@^4, @types/react-dom@^19, @vitejs/plugin-react@^6.2.1, name, private, version, tailwindcss, @types/react-dom (+1 more)

### Community 6 - "use-remote-checks.ts"
Cohesion: 0.15
Nodes (14): Home(), REMOTE_CHECKS, RemoteCheckId, Row, RowState, StatusBoard(), subscribeToConnection(), useOnline() (+6 more)

### Community 7 - "DispositivoHandlersTests"
Cohesion: 0.06
Nodes (15): ICifradoDeDatos, ActualizarDispositivoCommand, CrearDispositivoCommand, CodigoSeguimiento, CodigoSeguimientoTests, DispositivoHandlersTests, LocalFijo, LocalId (+7 more)

### Community 8 - "Instrucciones para Codex / Claude Code — Punto iOS"
Cohesion: 0.50
Nodes (3): Instrucciones para Codex / Claude Code — Punto iOS, Primer encargo, Reglas de trabajo

### Community 9 - "Punto iOS"
Cohesion: 0.25
Nodes (7): Cómo ejecutar, Documentación, Estado, Estructura, Pruebas, Punto iOS, Requisitos

### Community 10 - "PostgresReadyCheck"
Cohesion: 0.10
Nodes (4): PostgresReadyCheck, PostgresFixture, ConnectionString, PuntoIOS.Api.Health

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
Cohesion: 0.07
Nodes (22): ActualizarUsuarioRequest, UsuariosController, ActualizarUsuarioHandler, UsuarioAutorizado, ListarUsuariosHandler, IUsuarioRepositorio, UsuarioDto, Usuario (+14 more)

### Community 16 - "PuntoIosDbContext"
Cohesion: 0.12
Nodes (13): DesignTimeDbContextFactory, InicializadorBaseDeDatos, PuntoIosDbContext, Auditoria, Clientes, Dispositivos, LocalActualId, Locales (+5 more)

### Community 19 - "ServiceCollectionExtensions.cs"
Cohesion: 0.07
Nodes (8): TipoReparacionDto, PuntoIOS.Application.Features.Reparaciones.ConsultarOrdenes, PuntoIOS.Application.Features.Reparaciones.VerDesbloqueo, PuntoIOS.Application.Features.Reparaciones.RecibirEquipo, PuntoIOS.Api.Seguridad, PuntoIOS.Application.Features.Identidad.ObtenerUsuarioActual, PuntoIOS.UnitTests.Reparaciones, PuntoIOS.Application.Features.Identidad.ListarRoles

### Community 20 - "auth-provider.tsx"
Cohesion: 0.17
Nodes (13): body, display, metadata, RootLayout(), viewport, AuthContext, AuthContextValue, AuthProvider() (+5 more)

### Community 22 - "seguimiento-cliente.tsx"
Cohesion: 0.19
Nodes (19): formatoFecha, Props, SeguimientoCliente(), conToken(), enviarPorWhatsapp(), generar(), ver(), ana (+11 more)

### Community 23 - "TestApi.cs"
Cohesion: 0.24
Nodes (3): PuntoIOS.Api.Controllers, PuntoIOS.IntegrationTests, PuntoIOS.Application.Features.Reparaciones

### Community 25 - "DateTimeOffset"
Cohesion: 0.14
Nodes (5): InicialIdentidad, ClientesYLocales, AuditoriaYEdicionClientes, DispositivosDeClientes, OrdenesDeReparacion

### Community 26 - "PuntoIOS.Domain.Identidad"
Cohesion: 0.08
Nodes (17): ObtenerClienteHandler, Permiso, Codigo, Descripcion, Permisos, RolesDeSistema, PuntoIOS.Application.Common.Results, PuntoIOS.UnitTests.Identidad (+9 more)

### Community 27 - "Resultado"
Cohesion: 0.17
Nodes (10): ResultadoExtensions, Error, Resultado, Error, EsExitoso, Valor, TipoError, Conflicto (+2 more)

### Community 28 - "puntoios_application_common_pagination"
Cohesion: 0.18
Nodes (6): PuntoIOS.Application.Features.Reparaciones.Puertos, PuntoIOS.Application.Features.Clientes.Puertos, PuntoIOS.Application.Features.Clientes.BuscarClientes, PuntoIOS.Application.Features.Dispositivos.Puertos, PuntoIOS.Application.Features.Dispositivos.ObtenerDispositivo, PuntoIOS.Domain.Reparaciones

### Community 29 - "TestApi"
Cohesion: 0.15
Nodes (3): SeguimientoClienteEndpointTests, TestApi, Servicios

### Community 30 - "equipos-cliente.tsx"
Cohesion: 0.17
Nodes (9): listarDispositivos(), revelarImei(), EquiposCliente(), verImei(), Lista, Props, iphone, mocks (+1 more)

### Community 31 - "Rol"
Cohesion: 0.07
Nodes (19): CurrentUser, ActualizarUsuarioCommand, ObtenerUsuarioActualHandler, RegistrarUsuarioCommand, Rol, Descripcion, EsDeSistema, Id (+11 more)

### Community 32 - "clientes-page.tsx"
Cohesion: 0.17
Nodes (12): ClientesRoute(), PanelPage(), ReparacionesRoute(), useAuth(), RequireAuth(), mocks, ServerIdentity(), SessionBar() (+4 more)

### Community 33 - "FakesClientes.cs"
Cohesion: 0.12
Nodes (13): IAuditoriaRepositorio, PuntoIOS.UnitTests.Clientes, PuntoIOS.Application.Features.Clientes.ActualizarCliente, PuntoIOS.Application.Features.Dispositivos.CrearDispositivo, PuntoIOS.Application.Features.Dispositivos, PuntoIOS.Application.Features.Dispositivos.ActualizarDispositivo, PuntoIOS.Application.Features.Dispositivos.ListarDispositivos, PuntoIOS.Application.Features.Clientes.CrearCliente (+5 more)

### Community 34 - "login-form.tsx"
Cohesion: 0.19
Nodes (12): OjoAbierto(), OjoCerrado(), PasswordInput(), Props, describeAuthError(), LoginForm(), onGoogle(), onSubmit() (+4 more)

### Community 35 - "Fact"
Cohesion: 0.19
Nodes (3): RegistrarUsuarioRequest, MeEndpointTests, UsuariosEndpointTests

### Community 36 - "nueva-orden-form.tsx"
Cohesion: 0.11
Nodes (27): nextConfig, metadata, Dispositivo, AVISO_FECHA_ESTIMADA, ESTADOS, formatearFecha(), listarOrdenes(), listarTecnicos() (+19 more)

### Community 37 - "cliente-detalle.tsx"
Cohesion: 0.18
Nodes (13): ClienteRoute(), Detalle(), IngresarPage(), BackLink(), historialDeCliente(), obtenerCliente(), RegistroHistorial, ACCIONES (+5 more)

### Community 38 - "clientes/api.ts"
Cohesion: 0.09
Nodes (38): actualizarCliente(), CambioDeCampo, Cliente, Coincidencia, crearCliente(), DatosCliente, EquipoNuevo, NuevoCliente (+30 more)

### Community 39 - "IRolRepositorio.ObtenerPorIdAsync"
Cohesion: 0.15
Nodes (8): MeController, RolesController, ListarRolesHandler, UsuarioActualDto, IRolRepositorio, RolDto, RolRepositorio, RolRepositorioEnMemoria

### Community 40 - "Dispositivo"
Cohesion: 0.11
Nodes (14): Dispositivo, Almacenamiento, ClienteId, Color, CreadoEn, CreadoPorUsuarioId, Id, ImeiSerie (+6 more)

### Community 41 - "orden-detalle.tsx"
Cohesion: 0.13
Nodes (22): Detalle(), OrdenRoute(), State, EstadoMe, useMe(), UsuarioActual, nombreDeTipo(), obtenerOrden() (+14 more)

### Community 42 - "TestApi.cs"
Cohesion: 0.05
Nodes (15): FirebaseClaims, HttpContextCurrentUserAccessor, UsuarioLocalClaims, DotEnv, Program, IdentificarUsuarioHandler, AssemblyMarker, CodigoSeguimientoClientes (+7 more)

### Community 43 - "OrdenReparacion"
Cohesion: 0.08
Nodes (22): IOrdenReparacionRepositorio, OrdenReparacion, ClienteId, CreadoEn, CreadoPorUsuarioId, DesbloqueoCifrado, DesbloqueoTipo, DispositivoId (+14 more)

### Community 44 - "Cliente"
Cohesion: 0.11
Nodes (16): ClienteResumenDto, Cliente, Apellido, CodigoSeguimientoCifrado, CodigoSeguimientoGeneradoEn, CodigoSeguimientoHuella, CreadoEn, CreadoPorUsuarioId (+8 more)

### Community 45 - "DispositivosEndpointTests"
Cohesion: 0.35
Nodes (5): CrearClienteRequest, EquipoNuevoRequest, ActualizarDispositivoRequest, DispositivoDto, DispositivosEndpointTests

### Community 47 - "IReadOnlyList"
Cohesion: 0.10
Nodes (15): BuscarClientesHandler, IClienteRepositorio, IDirectorioDeUsuarios, UsuarioResumen, HistorialOrdenHandler, ListarOrdenesHandler, ListarTecnicosHandler, ObtenerOrdenHandler (+7 more)

### Community 50 - "RegistroAuditoria"
Cohesion: 0.14
Nodes (12): RegistroAuditoria, Accion, Cambios, Entidad, EntidadId, Fecha, Id, LocalId (+4 more)

### Community 51 - "ServiceCollectionExtensions.AddCasosDeUso"
Cohesion: 0.06
Nodes (20): ClientesController, DispositivosController, ServiceCollectionExtensions, CifradoConDataProtection, ActualizarClienteHandler, CrearClienteHandler, HistorialClienteHandler, ConsultarCodigoSeguimientoHandler (+12 more)

### Community 52 - "IdentificarUsuarioHandler.HandleAsync"
Cohesion: 0.29
Nodes (7): EstadoIdentificacion, Autorizado, Desactivado, NoRegistrado, ResultadoIdentificacion, Desactivado, NoRegistrado

### Community 53 - "scripts"
Cohesion: 0.25
Nodes (8): scripts, build, dev, lint, start, test, test:e2e, typecheck

### Community 54 - "DispositivosController.cs"
Cohesion: 0.31
Nodes (4): PuntoIOS.Application.Features.Dispositivos.HistorialDispositivo, PuntoIOS.Application.Features.Dispositivos.RevelarImei, PuntoIOS.Application.Features.Auditoria, PuntoIOS.Application.Features.Clientes.HistorialCliente

### Community 55 - "CancellationToken"
Cohesion: 0.16
Nodes (5): ImeiDto, DispositivoRepositorio, DispositivoRepoEnMemoria, Dispositivos, Guardados

### Community 56 - "devDependencies"
Cohesion: 0.17
Nodes (12): devDependencies, jsdom@^30.1.3, @tailwindcss/turbopack@^4, @types/node@^24.19.2, @types/react@^19, typescript@^5, vitest@^5.1.0, jsdom (+4 more)

### Community 57 - "OrdenReparacionDto"
Cohesion: 0.31
Nodes (4): CrearDispositivoRequest, RecibirEquipoRequest, OrdenReparacionDto, ReparacionesEndpointTests

### Community 58 - "Guid"
Cohesion: 0.16
Nodes (5): ReparacionesController, ActualizarClienteCommand, ClienteDto, OrdenResumenDto, DesbloqueoDto

### Community 59 - "CrearClienteHandlerTests"
Cohesion: 0.35
Nodes (3): CrearClienteCommand, EquipoNuevo, CrearClienteHandlerTests

### Community 60 - "EntityTypeBuilder"
Cohesion: 0.25
Nodes (5): ClienteConfiguration, DispositivoConfiguration, LocalConfiguration, UsuarioConfiguration, OrdenReparacionConfiguration

### Community 61 - "@playwright/test"
Cohesion: 0.29
Nodes (3): CORS, @playwright/test@^1.64.1, @playwright/test

### Community 63 - "eslint.config.mjs"
Cohesion: 0.33
Nodes (5): eslintConfig, eslint@^9, eslint-config-next@16.4.5, eslint, eslint-config-next

### Community 64 - "dependencies"
Cohesion: 0.33
Nodes (6): dependencies, firebase@^13.2.0, next@16.4.5, react@19.3.0, react-dom@19.3.0, react-dom

### Community 65 - "LocalUnico"
Cohesion: 0.17
Nodes (10): ILocalActual, LocalId, Local, Id, Nombre, LocalesDeSistema, LocalUnico, LocalId (+2 more)

### Community 66 - "RecibirEquipoCommand"
Cohesion: 0.21
Nodes (4): EmailValidator, ClienteValidacion, RecibirEquipoCommand, PuntoIOS.Application.Common.Validation

### Community 67 - "vitest"
Cohesion: 0.07
Nodes (21): @testing-library/react@^16.3.3, mocks, ana, Handler, historial, mocks, ana, Handler (+13 more)

### Community 70 - "PagedResult"
Cohesion: 0.22
Nodes (5): PagedResult, TotalPages, PagedResultTests, PuntoIOS.Application.Common.Pagination, PuntoIOS.UnitTests

### Community 71 - "InlineData"
Cohesion: 0.27
Nodes (5): PageRequest, Page, PageSize, Skip, PageRequestTests

### Community 74 - "TiposDeReparacion.cs"
Cohesion: 0.29
Nodes (3): EstadosDeReparacion, TiposDeDesbloqueo, TiposDeReparacion

### Community 76 - "CambioDeCampo"
Cohesion: 0.50
Nodes (3): AccionesAuditadas, CambioDeCampo, EntidadesAuditadas

## Knowledge Gaps
- **148 isolated node(s):** `Id`, `LocalId`, `Entidad`, `EntidadId`, `Accion` (+143 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 458 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **21 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `PuntoIosDbContext` connect `PuntoIosDbContext` to `IRolRepositorio.ObtenerPorIdAsync`, `OrdenReparacion`, `PermisoConfiguration`, `RolConfiguration`, `EntityTypeBuilder`, `IReadOnlyList`, `Usuario`, `RegistroAuditoria`, `ServiceCollectionExtensions.AddCasosDeUso`, `CancellationToken`, `OrdenReparacionDto`, `Guid`, `puntoios_application_common_pagination`?**
  _High betweenness centrality (0.038) - this node is a cross-community bridge._
- **What connects `Id`, `LocalId`, `Entidad` to the rest of the system?**
  _148 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Punto iOS — Software Design Document (SDD)` be split into smaller, more focused modules?**
  _Cohesion score 0.09090909090909091 - nodes in this community are weakly interconnected._
- **Why does `PuntoIOS.Domain.Identidad` connect `PuntoIOS.Domain.Identidad` to `ClientesConfiguraciones.cs`, `TestApi.cs`, `Usuario`, `ServiceCollectionExtensions.cs`, `DispositivosController.cs`, `TestApi.cs`, `puntoios_application_common_pagination`, `Rol`?**
  _High betweenness centrality (0.035) - this node is a cross-community bridge._
- **Should `PuntoIOS.ArchitectureTests.csproj` be split into smaller, more focused modules?**
  _Cohesion score 0.059800664451827246 - nodes in this community are weakly interconnected._
- **Why does `OrdenReparacion` connect `OrdenReparacion` to `IReadOnlyList`, `PuntoIosDbContext`, `RegistroAuditoria`, `Task`, `DateTimeOffset`, `Guid`, `EntityTypeBuilder`?**
  _High betweenness centrality (0.031) - this node is a cross-community bridge._
- **Should `3. Fases` be split into smaller, more focused modules?**
  _Cohesion score 0.09523809523809523 - nodes in this community are weakly interconnected._