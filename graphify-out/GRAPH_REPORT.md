# Graph Report - Punto iOS  (2026-11-12)

## Corpus Check
- 259 files · ~85,131 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 8 file(s) not represented in the graph (top: (none) 5, .example 1, .ico 1)

## Summary
- 2098 nodes · 6680 edges · 102 communities (78 shown, 24 thin omitted)
- Extraction: 92% EXTRACTED · 8% INFERRED · 0% AMBIGUOUS · INFERRED: 534 edges (avg confidence: 0.84)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `5eeffbec`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- Punto iOS — Software Design Document (SDD)
- PuntoIOS.ArchitectureTests.csproj
- 3. Fases
- 0001-decisiones-iniciales.md
- ChecklistYFotosTests
- Dispositivo
- use-remote-checks.ts
- RecibirEquipoHandlerTests
- Instrucciones para Codex / Claude Code — Punto iOS
- Punto iOS
- PostgresFixture
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
- seguimiento-cliente.tsx
- puntoios_application_common_pagination
- reparaciones-page.tsx
- Fact
- CambiarEstadoOrdenHandlerTests
- Resultado
- ReparacionesEndpointTests.OrdenesDeDosTecnicosAsync
- clientes/api.ts
- TestApi.cs
- Rol
- DispositivoDto
- OrdenRepoEnMemoria
- VersionDePresupuesto
- IClassFixture
- reparaciones/api.ts
- IdentificarUsuarioHandler.cs
- DispositivoRepoEnMemoria
- MaquinaDeEstadosTests
- TestApi.CrearAsync
- LocalUnico
- DateTimeOffset
- OrdenReparacion
- Cliente
- UsuarioLocalClaims.AsociarAsync
- DispositivosController.cs
- Guid
- List
- LayerDependencyTests
- RegistroAuditoria
- RelojFijo
- Permiso
- ReparacionesEndpointTests.cs
- puntoios_application_features_identidad_puertos
- PuntoIOS.Domain.Reparaciones
- devDependencies
- PuntoIOS.Application.Common.Security
- Guid
- PuntoIOS.Application.Features.Identidad.ObtenerUsuarioActual
- EntityTypeBuilder
- login-form.tsx
- IAuditoriaRepositorio
- DatosTecnicosEndpointTests.OrdenAsync
- CrearClienteHandlerTests
- ServiceCollectionExtensions.AddCasosDeUso
- Usuario.Registrar
- vitest
- CancellationToken
- CrearVersionPresupuestoHandlerTests
- HttpClient
- CurrentUser
- RecibirEquipoHandler.HandleAsync
- OrdenReparacionDto
- AuditoriaRepoEnMemoria
- Stream
- UsuarioFijo
- dependencies
- scripts
- orden-detalle.test.tsx
- ReparacionesConfiguraciones.cs
- @playwright/test
- Session state (auto-saved — last known reality)
- DotEnv
- useAuth
- eslint.config.mjs
- DependencyInjection.AddInfrastructure
- @testing-library/jest-dom
- TiposDeReparacion.cs
- ArgumentException
- Theory
- IDirectorioDeUsuarios
- ClienteRepoEnMemoria
- RecibirEquipoRequest
- CambiarEstadoOrdenHandlerTests
- package.json
- ListarOrdenesHandlerTests
- CifradoConDataProtection
- PermisoConfiguration

## God Nodes (most connected - your core abstractions)
1. `OrdenReparacionDto` - 52 edges
2. `OrdenReparacion` - 49 edges
3. `OrdenDetalle()` - 44 edges
4. `PuntoIosDbContext` - 42 edges
5. `Usuario` - 39 edges
6. `VersionDePresupuesto` - 37 edges
7. `PuntoIOS.Domain.Identidad` - 36 edges
8. `PuntoIOS.Domain.Auditoria` - 35 edges
9. `ReparacionesController` - 32 edges
10. `Cliente` - 31 edges

