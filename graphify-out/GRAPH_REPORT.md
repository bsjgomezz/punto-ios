# Graph Report - Punto iOS  (2026-11-10)

## Corpus Check
- 255 files · ~79,274 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 8 file(s) not represented in the graph (top: (none) 5, .example 1, .ico 1)

## Summary
- 2065 nodes · 6547 edges · 103 communities (74 shown, 29 thin omitted)
- Extraction: 92% EXTRACTED · 8% INFERRED · 0% AMBIGUOUS · INFERRED: 527 edges (avg confidence: 0.84)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `7588f707`
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
- Usuario
- PuntoIosDbContext
- PuntoIOS.Application/AssemblyMarker.cs
- PuntoIOS.Domain/AssemblyMarker.cs
- ServiceCollectionExtensions.cs
- auth-provider.tsx
- seguimiento-cliente.tsx
- TestApi.cs
- PagedResult
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
- TestApi.CrearAsync
- reparaciones/api.ts
- IdentificarUsuarioHandler.cs
- Dispositivo
- MaquinaDeEstadosTests
- ClientesEndpointTests
- LocalUnico
- DateTimeOffset
- OrdenReparacion
- Cliente
- cliente-detalle.tsx
- DispositivosController.cs
- Guid
- List
- LayerDependencyTests
- RegistroAuditoria
- RelojFijo
- HttpClient
- ResponderPresupuestoHandlerTests
- PuntoIOS.Domain.Auditoria
- PuntoIOS.Domain.Reparaciones
- devDependencies
- IdentificarUsuarioHandler
- Guid
- PuntoIOS.Application.Features.Identidad.ObtenerUsuarioActual
- EntityTypeBuilder
- login-form.tsx
- RecibirEquipoHandler
- DatosTecnicosEndpointTests.OrdenAsync
- CrearClienteHandlerTests
- InlineData
- Usuario.Registrar
- vitest
- ObtenerUsuarioActualHandler
- CrearVersionPresupuestoHandlerTests
- PageRequest
- CurrentUser
- RecibirEquipoHandler.HandleAsync
- OrdenReparacionDto
- microsoft_entityframeworkcore
- Stream
- CodigoSeguimiento.Generar
- dependencies
- scripts
- orden-detalle.test.tsx
- RolConfiguration
- @playwright/test
- Session state (auto-saved — last known reality)
- DotEnv
- useAuth
- eslint.config.mjs
- Permiso
- @testing-library/jest-dom
- TiposDeReparacion.cs
- IDirectorioDeUsuarios.ListarActivosConPermisoAsync
- ArgumentException
- Theory
- EmailValidator.cs
- @types/react@^19
- CrearClienteHandler.HandleAsync
- RecibirEquipoRequest
- @types/react-dom@^19
- reparaciones/api.ts
- PermisoConfiguration
- vitest.config.mts

## God Nodes (most connected - your core abstractions)
1. `OrdenReparacionDto` - 50 edges
2. `PuntoIosDbContext` - 43 edges
3. `OrdenReparacion` - 42 edges
4. `OrdenDetalle()` - 42 edges
5. `Usuario` - 39 edges
6. `VersionDePresupuesto` - 39 edges
7. `PuntoIOS.Domain.Auditoria` - 36 edges
8. `PuntoIOS.Domain.Identidad` - 34 edges
9. `ReparacionesController` - 32 edges
10. `Cliente` - 31 edges

## Surprising Connections (you probably didn't know these)
- `TestApi` --references--> `Program`  [EXTRACTED]
  backend/tests/PuntoIOS.IntegrationTests/TestApi.cs → backend/src/PuntoIOS.Api/Program.cs
- `ReparacionesController` --references--> `ListarVersionesPresupuestoHandler`  [EXTRACTED]
  backend/src/PuntoIOS.Api/Controllers/ReparacionesController.cs → backend/src/PuntoIOS.Application/Features/Reparaciones/ListarVersionesPresupuesto/ListarVersionesPresupuestoHandler.cs
- `AutorizacionTests` --references--> `PostgresFixture`  [EXTRACTED]
  backend/tests/PuntoIOS.IntegrationTests/AutorizacionTests.cs → backend/tests/PuntoIOS.IntegrationTests/PostgresFixture.cs
