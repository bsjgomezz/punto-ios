# Graph Report - Punto iOS  (2026-11-07)

## Corpus Check
- 229 files · ~58,602 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 8 file(s) not represented in the graph (top: (none) 5, .example 1, .ico 1)

## Summary
- 1722 nodes · 4988 edges · 88 communities (69 shown, 19 thin omitted)
- Extraction: 93% EXTRACTED · 7% INFERRED · 0% AMBIGUOUS · INFERRED: 347 edges (avg confidence: 0.84)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `c1620ea0`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- Punto iOS — Software Design Document (SDD)
- PuntoIOS.ArchitectureTests.csproj
- 3. Fases
- 0001-decisiones-iniciales.md
- vitest.config.mts
- Fact
- use-remote-checks.ts
- DispositivoHandlersTests
- Instrucciones para Codex / Claude Code — Punto iOS
- Punto iOS
- CurrentUser
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
- CancellationToken
- PuntoIOS.Domain.Identidad
- Resultado
- puntoios_application_common_pagination
- TestApi
- TestApi.cs
- Rol
- useAuth
- FakesClientes.cs
- vitest
- Fact
- reparaciones/api.ts
- cliente-detalle.tsx
- clientes/api.ts
- Guid
- Dispositivo
- react
- DateTimeOffset
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
- RecibirEquipoHandler.cs
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
- clientes-page.tsx
- @testing-library/jest-dom
- Cliente
- PageRequest
- RecibirEquipoHandler
- RecibirEquipoHandler.HandleAsync
- CodigoSeguimiento.Generar
- TiposDeReparacion.cs
- IdentidadConfiguraciones.cs
- FotoOrden
- ChecklistYFotosTests
- SeguimientoClienteHandlersTests
- DependencyInjection.AddInfrastructure
- RecibirEquipoHandlerTests
- nueva-orden-form.test.tsx
- Session state (auto-saved — last known reality)
- DotEnv
- CifradoConDataProtection
- @types/react@^19
- @types/react-dom@^19

## God Nodes (most connected - your core abstractions)
1. `OrdenReparacion` - 46 edges
2. `PuntoIosDbContext` - 41 edges
3. `Usuario` - 39 edges
4. `Cliente` - 31 edges
5. `useAuth()` - 31 edges
6. `PuntoIOS.Domain.Identidad` - 31 edges
7. `Rol` - 27 edges
8. `PuntoIOS.Domain.Auditoria` - 26 edges
9. `RegistroAuditoria` - 26 edges
10. `FotoOrden` - 26 edges

## Surprising Connections (you probably didn't know these)
- `TestApi` --references--> `Program`  [EXTRACTED]
  backend/tests/PuntoIOS.IntegrationTests/TestApi.cs → backend/src/PuntoIOS.Api/Program.cs
- `ReparacionesController` --references--> `AgregarFotoHandler`  [EXTRACTED]
  backend/src/PuntoIOS.Api/Controllers/ReparacionesController.cs → backend/src/PuntoIOS.Application/Features/Reparaciones/Fotos/FotosHandlers.cs
- `ReparacionesController` --references--> `ListarFotosHandler`  [EXTRACTED]
  backend/src/PuntoIOS.Api/Controllers/ReparacionesController.cs → backend/src/PuntoIOS.Application/Features/Reparaciones/Fotos/FotosHandlers.cs
- `ReparacionesController` --references--> `ObtenerFotoHandler`  [EXTRACTED]
  backend/src/PuntoIOS.Api/Controllers/ReparacionesController.cs → backend/src/PuntoIOS.Application/Features/Reparaciones/Fotos/FotosHandlers.cs
- `ReparacionesController` --references--> `RecibirEquipoHandler`  [EXTRACTED]
  backend/src/PuntoIOS.Api/Controllers/ReparacionesController.cs → backend/src/PuntoIOS.Application/Features/Reparaciones/RecibirEquipo/RecibirEquipoHandler.cs