## Surprising Connections (you probably didn't know these)
- `TestApi` --references--> `Program`  [EXTRACTED]
  backend/tests/PuntoIOS.IntegrationTests/TestApi.cs → backend/src/PuntoIOS.Api/Program.cs
- `ReparacionesController` --references--> `ActualizarDatosTecnicosHandler`  [EXTRACTED]
  backend/src/PuntoIOS.Api/Controllers/ReparacionesController.cs → backend/src/PuntoIOS.Application/Features/Reparaciones/ActualizarDatosTecnicos/ActualizarDatosTecnicosHandler.cs
- `OrdenReparacionConfiguration` --references--> `OrdenReparacion`  [EXTRACTED]
  backend/src/PuntoIOS.Infrastructure/Persistence/Configurations/ReparacionesConfiguraciones.cs → backend/src/PuntoIOS.Domain/Reparaciones/OrdenReparacion.cs
- `ActualizarDatosTecnicosHandlerTests` --references--> `OrdenReparacion`  [EXTRACTED]
  backend/tests/PuntoIOS.UnitTests/Reparaciones/DatosTecnicosTests.cs → backend/src/PuntoIOS.Domain/Reparaciones/OrdenReparacion.cs
- `OrdenDetalle()` --calls--> `nombreDeEstado()`  [EXTRACTED]
  frontend/src/features/reparaciones/orden-detalle.tsx → frontend/src/features/reparaciones/api.ts

## Import Cycles
- None detected.

## Communities (102 total, 24 thin omitted)

### Community 0 - "Punto iOS — Software Design Document (SDD)"
Cohesion: 0.09
Nodes (21): 10. Notificaciones internas, 11. PWA y offline, 12. API, 13. Seguridad y auditoría, 14. Pruebas, 15. Diseño visual, 16. Observabilidad, 17. Definition of Done por slice (+13 more)

### Community 1 - "PuntoIOS.ArchitectureTests.csproj"
Cohesion: 0.06
Nodes (35): net10.0, Microsoft.EntityFrameworkCore.Design (10.0.14), net10.0, Microsoft.NET.Sdk, net10.0, Microsoft.NET.Sdk, net10.0, Microsoft.EntityFrameworkCore.Design (10.0.14) (+27 more)

### Community 2 - "3. Fases"
Cohesion: 0.10
Nodes (20): 1. Ciclo de cada slice, 2. Hitos de uso real, 3. Fases, 4. Flujos E2E mínimos (Playwright), 5. Definition of Done, 6. Estrategia Git, Fase 10 — Accesorios, repuestos e inventario, Fase 11 — Finanzas → Hito C (+12 more)

### Community 4 - "ChecklistYFotosTests"
Cohesion: 0.40
Nodes (3): ChecklistIngreso, ItemChecklist, ChecklistYFotosTests

### Community 5 - "Dispositivo"
Cohesion: 0.11
Nodes (14): Dispositivo, Almacenamiento, ClienteId, Color, CreadoEn, CreadoPorUsuarioId, Id, ImeiSerie (+6 more)

### Community 6 - "use-remote-checks.ts"
Cohesion: 0.15
Nodes (14): Home(), REMOTE_CHECKS, RemoteCheckId, Row, RowState, StatusBoard(), subscribeToConnection(), useOnline() (+6 more)

### Community 7 - "RecibirEquipoHandlerTests"
Cohesion: 0.29
Nodes (4): RecibirEquipoCommand, DesbloqueoDto, RecibirEquipoHandlerTests, Invalidos

### Community 8 - "Instrucciones para Codex / Claude Code — Punto iOS"
Cohesion: 0.50
Nodes (3): Instrucciones para Codex / Claude Code — Punto iOS, Primer encargo, Reglas de trabajo

### Community 9 - "Punto iOS"
Cohesion: 0.25
Nodes (7): Cómo ejecutar, Documentación, Estado, Estructura, Pruebas, Punto iOS, Requisitos

### Community 10 - "PostgresFixture"
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