- `MeEndpointTests` --references--> `PostgresFixture`  [EXTRACTED]
  backend/tests/PuntoIOS.IntegrationTests/MeEndpointTests.cs → backend/tests/PuntoIOS.IntegrationTests/PostgresFixture.cs
- `UsuariosController` --references--> `ActualizarUsuarioHandler`  [EXTRACTED]
  backend/src/PuntoIOS.Api/Controllers/UsuariosController.cs → backend/src/PuntoIOS.Application/Features/Identidad/ActualizarUsuario/ActualizarUsuarioHandler.cs

## Import Cycles
- None detected.

## Communities (103 total, 29 thin omitted)

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
Cohesion: 0.21
Nodes (5): RecibirEquipoCommand, ChecklistIngreso, ItemChecklist, ItemsDeChecklist, ChecklistYFotosTests

### Community 5 - "Dispositivo"
Cohesion: 0.12
Nodes (14): Dispositivo, Almacenamiento, ClienteId, Color, CreadoEn, CreadoPorUsuarioId, Id, ImeiSerie (+6 more)

### Community 6 - "use-remote-checks.ts"
Cohesion: 0.15
Nodes (14): Home(), REMOTE_CHECKS, RemoteCheckId, Row, RowState, StatusBoard(), subscribeToConnection(), useOnline() (+6 more)

### Community 7 - "RecibirEquipoHandlerTests"
Cohesion: 0.12
Nodes (9): ICifradoDeDatos, DesbloqueoDto, LocalFijo, LocalId, UsuarioFijo, CifradoDePrueba, SeguimientoClienteHandlersTests, RecibirEquipoHandlerTests (+1 more)

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

### Community 15 - "Usuario"
Cohesion: 0.08
Nodes (21): ActualizarUsuarioHandler, IdentificarUsuarioHandler, IRolRepositorio, IUsuarioRepositorio, Usuario, Activo, CreadoEn, Email (+13 more)

### Community 16 - "PuntoIosDbContext"
Cohesion: 0.11
Nodes (15): DesignTimeDbContextFactory, InicializadorBaseDeDatos, PuntoIosDbContext, Auditoria, Clientes, Dispositivos, FotosOrden, LocalActualId (+7 more)

### Community 19 - "ServiceCollectionExtensions.cs"
Cohesion: 0.07
Nodes (5): ResponderPresupuestoRequest, PuntoIOS.Application.Features.Reparaciones.ListarVersionesPresupuesto, PuntoIOS.Application.Features.Reparaciones.ResponderPresupuesto, PuntoIOS.Api.Seguridad, PuntoIOS.Application.Features.Clientes.SeguimientoCliente

### Community 20 - "auth-provider.tsx"
Cohesion: 0.17
Nodes (12): body, display, metadata, RootLayout(), viewport, AuthContext, AuthContextValue, AuthProvider() (+4 more)

### Community 22 - "seguimiento-cliente.tsx"
Cohesion: 0.19
Nodes (19): formatoFecha, Props, SeguimientoCliente(), conToken(), enviarPorWhatsapp(), generar(), ver(), ana (+11 more)

### Community 23 - "TestApi.cs"
Cohesion: 0.11
Nodes (15): ListarUsuariosHandler, Permisos, RolesDeSistema, PuntoIOS.Application.Common.Results, PuntoIOS.Api.Controllers, PuntoIOS.IntegrationTests, PuntoIOS.UnitTests.Identidad, PuntoIOS.Application.Features.Identidad.ActualizarUsuario (+7 more)

### Community 26 - "CambiarEstadoOrdenHandlerTests"
Cohesion: 0.24
Nodes (4): ActualizarDatosTecnicosCommand, CambiarEstadoOrdenCommand, CambiarEstadoOrdenHandlerTests, ActualizarDatosTecnicosHandlerTests

### Community 27 - "Resultado"
Cohesion: 0.17
Nodes (10): ResultadoExtensions, Error, Resultado, Error, EsExitoso, Valor, TipoError, Conflicto (+2 more)