## Import Cycles
- None detected.

## Communities (88 total, 19 thin omitted)

### Community 0 - "Punto iOS — Software Design Document (SDD)"
Cohesion: 0.09
Nodes (21): 10. Notificaciones internas, 11. PWA y offline, 12. API, 13. Seguridad y auditoría, 14. Pruebas, 15. Diseño visual, 16. Observabilidad, 17. Definition of Done por slice (+13 more)

### Community 1 - "PuntoIOS.ArchitectureTests.csproj"
Cohesion: 0.06
Nodes (35): net10.0, Microsoft.EntityFrameworkCore.Design (10.0.14), net10.0, Microsoft.NET.Sdk, net10.0, Microsoft.NET.Sdk, net10.0, Microsoft.EntityFrameworkCore.Design (10.0.14) (+27 more)

### Community 2 - "3. Fases"
Cohesion: 0.10
Nodes (20): 1. Ciclo de cada slice, 2. Hitos de uso real, 3. Fases, 4. Flujos E2E mínimos (Playwright), 5. Definition of Done, 6. Estrategia Git, Fase 10 — Accesorios, repuestos e inventario, Fase 11 — Finanzas → Hito C (+12 more)

### Community 5 - "Fact"
Cohesion: 0.25
Nodes (4): ActualizarClienteRequest, CrearClienteRequest, EquipoNuevoRequest, ClientesEndpointTests

### Community 6 - "use-remote-checks.ts"
Cohesion: 0.15
Nodes (14): Home(), REMOTE_CHECKS, RemoteCheckId, Row, RowState, StatusBoard(), subscribeToConnection(), useOnline() (+6 more)

### Community 7 - "DispositivoHandlersTests"
Cohesion: 0.24
Nodes (7): ActualizarDispositivoCommand, DispositivoHandlersTests, LocalFijo, LocalId, UsuarioFijo, RelojFijo, Ahora

### Community 8 - "Instrucciones para Codex / Claude Code — Punto iOS"
Cohesion: 0.50
Nodes (3): Instrucciones para Codex / Claude Code — Punto iOS, Primer encargo, Reglas de trabajo

### Community 9 - "Punto iOS"
Cohesion: 0.25
Nodes (7): Cómo ejecutar, Documentación, Estado, Estructura, Pruebas, Punto iOS, Requisitos

### Community 10 - "CurrentUser"
Cohesion: 0.05
Nodes (16): FirebaseClaims, HttpContextCurrentUserAccessor, UsuarioLocalClaims, PostgresReadyCheck, CurrentUser, IdentificarUsuarioHandler, ObtenerUsuarioActualHandler, UsuarioActualDto (+8 more)

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
Cohesion: 0.08
Nodes (20): UsuariosController, ActualizarUsuarioHandler, UsuarioAutorizado, ListarUsuariosHandler, IUsuarioRepositorio, Usuario, Activo, CreadoEn (+12 more)

### Community 16 - "PuntoIosDbContext"
Cohesion: 0.12
Nodes (13): DesignTimeDbContextFactory, PuntoIosDbContext, Auditoria, Clientes, Dispositivos, FotosOrden, LocalActualId, Locales (+5 more)

### Community 19 - "ServiceCollectionExtensions.cs"
Cohesion: 0.08
Nodes (6): TipoReparacionDto, PuntoIOS.Application.Features.Reparaciones.VerDesbloqueo, PuntoIOS.Application.Features.Reparaciones.RecibirEquipo, PuntoIOS.Api.Seguridad, PuntoIOS.UnitTests.Reparaciones, PuntoIOS.Application.Features.Reparaciones.Fotos

### Community 20 - "auth-provider.tsx"
Cohesion: 0.17
Nodes (12): body, display, metadata, RootLayout(), viewport, AuthContext, AuthContextValue, AuthProvider() (+4 more)