### Community 15 - "Task"
Cohesion: 0.07
Nodes (20): RegistroHistorialDto, IAuditoriaRepositorio, ListarUsuariosHandler, IUsuarioRepositorio, OrdenResumenDto, Usuario, Activo, CreadoEn (+12 more)

### Community 16 - "PuntoIosDbContext"
Cohesion: 0.11
Nodes (14): DesignTimeDbContextFactory, PuntoIosDbContext, Auditoria, Clientes, Dispositivos, FotosOrden, LocalActualId, Locales (+6 more)

### Community 19 - "ServiceCollectionExtensions.cs"
Cohesion: 0.06
Nodes (7): ActualizarClienteRequest, CambiarEstadoRequest, ResponderPresupuestoRequest, PuntoIOS.Application.Features.Reparaciones.ResponderPresupuesto, PuntoIOS.Api.Seguridad, PuntoIOS.Application.Features.Clientes, PuntoIOS.Application.Features.Clientes.SeguimientoCliente

### Community 20 - "auth-provider.tsx"
Cohesion: 0.17
Nodes (13): body, display, metadata, RootLayout(), viewport, AuthContext, AuthContextValue, AuthProvider() (+5 more)

### Community 22 - "seguimiento-cliente.tsx"
Cohesion: 0.18
Nodes (20): formatoFecha, Props, SeguimientoCliente(), conToken(), enviarPorWhatsapp(), generar(), ver(), ana (+12 more)

### Community 23 - "puntoios_application_common_pagination"
Cohesion: 0.08
Nodes (12): ResultadoExtensions, PuntoIOS.Application.Common.Results, PuntoIOS.Application.Features.Reparaciones.Puertos, PuntoIOS.Application.Features.Clientes.Puertos, PuntoIOS.Application.Features.Clientes.BuscarClientes, PuntoIOS.Application.Features.Reparaciones.ConsultarOrdenes, PuntoIOS.UnitTests.Identidad, PuntoIOS.Application.Features.Identidad.ActualizarUsuario (+4 more)

### Community 24 - "reparaciones-page.tsx"
Cohesion: 0.15
Nodes (13): buscarClientes(), formatearFecha(), listarOrdenes(), nombreDeEstado(), OrdenResumen, ListaOrdenes(), hoy(), NuevaOrdenForm() (+5 more)

### Community 26 - "CambiarEstadoOrdenHandlerTests"
Cohesion: 0.22
Nodes (3): ActualizarDatosTecnicosCommand, ItemsDeChecklist, ActualizarDatosTecnicosHandlerTests

### Community 27 - "Resultado"
Cohesion: 0.23
Nodes (9): Error, Resultado, Error, EsExitoso, Valor, TipoError, Conflicto, NoEncontrado (+1 more)

### Community 28 - "ReparacionesEndpointTests.OrdenesDeDosTecnicosAsync"
Cohesion: 0.21
Nodes (3): CrearDispositivoRequest, RecibirEquipoRequest, ReparacionesEndpointTests

### Community 29 - "clientes/api.ts"
Cohesion: 0.06
Nodes (55): ClienteRoute(), Detalle(), State, EstadoMe, useMe(), UsuarioActual, actualizarCliente(), Cliente (+47 more)

### Community 30 - "TestApi.cs"
Cohesion: 0.06
Nodes (7): Program, FormatoDeImagen, CodigoSeguimiento, AssemblyMarker, CodigoSeguimientoTests, PuntoIOS.Infrastructure, PuntoIOS.Api.Extensions

### Community 31 - "Rol"
Cohesion: 0.11
Nodes (11): Rol, Descripcion, EsDeSistema, Id, Nombre, Permisos, RolPermiso, PermisoCodigo (+3 more)

### Community 32 - "DispositivoDto"
Cohesion: 0.35
Nodes (4): CrearClienteRequest, EquipoNuevoRequest, ActualizarDispositivoRequest, DispositivosEndpointTests