### Community 29 - "clientes/api.ts"
Cohesion: 0.07
Nodes (46): actualizarCliente(), Cliente, Coincidencia, crearCliente(), DatosCliente, EquipoNuevo, NuevoCliente, Pagina (+38 more)

### Community 30 - "TestApi.cs"
Cohesion: 0.09
Nodes (4): Program, AssemblyMarker, PuntoIOS.Infrastructure, PuntoIOS.Api.Extensions

### Community 31 - "Rol"
Cohesion: 0.11
Nodes (11): Rol, Descripcion, EsDeSistema, Id, Nombre, Permisos, RolPermiso, PermisoCodigo (+3 more)

### Community 32 - "DispositivoDto"
Cohesion: 0.26
Nodes (7): CrearClienteRequest, EquipoNuevoRequest, ActualizarDispositivoRequest, CrearDispositivoRequest, DispositivoDto, ImeiDto, DispositivosEndpointTests

### Community 33 - "OrdenRepoEnMemoria"
Cohesion: 0.11
Nodes (9): FiltroDeOrdenes, FotoOrdenConfiguration, OrdenReparacionRepositorio, OrdenRepoEnMemoria, Fotos, Guardados, Ordenes, VersionesPresupuesto (+1 more)

### Community 34 - "VersionDePresupuesto"
Cohesion: 0.13
Nodes (12): VersionDePresupuesto, CreadoEn, CreadoPorUsuarioId, Id, Items, LocalId, Numero, OrdenId (+4 more)

### Community 35 - "TestApi.CrearAsync"
Cohesion: 0.15
Nodes (4): RegistrarUsuarioRequest, MeEndpointTests, SeguimientoClienteEndpointTests, UsuariosEndpointTests

### Community 36 - "reparaciones/api.ts"
Cohesion: 0.07
Nodes (44): CambioDeCampo, actualizarDatosTecnicos(), cambiarEstadoDeOrden(), crearVersionPresupuesto(), DatosTecnicos, formatearMonto(), Foto, ItemPresupuesto (+36 more)

### Community 37 - "IdentificarUsuarioHandler.cs"
Cohesion: 0.28
Nodes (8): EstadoIdentificacion, Autorizado, Desactivado, NoRegistrado, ResultadoIdentificacion, Desactivado, NoRegistrado, UsuarioAutorizado

### Community 38 - "Dispositivo"
Cohesion: 0.19
Nodes (4): DispositivoRepositorio, DispositivoRepoEnMemoria, Dispositivos, Guardados

### Community 41 - "LocalUnico"
Cohesion: 0.17
Nodes (10): ILocalActual, LocalId, Local, Id, Nombre, LocalesDeSistema, LocalUnico, LocalId (+2 more)

### Community 42 - "DateTimeOffset"
Cohesion: 0.06
Nodes (22): OrdenResumenDto, FotoOrden, CreadoEn, CreadoPorUsuarioId, Id, LocalId, OrdenId, Ruta (+14 more)

### Community 43 - "OrdenReparacion"
Cohesion: 0.07
Nodes (26): OrdenReparacion, ChecklistItems, ClienteId, ComponentesAfectados, CreadoEn, CreadoPorUsuarioId, DesbloqueoCifrado, DesbloqueoTipo (+18 more)

### Community 44 - "Cliente"
Cohesion: 0.10
Nodes (18): ActualizarClienteCommand, Cliente, Apellido, CodigoSeguimientoCifrado, CodigoSeguimientoGeneradoEn, CodigoSeguimientoHuella, CreadoEn, CreadoPorUsuarioId (+10 more)

### Community 45 - "cliente-detalle.tsx"
Cohesion: 0.12
Nodes (15): IngresarPage(), BackLink(), historialDeCliente(), obtenerCliente(), RegistroHistorial, ACCIONES, ClienteDetalle(), Datos (+7 more)

### Community 46 - "DispositivosController.cs"
Cohesion: 0.19
Nodes (9): HistorialDeAuditoria, PuntoIOS.Application.Features.Dispositivos.HistorialDispositivo, PuntoIOS.Application.Features.Dispositivos.CrearDispositivo, PuntoIOS.Application.Features.Dispositivos, PuntoIOS.Application.Features.Dispositivos.ActualizarDispositivo, PuntoIOS.Application.Features.Dispositivos.ListarDispositivos, PuntoIOS.Application.Features.Dispositivos.RevelarImei, PuntoIOS.Application.Features.Auditoria (+1 more)