### Community 22 - "seguimiento-cliente.tsx"
Cohesion: 0.19
Nodes (19): formatoFecha, Props, SeguimientoCliente(), conToken(), enviarPorWhatsapp(), generar(), ver(), ana (+11 more)

### Community 23 - "TestApi.cs"
Cohesion: 0.27
Nodes (4): PuntoIOS.Api.Controllers, PuntoIOS.IntegrationTests, PuntoIOS.Application.Features.Auditoria, PuntoIOS.Application.Features.Reparaciones

### Community 24 - "Task"
Cohesion: 0.23
Nodes (3): RegistroHistorialDto, HistorialClienteHandler, HistorialDispositivoHandler

### Community 25 - "CancellationToken"
Cohesion: 0.14
Nodes (10): IAlmacenDeArchivos, ArchivoDeFoto, ListarFotosHandler, ObtenerFotoHandler, FotoDto, IOrdenReparacionRepositorio, AlmacenEnDisco, OrdenReparacionRepositorio (+2 more)

### Community 26 - "PuntoIOS.Domain.Identidad"
Cohesion: 0.07
Nodes (22): ListarRolesHandler, Permiso, Codigo, Descripcion, Permisos, RolesDeSistema, PuntoIOS.Application.Common.Results, PuntoIOS.Application.Features.Dispositivos.HistorialDispositivo (+14 more)

### Community 27 - "Resultado"
Cohesion: 0.17
Nodes (10): ResultadoExtensions, Error, Resultado, Error, EsExitoso, Valor, TipoError, Conflicto (+2 more)

### Community 28 - "puntoios_application_common_pagination"
Cohesion: 0.13
Nodes (6): HistorialDeAuditoria, PuntoIOS.Application.Features.Reparaciones.Puertos, PuntoIOS.Application.Features.Clientes.Puertos, PuntoIOS.Application.Features.Clientes.BuscarClientes, PuntoIOS.Application.Features.Reparaciones.ConsultarOrdenes, PuntoIOS.Application.Features.Clientes.ObtenerCliente

### Community 30 - "TestApi.cs"
Cohesion: 0.09
Nodes (4): Program, AssemblyMarker, PuntoIOS.Infrastructure, PuntoIOS.Api.Extensions

### Community 31 - "Rol"
Cohesion: 0.08
Nodes (15): ActualizarUsuarioCommand, RegistrarUsuarioCommand, Rol, Descripcion, EsDeSistema, Id, Nombre, Permisos (+7 more)

### Community 32 - "useAuth"
Cohesion: 0.11
Nodes (25): nextConfig, PanelPage(), ReparacionesRoute(), metadata, useAuth(), ServerIdentity(), State, mocks (+17 more)

### Community 33 - "FakesClientes.cs"
Cohesion: 0.15
Nodes (9): PuntoIOS.UnitTests.Clientes, PuntoIOS.Application.Features.Clientes.ActualizarCliente, PuntoIOS.Application.Features.Dispositivos, PuntoIOS.Application.Common.Security, PuntoIOS.Application.Features.Clientes, PuntoIOS.Application.Features.Auditoria.Puertos, PuntoIOS.Domain.Clientes, PuntoIOS.Domain.Auditoria (+1 more)

### Community 34 - "vitest"
Cohesion: 0.07
Nodes (27): OjoAbierto(), OjoCerrado(), PasswordInput(), Props, describeAuthError(), LoginForm(), onGoogle(), onSubmit() (+19 more)

### Community 35 - "Fact"
Cohesion: 0.16
Nodes (3): RegistrarUsuarioRequest, MeEndpointTests, UsuariosEndpointTests

### Community 36 - "reparaciones/api.ts"
Cohesion: 0.08
Nodes (41): Dispositivo, AVISO_FECHA_ESTIMADA, Checklist, CHECKLIST_CATALOGO, ESTADOS, Foto, ITEM_ENCIENDE, listarFotos() (+33 more)