### Community 33 - "OrdenRepoEnMemoria"
Cohesion: 0.12
Nodes (6): OrdenReparacionRepositorio, OrdenRepoEnMemoria, Fotos, Guardados, Ordenes, VersionesPresupuesto

### Community 34 - "VersionDePresupuesto"
Cohesion: 0.11
Nodes (15): ResponderPresupuestoCommand, VersionDePresupuesto, CreadoEn, CreadoPorUsuarioId, Id, Items, LocalId, Numero (+7 more)

### Community 35 - "IClassFixture"
Cohesion: 0.16
Nodes (4): RegistrarUsuarioRequest, HealthEndpointTests, SeguimientoClienteEndpointTests, UsuariosEndpointTests

### Community 36 - "reparaciones/api.ts"
Cohesion: 0.05
Nodes (71): CambioDeCampo, actualizarDatosTecnicos(), AVISO_FECHA_ESTIMADA, cambiarEstadoDeOrden(), Checklist, CHECKLIST_CATALOGO, crearVersionPresupuesto(), DatosTecnicos (+63 more)

### Community 37 - "IdentificarUsuarioHandler.cs"
Cohesion: 0.25
Nodes (9): EstadoIdentificacion, Autorizado, Desactivado, NoRegistrado, IdentificarUsuarioHandler, ResultadoIdentificacion, Desactivado, NoRegistrado (+1 more)

### Community 38 - "DispositivoRepoEnMemoria"
Cohesion: 0.50
Nodes (3): DispositivoRepoEnMemoria, Dispositivos, Guardados

### Community 40 - "TestApi.CrearAsync"
Cohesion: 0.11
Nodes (4): ClientesEndpointTests, MeEndpointTests, TestApi, Servicios

### Community 41 - "LocalUnico"
Cohesion: 0.17
Nodes (10): ILocalActual, LocalId, Local, Id, Nombre, LocalesDeSistema, LocalUnico, LocalId (+2 more)

### Community 42 - "DateTimeOffset"
Cohesion: 0.05
Nodes (24): FotoOrden, CreadoEn, CreadoPorUsuarioId, Id, LocalId, OrdenId, Ruta, Tamano (+16 more)

### Community 43 - "OrdenReparacion"
Cohesion: 0.07
Nodes (27): OrdenReparacion, ChecklistItems, ClienteId, ComponentesAfectados, CreadoEn, CreadoPorUsuarioId, DesbloqueoCifrado, DesbloqueoTipo (+19 more)

### Community 44 - "Cliente"
Cohesion: 0.07
Nodes (20): EmailValidator, ClienteValidacion, Cliente, Apellido, CodigoSeguimientoCifrado, CodigoSeguimientoGeneradoEn, CodigoSeguimientoHuella, CreadoEn (+12 more)

### Community 45 - "UsuarioLocalClaims.AsociarAsync"
Cohesion: 0.14
Nodes (5): FirebaseClaims, HttpContextCurrentUserAccessor, UsuarioLocalClaims, PuntoIOS.Application.Features.Identidad.IdentificarUsuario, PuntoIOS.Api.Authentication

### Community 46 - "DispositivosController.cs"
Cohesion: 0.12
Nodes (12): RolesController, ListarRolesHandler, PuntoIOS.Application.Features.Dispositivos.HistorialDispositivo, PuntoIOS.Application.Features.Dispositivos.CrearDispositivo, PuntoIOS.Application.Features.Dispositivos, PuntoIOS.Application.Features.Identidad, PuntoIOS.Application.Features.Dispositivos.ActualizarDispositivo, PuntoIOS.Application.Features.Dispositivos.ListarDispositivos (+4 more)

### Community 47 - "Guid"
Cohesion: 0.10
Nodes (8): BuscarClientesHandler, ObtenerClienteHandler, IClienteRepositorio, IDirectorioDeUsuarios, UsuarioResumen, ChecklistDto, DirectorioDeUsuarios, ClienteRepositorio