### Community 47 - "Guid"
Cohesion: 0.15
Nodes (7): RegistroHistorialDto, IAuditoriaRepositorio, ListarRolesHandler, UsuarioActualDto, RolDto, VersionDePresupuestoDto, DirectorioFijo

### Community 48 - "List"
Cohesion: 0.41
Nodes (4): CrearVersionPresupuestoRequest, ItemPresupuestoRequest, ItemPresupuestoDto, PresupuestoEndpointTests

### Community 50 - "RegistroAuditoria"
Cohesion: 0.12
Nodes (15): AccionesAuditadas, EntidadesAuditadas, RegistroAuditoria, Accion, Cambios, Entidad, EntidadId, Fecha (+7 more)

### Community 51 - "RelojFijo"
Cohesion: 0.30
Nodes (5): ActualizarDispositivoCommand, CrearDispositivoCommand, DispositivoHandlersTests, RelojFijo, Ahora

### Community 54 - "PuntoIOS.Domain.Auditoria"
Cohesion: 0.15
Nodes (10): PuntoIOS.Application.Features.Reparaciones.Puertos, PuntoIOS.Application.Features.Clientes.ActualizarCliente, PuntoIOS.Application.Common.Archivos, PuntoIOS.Application.Features.Dispositivos.Puertos, PuntoIOS.Application.Features.Clientes, PuntoIOS.Application.Features.Auditoria.Puertos, PuntoIOS.Application.Features.Clientes.HistorialCliente, PuntoIOS.Domain.Clientes (+2 more)

### Community 55 - "PuntoIOS.Domain.Reparaciones"
Cohesion: 0.12
Nodes (11): PuntoIOS.Application.Features.Reparaciones.CrearVersionPresupuesto, PuntoIOS.UnitTests.Clientes, PuntoIOS.Application.Features.Reparaciones.ConsultarOrdenes, PuntoIOS.Application.Features.Reparaciones.VerDesbloqueo, PuntoIOS.Application.Features.Reparaciones.RecibirEquipo, PuntoIOS.Application.Features.Reparaciones.ActualizarDatosTecnicos, PuntoIOS.Application.Features.Reparaciones.CambiarEstado, PuntoIOS.Application.Features.Clientes.CrearCliente (+3 more)

### Community 56 - "devDependencies"
Cohesion: 0.15
Nodes (13): devDependencies, jsdom@^30.1.3, tailwindcss@^4, @tailwindcss/turbopack@^4, @testing-library/react@^16.3.3, @types/node@^24.19.2, typescript@^5, vitest@^5.1.0 (+5 more)

### Community 57 - "IdentificarUsuarioHandler"
Cohesion: 0.11
Nodes (7): FirebaseClaims, HttpContextCurrentUserAccessor, UsuarioLocalClaims, ICurrentUserAccessor, PuntoIOS.Application.Features.Identidad.IdentificarUsuario, PuntoIOS.Application.Common.Security, PuntoIOS.Api.Authentication

### Community 58 - "Guid"
Cohesion: 0.05
Nodes (14): ClientesController, DispositivosController, MeController, ReparacionesController, TipoReparacionDto, RolesController, UsuariosController, ServiceCollectionExtensions (+6 more)

### Community 59 - "PuntoIOS.Application.Features.Identidad.ObtenerUsuarioActual"
Cohesion: 0.16
Nodes (6): PagedResult, TotalPages, PagedResultTests, PuntoIOS.Application.Common.Pagination, PuntoIOS.UnitTests, PuntoIOS.Application.Features.Identidad.ObtenerUsuarioActual

### Community 60 - "EntityTypeBuilder"
Cohesion: 0.23
Nodes (6): ClienteConfiguration, DispositivoConfiguration, LocalConfiguration, UsuarioConfiguration, OrdenReparacionConfiguration, VersionDePresupuestoConfiguration