### Community 37 - "cliente-detalle.tsx"
Cohesion: 0.12
Nodes (24): actualizarCliente(), CambioDeCampo, Cliente, crearCliente(), DatosCliente, EquipoNuevo, historialDeCliente(), NuevoCliente (+16 more)

### Community 38 - "clientes/api.ts"
Cohesion: 0.11
Nodes (28): Coincidencia, actualizarDispositivo(), crearDispositivo(), DatosDispositivo, EdicionDispositivo, listarDispositivos(), NuevoDispositivo, revelarImei() (+20 more)

### Community 39 - "Guid"
Cohesion: 0.19
Nodes (6): ActualizarUsuarioRequest, ActualizarClienteCommand, IRolRepositorio, UsuarioDto, RolRepositorio, RolRepositorioEnMemoria

### Community 40 - "Dispositivo"
Cohesion: 0.10
Nodes (17): AccionesAuditadas, CambioDeCampo, EntidadesAuditadas, Dispositivo, Almacenamiento, ClienteId, Color, CreadoEn (+9 more)

### Community 41 - "react"
Cohesion: 0.36
Nodes (7): ClienteRoute(), Detalle(), ClientesRoute(), Detalle(), OrdenRoute(), RequireAuth(), react

### Community 42 - "DateTimeOffset"
Cohesion: 0.08
Nodes (10): InicialIdentidad, ClientesYLocales, AuditoriaYEdicionClientes, DispositivosDeClientes, CodigoSeguimientoClientes, OrdenesDeReparacion, ChecklistYFotosDeOrdenes, PuntoIosDbContextModelSnapshot (+2 more)

### Community 43 - "OrdenReparacion"
Cohesion: 0.08
Nodes (21): OrdenReparacion, ChecklistItems, ClienteId, CreadoEn, CreadoPorUsuarioId, DesbloqueoCifrado, DesbloqueoTipo, DispositivoId (+13 more)

### Community 44 - "Cliente"
Cohesion: 0.10
Nodes (18): ClienteResumenDto, Cliente, Apellido, CodigoSeguimientoCifrado, CodigoSeguimientoGeneradoEn, CodigoSeguimientoHuella, CreadoEn, CreadoPorUsuarioId (+10 more)

### Community 45 - "DispositivosEndpointTests"
Cohesion: 0.36
Nodes (4): ActualizarDispositivoRequest, CrearDispositivoRequest, ImeiDto, DispositivosEndpointTests

### Community 46 - "Theory"
Cohesion: 0.21
Nodes (3): CrearDispositivoCommand, AutorizacionTests, Endpoints

### Community 47 - "IReadOnlyList"
Cohesion: 0.12
Nodes (8): IDirectorioDeUsuarios, UsuarioResumen, RolDto, HistorialOrdenHandler, ListarTecnicosHandler, ArmadorDeOrdenes, DirectorioDeUsuarios, DirectorioFijo

### Community 50 - "RegistroAuditoria"
Cohesion: 0.14
Nodes (12): RegistroAuditoria, Accion, Cambios, Entidad, EntidadId, Fecha, Id, LocalId (+4 more)

### Community 52 - "IdentificarUsuarioHandler.HandleAsync"
Cohesion: 0.29
Nodes (7): EstadoIdentificacion, Autorizado, Desactivado, NoRegistrado, ResultadoIdentificacion, Desactivado, NoRegistrado

### Community 53 - "scripts"
Cohesion: 0.25
Nodes (8): scripts, build, dev, lint, start, test, test:e2e, typecheck

### Community 54 - "RecibirEquipoHandler.cs"
Cohesion: 0.18
Nodes (6): PuntoIOS.Application.Common.Archivos, PuntoIOS.Application.Features.Clientes.CrearCliente, PuntoIOS.Application.Features.Dispositivos.Puertos, PuntoIOS.Application.Features.Clientes.HistorialCliente, PuntoIOS.Domain.Reparaciones, PuntoIOS.Infrastructure.Archivos