### Community 48 - "List"
Cohesion: 0.37
Nodes (5): CrearVersionPresupuestoRequest, ItemPresupuestoRequest, ItemPresupuestoDto, VersionDePresupuestoDto, PresupuestoEndpointTests

### Community 50 - "RegistroAuditoria"
Cohesion: 0.12
Nodes (13): AccionesAuditadas, EntidadesAuditadas, RegistroAuditoria, Accion, Cambios, Entidad, EntidadId, Fecha (+5 more)

### Community 51 - "RelojFijo"
Cohesion: 0.26
Nodes (7): ActualizarDispositivoCommand, CrearDispositivoCommand, DispositivoHandlersTests, LocalFijo, LocalId, RelojFijo, Ahora

### Community 52 - "Permiso"
Cohesion: 0.50
Nodes (3): Permiso, Codigo, Descripcion

### Community 53 - "ReparacionesEndpointTests.cs"
Cohesion: 0.21
Nodes (6): Permisos, RolesDeSistema, PuntoIOS.Api.Controllers, PuntoIOS.IntegrationTests, PuntoIOS.Domain.Identidad, PuntoIOS.Application.Features.Reparaciones

### Community 54 - "puntoios_application_features_identidad_puertos"
Cohesion: 0.22
Nodes (5): HistorialDeAuditoria, PuntoIOS.Application.Features.Reparaciones.ListarVersionesPresupuesto, PuntoIOS.Application.Common.Archivos, PuntoIOS.Application.Features.Auditoria, PuntoIOS.Infrastructure.Archivos

### Community 55 - "PuntoIOS.Domain.Reparaciones"
Cohesion: 0.11
Nodes (15): PuntoIOS.Application.Features.Reparaciones.CrearVersionPresupuesto, PuntoIOS.UnitTests.Clientes, PuntoIOS.Application.Features.Clientes.ActualizarCliente, PuntoIOS.Application.Features.Reparaciones.VerDesbloqueo, PuntoIOS.Application.Features.Reparaciones.RecibirEquipo, PuntoIOS.Application.Features.Reparaciones.ActualizarDatosTecnicos, PuntoIOS.Application.Features.Reparaciones.CambiarEstado, PuntoIOS.Application.Features.Clientes.CrearCliente (+7 more)

### Community 56 - "devDependencies"
Cohesion: 0.17
Nodes (12): devDependencies, jsdom@^30.1.3, @tailwindcss/turbopack@^4, @types/node@^24.19.2, @types/react@^19, typescript@^5, vitest@^5.1.0, jsdom (+4 more)

### Community 57 - "PuntoIOS.Application.Common.Security"
Cohesion: 0.16
Nodes (8): ICurrentUserAccessor, ObtenerUsuarioActualHandler, UsuarioActualDto, FakeAccessor, ObtenerUsuarioActualHandlerTests, PuntoIOS.UnitTests, PuntoIOS.Application.Common.Security, PuntoIOS.Application.Features.Identidad.ObtenerUsuarioActual

### Community 58 - "Guid"
Cohesion: 0.11
Nodes (8): DispositivosController, TipoReparacionDto, ActualizarUsuarioRequest, ActualizarClienteCommand, ClienteDto, CrearClienteCommand, EquipoNuevo, FotoDto

### Community 59 - "PuntoIOS.Application.Features.Identidad.ObtenerUsuarioActual"
Cohesion: 0.14
Nodes (9): PagedResult, TotalPages, PageRequest, Page, PageSize, Skip, PagedResultTests, PageRequestTests (+1 more)

### Community 60 - "EntityTypeBuilder"
Cohesion: 0.20
Nodes (6): DispositivoConfiguration, LocalConfiguration, UsuarioConfiguration, FotoOrdenConfiguration, OrdenReparacionConfiguration, VersionDePresupuestoConfiguration

### Community 61 - "login-form.tsx"
Cohesion: 0.16
Nodes (14): IngresarPage(), BackLink(), OjoAbierto(), OjoCerrado(), PasswordInput(), Props, describeAuthError(), LoginForm() (+6 more)