### Community 61 - "login-form.tsx"
Cohesion: 0.19
Nodes (12): OjoAbierto(), OjoCerrado(), PasswordInput(), Props, describeAuthError(), LoginForm(), onGoogle(), onSubmit() (+4 more)

### Community 62 - "RecibirEquipoHandler"
Cohesion: 0.09
Nodes (29): CifradoConDataProtection, ActualizarClienteHandler, CrearClienteHandler, HistorialClienteHandler, ConsultarCodigoSeguimientoHandler, GenerarCodigoSeguimientoHandler, ActualizarDispositivoHandler, CrearDispositivoHandler (+21 more)

### Community 66 - "Usuario.Registrar"
Cohesion: 0.33
Nodes (3): ActualizarUsuarioCommand, RegistrarUsuarioCommand, GestionDeUsuariosTests

### Community 67 - "vitest"
Cohesion: 0.08
Nodes (21): mocks, mocks, ana, Handler, mocks, ana, iphone, json() (+13 more)

### Community 68 - "ObtenerUsuarioActualHandler"
Cohesion: 0.54
Nodes (3): ObtenerUsuarioActualHandler, FakeAccessor, ObtenerUsuarioActualHandlerTests

### Community 69 - "CrearVersionPresupuestoHandlerTests"
Cohesion: 0.46
Nodes (3): CrearVersionPresupuestoCommand, ItemPresupuestoInput, CrearVersionPresupuestoHandlerTests

### Community 70 - "PageRequest"
Cohesion: 0.29
Nodes (5): PageRequest, Page, PageSize, Skip, PageRequestTests

### Community 72 - "RecibirEquipoHandler.HandleAsync"
Cohesion: 0.17
Nodes (4): ClienteResumenDto, DispositivoValidacion, ArmadorDeOrdenes, CambioDeCampo

### Community 73 - "OrdenReparacionDto"
Cohesion: 0.21
Nodes (5): CambiarEstadoRequest, ChecklistDto, OrdenReparacionDto, CambiarEstadoEndpointTests, ResponderPresupuestoEndpointTests

### Community 74 - "microsoft_entityframeworkcore"
Cohesion: 0.14
Nodes (6): BuscarClientesHandler, PuntoIOS.Infrastructure.Persistence.Configurations, PuntoIOS.Application.Features.Clientes.Puertos, PuntoIOS.Application.Features.Clientes.BuscarClientes, PuntoIOS.Infrastructure.Persistence, PuntoIOS.Application.Features.Clientes.ObtenerCliente

### Community 75 - "Stream"
Cohesion: 0.20
Nodes (6): IAlmacenDeArchivos, ArchivoDeFoto, ObtenerFotoHandler, AlmacenEnDisco, AlmacenEnMemoria, Archivos

### Community 76 - "CodigoSeguimiento.Generar"
Cohesion: 0.18
Nodes (3): FormatoDeImagen, CodigoSeguimiento, CodigoSeguimientoTests

### Community 77 - "dependencies"
Cohesion: 0.22
Nodes (10): dependencies, firebase@^13.2.0, next@16.4.5, react@19.3.0, react-dom@19.3.0, name, private, version (+2 more)

### Community 78 - "scripts"
Cohesion: 0.25
Nodes (8): scripts, build, dev, lint, start, test, test:e2e, typecheck

### Community 79 - "orden-detalle.test.tsx"
Cohesion: 0.38
Nodes (9): json(), me(), mocks, orden, PERMISOS_ESTADO, stubApi(), stubCambioDeEstado(), stubDatosTecnicos() (+1 more)

### Community 81 - "@playwright/test"
Cohesion: 0.29
Nodes (3): CORS, @playwright/test@^1.64.1, @playwright/test

### Community 82 - "Session state (auto-saved — last known reality)"
Cohesion: 0.40
Nodes (4): Recent commits, Resume, Session state (auto-saved — last known reality), Uncommitted changes

### Community 84 - "useAuth"
Cohesion: 0.08
Nodes (36): nextConfig, ClienteRoute(), Detalle(), ClientesRoute(), PanelPage(), Detalle(), OrdenRoute(), ReparacionesRoute() (+28 more)