### Community 55 - "CancellationToken"
Cohesion: 0.18
Nodes (4): DispositivoRepositorio, DispositivoRepoEnMemoria, Dispositivos, Guardados

### Community 56 - "devDependencies"
Cohesion: 0.15
Nodes (13): devDependencies, jsdom@^30.1.3, tailwindcss@^4, @tailwindcss/turbopack@^4, @testing-library/react@^16.3.3, @types/node@^24.19.2, typescript@^5, vitest@^5.1.0 (+5 more)

### Community 57 - "OrdenReparacionDto"
Cohesion: 0.21
Nodes (7): ChecklistRequest, RecibirEquipoRequest, ChecklistDto, OrdenReparacionDto, OrdenResumenDto, ChecklistYFotosEndpointTests, ReparacionesEndpointTests

### Community 58 - "Guid"
Cohesion: 0.14
Nodes (5): DispositivosController, MeController, ReparacionesController, RolesController, ClienteDto

### Community 59 - "CrearClienteHandlerTests"
Cohesion: 0.38
Nodes (3): CrearClienteCommand, EquipoNuevo, CrearClienteHandlerTests

### Community 60 - "EntityTypeBuilder"
Cohesion: 0.20
Nodes (6): ClienteConfiguration, DispositivoConfiguration, LocalConfiguration, UsuarioConfiguration, FotoOrdenConfiguration, OrdenReparacionConfiguration

### Community 61 - "@playwright/test"
Cohesion: 0.29
Nodes (3): CORS, @playwright/test@^1.64.1, @playwright/test

### Community 63 - "eslint.config.mjs"
Cohesion: 0.33
Nodes (5): eslintConfig, eslint@^9, eslint-config-next@16.4.5, eslint, eslint-config-next

### Community 64 - "dependencies"
Cohesion: 0.22
Nodes (10): dependencies, firebase@^13.2.0, next@16.4.5, react@19.3.0, react-dom@19.3.0, name, private, version (+2 more)

### Community 65 - "LocalUnico"
Cohesion: 0.17
Nodes (10): ILocalActual, LocalId, Local, Id, Nombre, LocalesDeSistema, LocalUnico, LocalId (+2 more)

### Community 66 - "RecibirEquipoCommand"
Cohesion: 0.23
Nodes (3): EmailValidator, ClienteValidacion, PuntoIOS.Application.Common.Validation

### Community 67 - "clientes-page.tsx"
Cohesion: 0.13
Nodes (9): IngresarPage(), BackLink(), Pagina, ClientesPage(), Resultado, ana, Handler, mocks (+1 more)

### Community 69 - "Cliente"
Cohesion: 0.13
Nodes (9): BuscarClientesHandler, ObtenerClienteHandler, IClienteRepositorio, ListarOrdenesHandler, ObtenerOrdenHandler, ClienteRepositorio, ClienteRepoEnMemoria, Clientes (+1 more)

### Community 70 - "PageRequest"
Cohesion: 0.13
Nodes (10): PagedResult, TotalPages, PageRequest, Page, PageSize, Skip, PagedResultTests, PageRequestTests (+2 more)

### Community 71 - "RecibirEquipoHandler"
Cohesion: 0.26
Nodes (12): ICifradoDeDatos, IAuditoriaRepositorio, ActualizarClienteHandler, CrearClienteHandler, ConsultarCodigoSeguimientoHandler, GenerarCodigoSeguimientoHandler, ActualizarDispositivoHandler, CrearDispositivoHandler (+4 more)

### Community 72 - "RecibirEquipoHandler.HandleAsync"
Cohesion: 0.23
Nodes (5): DispositivoDto, DispositivoValidacion, ListarDispositivosHandler, ObtenerDispositivoHandler, IDispositivoRepositorio