### Community 62 - "IAuditoriaRepositorio"
Cohesion: 0.35
Nodes (14): ActualizarClienteHandler, CrearClienteHandler, HistorialClienteHandler, ConsultarCodigoSeguimientoHandler, GenerarCodigoSeguimientoHandler, ActualizarDispositivoHandler, CrearDispositivoHandler, RevelarImeiHandler (+6 more)

### Community 65 - "ServiceCollectionExtensions.AddCasosDeUso"
Cohesion: 0.07
Nodes (3): ClientesController, MeController, ReparacionesController

### Community 66 - "Usuario.Registrar"
Cohesion: 0.25
Nodes (4): ActualizarUsuarioCommand, RegistrarUsuarioCommand, InicializadorBaseDeDatos, GestionDeUsuariosTests

### Community 67 - "vitest"
Cohesion: 0.05
Nodes (28): @testing-library/react@^16.3.3, mocks, ana, Handler, historial, mocks, ana, Handler (+20 more)

### Community 68 - "CancellationToken"
Cohesion: 0.12
Nodes (8): UsuariosController, ActualizarUsuarioHandler, IRolRepositorio, RolDto, UsuarioDto, DispositivoRepositorio, RolRepositorio, RolRepositorioEnMemoria

### Community 69 - "CrearVersionPresupuestoHandlerTests"
Cohesion: 0.42
Nodes (4): CrearVersionPresupuestoCommand, ItemPresupuestoInput, CambioDeCampo, CrearVersionPresupuestoHandlerTests

### Community 72 - "RecibirEquipoHandler.HandleAsync"
Cohesion: 0.13
Nodes (9): ClienteResumenDto, CodigoSeguimientoDto, DispositivoDto, ImeiDto, DispositivoValidacion, ListarDispositivosHandler, ObtenerDispositivoHandler, IDispositivoRepositorio (+1 more)

### Community 75 - "Stream"
Cohesion: 0.36
Nodes (4): IAlmacenDeArchivos, ArchivoDeFoto, ObtenerFotoHandler, AlmacenEnDisco

### Community 76 - "UsuarioFijo"
Cohesion: 0.16
Nodes (4): ICifradoDeDatos, UsuarioFijo, CifradoDePrueba, SeguimientoClienteHandlersTests

### Community 77 - "dependencies"
Cohesion: 0.33
Nodes (6): dependencies, firebase@^13.2.0, next@16.4.5, react@19.3.0, react-dom@19.3.0, react-dom

### Community 78 - "scripts"
Cohesion: 0.25
Nodes (8): scripts, build, dev, lint, start, test, test:e2e, typecheck

### Community 79 - "orden-detalle.test.tsx"
Cohesion: 0.36
Nodes (10): json(), me(), mocks, orden, PERMISOS_ESTADO, stubApi(), stubCambioDeEstado(), stubDatosTecnicos() (+2 more)

### Community 80 - "ReparacionesConfiguraciones.cs"
Cohesion: 0.29
Nodes (3): ClienteConfiguration, RolConfiguration, PuntoIOS.Infrastructure.Persistence.Configurations

### Community 81 - "@playwright/test"
Cohesion: 0.29
Nodes (3): CORS, @playwright/test@^1.64.1, @playwright/test

### Community 82 - "Session state (auto-saved — last known reality)"
Cohesion: 0.40
Nodes (4): Recent commits, Resume, Session state (auto-saved — last known reality), Uncommitted changes

### Community 84 - "useAuth"
Cohesion: 0.11
Nodes (19): nextConfig, ClientesRoute(), PanelPage(), Detalle(), OrdenRoute(), ReparacionesRoute(), metadata, useAuth() (+11 more)

### Community 86 - "eslint.config.mjs"
Cohesion: 0.33
Nodes (5): eslintConfig, eslint@^9, eslint-config-next@16.4.5, eslint, eslint-config-next