### Community 86 - "eslint.config.mjs"
Cohesion: 0.33
Nodes (5): eslintConfig, eslint@^9, eslint-config-next@16.4.5, eslint, eslint-config-next

### Community 88 - "Permiso"
Cohesion: 0.50
Nodes (3): Permiso, Codigo, Descripcion

### Community 90 - "TiposDeReparacion.cs"
Cohesion: 0.29
Nodes (3): EstadosDeReparacion, TiposDeDesbloqueo, TiposDeReparacion

### Community 91 - "IDirectorioDeUsuarios.ListarActivosConPermisoAsync"
Cohesion: 0.53
Nodes (3): IDirectorioDeUsuarios, UsuarioResumen, DirectorioDeUsuarios

### Community 92 - "ArgumentException"
Cohesion: 0.22
Nodes (4): ItemDePresupuesto, TiposDeItemPresupuesto, VersionDePresupuestoTests, ItemsInvalidos

### Community 96 - "CrearClienteHandler.HandleAsync"
Cohesion: 0.14
Nodes (7): ClienteValidacion, ObtenerClienteHandler, IClienteRepositorio, ClienteRepositorio, ClienteRepoEnMemoria, Clientes, Guardados

### Community 97 - "RecibirEquipoRequest"
Cohesion: 0.40
Nodes (3): ChecklistRequest, RecibirEquipoRequest, ChecklistYFotosEndpointTests

### Community 102 - "reparaciones/api.ts"
Cohesion: 0.12
Nodes (23): Dispositivo, AVISO_FECHA_ESTIMADA, Checklist, CHECKLIST_CATALOGO, ESTADOS, ITEM_ENCIENDE, listarTecnicos(), NOMBRES_DESBLOQUEO (+15 more)

## Knowledge Gaps
- **182 isolated node(s):** `Microsoft.EntityFrameworkCore.Design (10.0.14)`, `Microsoft.EntityFrameworkCore.Design (10.0.14)`, `coverlet.collector (6.0.4)`, `Microsoft.NET.Test.Sdk (17.14.1)`, `xunit (2.9.3)` (+177 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 523 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **29 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `OrdenReparacion` connect `OrdenReparacion` to `OrdenRepoEnMemoria`, `MaquinaDeEstadosTests`, `DateTimeOffset`, `Guid`, `List`, `PuntoIOS.Domain.Reparaciones`, `Fact`, `CambiarEstadoOrdenHandlerTests`?**
  _High betweenness centrality (0.033) - this node is a cross-community bridge._
- **What connects `Microsoft.EntityFrameworkCore.Design (10.0.14)`, `Microsoft.EntityFrameworkCore.Design (10.0.14)`, `coverlet.collector (6.0.4)` to the rest of the system?**
  _182 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Punto iOS — Software Design Document (SDD)` be split into smaller, more focused modules?**
  _Cohesion score 0.09090909090909091 - nodes in this community are weakly interconnected._
- **Why does `Usuario` connect `Usuario` to `Usuario.Registrar`, `CurrentUser`, `DateTimeOffset`, `Guid`, `PagedResult`, `Rol`?**
  _High betweenness centrality (0.020) - this node is a cross-community bridge._
- **Should `PuntoIOS.ArchitectureTests.csproj` be split into smaller, more focused modules?**
  _Cohesion score 0.059800664451827246 - nodes in this community are weakly interconnected._
- **Why does `PuntoIosDbContext` connect `PuntoIosDbContext` to `CrearClienteHandler.HandleAsync`, `OrdenRepoEnMemoria`, `VersionDePresupuesto`, `Dispositivo`, `PermisoConfiguration`, `microsoft_entityframeworkcore`, `DateTimeOffset`, `Guid`, `RolConfiguration`, `Usuario`, `RegistroAuditoria`, `List`, `HttpClient`, `IDirectorioDeUsuarios.ListarActivosConPermisoAsync`, `EntityTypeBuilder`, `RecibirEquipoHandler`?**
  _High betweenness centrality (0.019) - this node is a cross-community bridge._
- **Should `3. Fases` be split into smaller, more focused modules?**
  _Cohesion score 0.09523809523809523 - nodes in this community are weakly interconnected._