### Community 73 - "CodigoSeguimiento.Generar"
Cohesion: 0.19
Nodes (3): FormatoDeImagen, CodigoSeguimiento, CodigoSeguimientoTests

### Community 74 - "TiposDeReparacion.cs"
Cohesion: 0.33
Nodes (3): EstadosDeReparacion, TiposDeDesbloqueo, TiposDeReparacion

### Community 75 - "IdentidadConfiguraciones.cs"
Cohesion: 0.20
Nodes (5): RegistroAuditoriaConfiguration, PermisoConfiguration, RolConfiguration, RolPermisoConfiguration, PuntoIOS.Infrastructure.Persistence.Configurations

### Community 76 - "FotoOrden"
Cohesion: 0.14
Nodes (13): FotoOrden, CreadoEn, CreadoPorUsuarioId, Id, LocalId, OrdenId, Ruta, Tamano (+5 more)

### Community 77 - "ChecklistYFotosTests"
Cohesion: 0.31
Nodes (4): ChecklistIngreso, ItemChecklist, ItemsDeChecklist, ChecklistYFotosTests

### Community 80 - "RecibirEquipoHandlerTests"
Cohesion: 0.32
Nodes (4): RecibirEquipoCommand, DesbloqueoDto, RecibirEquipoHandlerTests, Invalidos

### Community 81 - "nueva-orden-form.test.tsx"
Cohesion: 0.33
Nodes (5): ana, iphone, json(), mocks, stubApi()

### Community 82 - "Session state (auto-saved — last known reality)"
Cohesion: 0.40
Nodes (4): Recent commits, Resume, Session state (auto-saved — last known reality), Uncommitted changes

## Knowledge Gaps
- **163 isolated node(s):** `Id`, `LocalId`, `Entidad`, `EntidadId`, `Accion` (+158 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 491 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **19 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `PuntoIosDbContext` connect `PuntoIosDbContext` to `Cliente`, `Guid`, `RecibirEquipoHandler`, `OrdenReparacion`, `FotoOrden`, `IdentidadConfiguraciones.cs`, `EntityTypeBuilder`, `IReadOnlyList`, `DependencyInjection.AddInfrastructure`, `Usuario`, `RegistroAuditoria`, `OrdenReparacionDto`, `CancellationToken`, `CancellationToken`, `puntoios_application_common_pagination`, `TestApi`, `Rol`?**
  _High betweenness centrality (0.039) - this node is a cross-community bridge._
- **What connects `Id`, `LocalId`, `Entidad` to the rest of the system?**
  _163 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Punto iOS — Software Design Document (SDD)` be split into smaller, more focused modules?**
  _Cohesion score 0.09090909090909091 - nodes in this community are weakly interconnected._
- **Why does `OrdenReparacion` connect `OrdenReparacion` to `Guid`, `DateTimeOffset`, `FotoOrden`, `ChecklistYFotosTests`, `IReadOnlyList`, `PuntoIosDbContext`, `RegistroAuditoria`, `Task`, `CancellationToken`, `EntityTypeBuilder`?**
  _High betweenness centrality (0.030) - this node is a cross-community bridge._
- **Should `PuntoIOS.ArchitectureTests.csproj` be split into smaller, more focused modules?**
  _Cohesion score 0.059800664451827246 - nodes in this community are weakly interconnected._
- **Why does `PuntoIOS.Domain.Identidad` connect `PuntoIOS.Domain.Identidad` to `DateTimeOffset`, `IdentidadConfiguraciones.cs`, `Usuario`, `ServiceCollectionExtensions.cs`, `TestApi.cs`, `puntoios_application_common_pagination`, `Rol`?**
  _High betweenness centrality (0.023) - this node is a cross-community bridge._
- **Should `3. Fases` be split into smaller, more focused modules?**
  _Cohesion score 0.09523809523809523 - nodes in this community are weakly interconnected._