### Community 88 - "DependencyInjection.AddInfrastructure"
Cohesion: 0.12
Nodes (3): ServiceCollectionExtensions, RegistrarUsuarioHandler, DependencyInjection

### Community 90 - "TiposDeReparacion.cs"
Cohesion: 0.29
Nodes (3): EstadosDeReparacion, TiposDeDesbloqueo, TiposDeReparacion

### Community 92 - "ArgumentException"
Cohesion: 0.22
Nodes (4): ItemDePresupuesto, TiposDeItemPresupuesto, VersionDePresupuestoTests, ItemsInvalidos

### Community 95 - "IDirectorioDeUsuarios"
Cohesion: 0.24
Nodes (9): HistorialDispositivoHandler, HistorialOrdenHandler, ListarOrdenesHandler, ListarTecnicosHandler, ObtenerOrdenHandler, CrearVersionPresupuestoHandler, ListarFotosHandler, ListarVersionesPresupuestoHandler (+1 more)

### Community 96 - "ClienteRepoEnMemoria"
Cohesion: 0.50
Nodes (3): ClienteRepoEnMemoria, Clientes, Guardados

### Community 100 - "package.json"
Cohesion: 0.20
Nodes (9): tailwindcss@^4, @types/react-dom@^19, @vitejs/plugin-react@^6.2.1, name, private, version, tailwindcss, @types/react-dom (+1 more)

### Community 101 - "ListarOrdenesHandlerTests"
Cohesion: 0.25
Nodes (5): FiltroDeOrdenes, AlmacenEnMemoria, Archivos, DirectorioFijo, ListarOrdenesHandlerTests

## Knowledge Gaps
- **183 isolated node(s):** `Id`, `LocalId`, `Numero`, `ClienteId`, `DispositivoId` (+178 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 534 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **24 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `OrdenReparacion` connect `OrdenReparacion` to `Guid`, `MaquinaDeEstadosTests`, `RecibirEquipoHandler.HandleAsync`, `DateTimeOffset`, `Task`, `Guid`, `List`, `PuntoIOS.Domain.Reparaciones`, `Fact`, `CambiarEstadoOrdenHandlerTests`, `EntityTypeBuilder`?**
  _High betweenness centrality (0.032) - this node is a cross-community bridge._
- **What connects `Id`, `LocalId`, `Numero` to the rest of the system?**
  _183 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Punto iOS — Software Design Document (SDD)` be split into smaller, more focused modules?**
  _Cohesion score 0.09090909090909091 - nodes in this community are weakly interconnected._
- **Why does `PuntoIosDbContext` connect `PuntoIosDbContext` to `OrdenRepoEnMemoria`, `Usuario.Registrar`, `VersionDePresupuesto`, `CancellationToken`, `PermisoConfiguration`, `TestApi.CrearAsync`, `Guid`, `ReparacionesConfiguraciones.cs`, `Task`, `RegistroAuditoria`, `PuntoIOS.Domain.Reparaciones`, `DependencyInjection.AddInfrastructure`, `Guid`, `EntityTypeBuilder`, `IAuditoriaRepositorio`?**
  _High betweenness centrality (0.031) - this node is a cross-community bridge._
- **Should `PuntoIOS.ArchitectureTests.csproj` be split into smaller, more focused modules?**
  _Cohesion score 0.059800664451827246 - nodes in this community are weakly interconnected._
- **Why does `PuntoIOS.Domain.Identidad` connect `ReparacionesEndpointTests.cs` to `CancellationToken`, `IdentificarUsuarioHandler.cs`, `DateTimeOffset`, `DispositivosController.cs`, `Task`, `ReparacionesConfiguraciones.cs`, `ServiceCollectionExtensions.cs`, `Permiso`, `PuntoIOS.Domain.Reparaciones`, `puntoios_application_common_pagination`, `Rol`?**
  _High betweenness centrality (0.025) - this node is a cross-community bridge._
- **Should `3. Fases` be split into smaller, more focused modules?**
  _Cohesion score 0.09523809523809523 - nodes in this community are weakly interconnected._