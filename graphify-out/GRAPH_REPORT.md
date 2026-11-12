# Graph Report - Punto iOS  (2026-11-12)

## Corpus Check
- 257 files · ~82,878 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 8 file(s) not represented in the graph (top: (none) 5, .example 1, .ico 1)

## Summary
- 2083 nodes · 6644 edges · 110 communities (74 shown, 36 thin omitted)
- Extraction: 92% EXTRACTED · 8% INFERRED · 0% AMBIGUOUS · INFERRED: 533 edges (avg confidence: 0.84)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `57b05a9b`
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
- ReparacionesEndpointTests.cs
- PuntoIOS.Domain.Auditoria
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
- Task
- CrearVersionPresupuestoHandlerTests
- HttpClient
- CurrentUser
- RecibirEquipoHandler.HandleAsync
- OrdenReparacionDto
- equipos-cliente.tsx
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
- DependencyInjection.AddInfrastructure
- @testing-library/jest-dom
- TiposDeReparacion.cs
- ActualizarClienteHandlerTests
- ArgumentException
- Theory
- OrdenReparacion.Recibir
- IDirectorioDeUsuarios
- Cliente
- RecibirEquipoRequest
- CambiarEstadoOrdenHandlerTests
- Roles.Administrador
- package.json
- ListarOrdenesHandlerTests
- CifradoConDataProtection
- PermisoConfiguration
- IAuditoriaRepositorio.cs
- UsuarioConfiguration
- [codigo]/page.tsx

## God Nodes (most connected - your core abstractions)
1. `OrdenReparacionDto` - 52 edges
2. `OrdenDetalle()` - 44 edges
3. `PuntoIosDbContext` - 42 edges
4. `VersionDePresupuesto` - 41 edges
5. `OrdenReparacion` - 41 edges
6. `Usuario` - 39 edges
7. `PuntoIOS.Domain.Identidad` - 38 edges
8. `PuntoIOS.Domain.Auditoria` - 36 edges
9. `ReparacionesController` - 32 edges
10. `Cliente` - 31 edges

## Surprising Connections (you probably didn't know these)
- `TestApi` --references--> `Program`  [EXTRACTED]
  backend/tests/PuntoIOS.IntegrationTests/TestApi.cs → backend/src/PuntoIOS.Api/Program.cs
- `ReparacionesController` --references--> `CambiarEstadoOrdenHandler`  [EXTRACTED]
  backend/src/PuntoIOS.Api/Controllers/ReparacionesController.cs → backend/src/PuntoIOS.Application/Features/Reparaciones/CambiarEstado/CambiarEstadoOrdenHandler.cs
- `ReparacionesController` --references--> `ListarVersionesPresupuestoHandler`  [EXTRACTED]
  backend/src/PuntoIOS.Api/Controllers/ReparacionesController.cs → backend/src/PuntoIOS.Application/Features/Reparaciones/ListarVersionesPresupuesto/ListarVersionesPresupuestoHandler.cs
- `ReparacionesController` --references--> `ResponderPresupuestoHandler`  [EXTRACTED]
  backend/src/PuntoIOS.Api/Controllers/ReparacionesController.cs → backend/src/PuntoIOS.Application/Features/Reparaciones/ResponderPresupuesto/ResponderPresupuestoHandler.cs
- `RegistroHistorialDto` --references--> `CambioDeCampo`  [EXTRACTED]
  backend/src/PuntoIOS.Application/Features/Auditoria/HistorialDeAuditoria.cs → backend/src/PuntoIOS.Domain/Auditoria/RegistroAuditoria.cs

## Import Cycles
- None detected.

## Communities (110 total, 36 thin omitted)

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
Cohesion: 0.24
Nodes (4): FormatoDeImagen, ChecklistIngreso, ItemChecklist, ChecklistYFotosTests

### Community 5 - "Dispositivo"
Cohesion: 0.11
Nodes (14): Dispositivo, Almacenamiento, ClienteId, Color, CreadoEn, CreadoPorUsuarioId, Id, ImeiSerie (+6 more)

### Community 6 - "use-remote-checks.ts"
Cohesion: 0.15
Nodes (14): Home(), REMOTE_CHECKS, RemoteCheckId, Row, RowState, StatusBoard(), subscribeToConnection(), useOnline() (+6 more)

### Community 7 - "RecibirEquipoHandlerTests"
Cohesion: 0.15
Nodes (8): ICifradoDeDatos, RecibirEquipoCommand, DesbloqueoDto, AuditoriaRepoEnMemoria, Registros, CifradoDePrueba, RecibirEquipoHandlerTests, Invalidos

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
Cohesion: 0.10
Nodes (17): IdentificarUsuarioHandler, IUsuarioRepositorio, Usuario, Activo, CreadoEn, Email, FirebaseUid, Id (+9 more)

### Community 16 - "PuntoIosDbContext"
Cohesion: 0.11
Nodes (15): DesignTimeDbContextFactory, InicializadorBaseDeDatos, PuntoIosDbContext, Auditoria, Clientes, Dispositivos, FotosOrden, LocalActualId (+7 more)

### Community 19 - "ServiceCollectionExtensions.cs"
Cohesion: 0.08
Nodes (4): PuntoIOS.Application.Features.Reparaciones.ListarVersionesPresupuesto, PuntoIOS.Application.Features.Reparaciones.ResponderPresupuesto, PuntoIOS.Application.Features.Reparaciones.CambiarEstado, PuntoIOS.Api.Seguridad

### Community 20 - "auth-provider.tsx"
Cohesion: 0.17
Nodes (12): body, display, metadata, RootLayout(), viewport, AuthContext, AuthContextValue, AuthProvider() (+4 more)

### Community 22 - "seguimiento-cliente.tsx"
Cohesion: 0.18
Nodes (20): formatoFecha, Props, SeguimientoCliente(), conToken(), enviarPorWhatsapp(), generar(), ver(), ana (+12 more)

### Community 23 - "TestApi.cs"
Cohesion: 0.07
Nodes (21): RolesController, BuscarClientesHandler, ObtenerClienteHandler, ListarRolesHandler, Permiso, Codigo, Descripcion, Permisos (+13 more)

### Community 24 - "PagedResult"
Cohesion: 0.22
Nodes (4): HistorialDeAuditoria, RegistroHistorialDto, ListarUsuariosHandler, OrdenResumenDto

### Community 27 - "Resultado"
Cohesion: 0.17
Nodes (10): ResultadoExtensions, Error, Resultado, Error, EsExitoso, Valor, TipoError, Conflicto (+2 more)

### Community 29 - "clientes/api.ts"
Cohesion: 0.09
Nodes (36): CambioDeCampo, Cliente, Coincidencia, crearCliente(), DatosCliente, EquipoNuevo, NuevoCliente, Pagina (+28 more)

### Community 30 - "TestApi.cs"
Cohesion: 0.06
Nodes (9): FirebaseClaims, HttpContextCurrentUserAccessor, UsuarioLocalClaims, Program, AssemblyMarker, PuntoIOS.Application.Features.Identidad.IdentificarUsuario, PuntoIOS.Infrastructure, PuntoIOS.Api.Authentication (+1 more)

### Community 31 - "Rol"
Cohesion: 0.15
Nodes (9): Rol, Descripcion, EsDeSistema, Id, Nombre, Permisos, RolPermiso, PermisoCodigo (+1 more)

### Community 32 - "DispositivoDto"
Cohesion: 0.26
Nodes (7): CrearClienteRequest, EquipoNuevoRequest, ActualizarDispositivoRequest, CrearDispositivoRequest, DispositivoDto, ImeiDto, DispositivosEndpointTests

### Community 33 - "OrdenRepoEnMemoria"
Cohesion: 0.11
Nodes (10): ListarFotosHandler, IOrdenReparacionRepositorio, FotoOrdenConfiguration, OrdenReparacionConfiguration, OrdenReparacionRepositorio, OrdenRepoEnMemoria, Fotos, Guardados (+2 more)

### Community 34 - "VersionDePresupuesto"
Cohesion: 0.11
Nodes (15): ResponderPresupuestoCommand, VersionDePresupuesto, CreadoEn, CreadoPorUsuarioId, Id, Items, LocalId, Numero (+7 more)

### Community 35 - "TestApi.CrearAsync"
Cohesion: 0.12
Nodes (5): ActualizarUsuarioRequest, RegistrarUsuarioRequest, MeEndpointTests, SeguimientoClienteEndpointTests, UsuariosEndpointTests

### Community 36 - "reparaciones/api.ts"
Cohesion: 0.06
Nodes (70): Dispositivo, actualizarDatosTecnicos(), AVISO_FECHA_ESTIMADA, cambiarEstadoDeOrden(), Checklist, CHECKLIST_CATALOGO, crearVersionPresupuesto(), DatosTecnicos (+62 more)

### Community 37 - "IdentificarUsuarioHandler.cs"
Cohesion: 0.24
Nodes (8): EstadoIdentificacion, Autorizado, Desactivado, NoRegistrado, ResultadoIdentificacion, Desactivado, NoRegistrado, UsuarioAutorizado

### Community 38 - "Dispositivo"
Cohesion: 0.21
Nodes (4): DispositivoRepositorio, DispositivoRepoEnMemoria, Dispositivos, Guardados

### Community 41 - "LocalUnico"
Cohesion: 0.17
Nodes (10): ILocalActual, LocalId, Local, Id, Nombre, LocalesDeSistema, LocalUnico, LocalId (+2 more)

### Community 42 - "DateTimeOffset"
Cohesion: 0.06
Nodes (23): FotoOrden, CreadoEn, CreadoPorUsuarioId, Id, LocalId, OrdenId, Ruta, Tamano (+15 more)

### Community 43 - "OrdenReparacion"
Cohesion: 0.07
Nodes (26): OrdenReparacion, ChecklistItems, ClienteId, ComponentesAfectados, CreadoEn, CreadoPorUsuarioId, DesbloqueoCifrado, DesbloqueoTipo (+18 more)

### Community 44 - "Cliente"
Cohesion: 0.11
Nodes (16): Cliente, Apellido, CodigoSeguimientoCifrado, CodigoSeguimientoGeneradoEn, CodigoSeguimientoHuella, CreadoEn, CreadoPorUsuarioId, Direccion (+8 more)

### Community 45 - "cliente-detalle.tsx"
Cohesion: 0.12
Nodes (16): ClienteRoute(), Detalle(), actualizarCliente(), historialDeCliente(), obtenerCliente(), RegistroHistorial, ACCIONES, ClienteDetalle() (+8 more)

### Community 46 - "DispositivosController.cs"
Cohesion: 0.36
Nodes (6): PuntoIOS.Application.Features.Dispositivos.HistorialDispositivo, PuntoIOS.Application.Features.Dispositivos.CrearDispositivo, PuntoIOS.Application.Features.Dispositivos.ActualizarDispositivo, PuntoIOS.Application.Features.Dispositivos.ListarDispositivos, PuntoIOS.Application.Features.Dispositivos.RevelarImei, PuntoIOS.Application.Features.Dispositivos.ObtenerDispositivo

### Community 47 - "Guid"
Cohesion: 0.12
Nodes (6): IDirectorioDeUsuarios, UsuarioResumen, RolDto, FotoDto, DirectorioDeUsuarios, DirectorioFijo

### Community 48 - "List"
Cohesion: 0.37
Nodes (5): CrearVersionPresupuestoRequest, ItemPresupuestoRequest, ItemPresupuestoDto, VersionDePresupuestoDto, PresupuestoEndpointTests

### Community 50 - "RegistroAuditoria"
Cohesion: 0.13
Nodes (13): AccionesAuditadas, EntidadesAuditadas, RegistroAuditoria, Accion, Cambios, Entidad, EntidadId, Fecha (+5 more)

### Community 51 - "RelojFijo"
Cohesion: 0.21
Nodes (8): ActualizarDispositivoCommand, CrearDispositivoCommand, DispositivoHandlersTests, LocalFijo, LocalId, UsuarioFijo, RelojFijo, Ahora

### Community 53 - "ReparacionesEndpointTests.cs"
Cohesion: 0.15
Nodes (7): PuntoIOS.Api.Controllers, PuntoIOS.IntegrationTests, PuntoIOS.Application.Features.Dispositivos, PuntoIOS.Application.Features.Identidad.ObtenerUsuarioActual, PuntoIOS.Application.Features.Clientes, PuntoIOS.Application.Features.Reparaciones, PuntoIOS.Application.Features.Clientes.SeguimientoCliente

### Community 54 - "PuntoIOS.Domain.Auditoria"
Cohesion: 0.20
Nodes (6): PuntoIOS.Application.Features.Reparaciones.Puertos, PuntoIOS.Application.Common.Archivos, PuntoIOS.Application.Features.Dispositivos.Puertos, PuntoIOS.Application.Features.Auditoria, PuntoIOS.Infrastructure.Archivos, PuntoIOS.Domain.Auditoria

### Community 55 - "PuntoIOS.Domain.Reparaciones"
Cohesion: 0.09
Nodes (14): PuntoIOS.Application.Features.Reparaciones.CrearVersionPresupuesto, PuntoIOS.Infrastructure.Persistence.Configurations, PuntoIOS.UnitTests.Clientes, PuntoIOS.Application.Features.Clientes.ActualizarCliente, PuntoIOS.Application.Features.Reparaciones.ConsultarOrdenes, PuntoIOS.Application.Features.Reparaciones.VerDesbloqueo, PuntoIOS.Application.Features.Reparaciones.RecibirEquipo, PuntoIOS.Application.Features.Reparaciones.ActualizarDatosTecnicos (+6 more)

### Community 56 - "devDependencies"
Cohesion: 0.15
Nodes (13): devDependencies, @tailwindcss/turbopack@^4, @testing-library/react@^16.3.3, @types/react@^19, @types/react-dom@^19, typescript@^5, @vitejs/plugin-react@^6.2.1, vitest@^5.1.0 (+5 more)

### Community 58 - "Guid"
Cohesion: 0.11
Nodes (11): ClientesController, DispositivosController, MeController, ReparacionesController, TipoReparacionDto, UsuariosController, ClienteDto, CrearClienteCommand (+3 more)

### Community 59 - "PuntoIOS.Application.Features.Identidad.ObtenerUsuarioActual"
Cohesion: 0.13
Nodes (10): PagedResult, TotalPages, PageRequest, Page, PageSize, Skip, PagedResultTests, PageRequestTests (+2 more)

### Community 60 - "EntityTypeBuilder"
Cohesion: 0.31
Nodes (4): ClienteConfiguration, DispositivoConfiguration, LocalConfiguration, VersionDePresupuestoConfiguration

### Community 61 - "login-form.tsx"
Cohesion: 0.19
Nodes (12): OjoAbierto(), OjoCerrado(), PasswordInput(), Props, describeAuthError(), LoginForm(), onGoogle(), onSubmit() (+4 more)

### Community 62 - "IAuditoriaRepositorio"
Cohesion: 0.35
Nodes (13): ActualizarClienteHandler, CrearClienteHandler, HistorialClienteHandler, ConsultarCodigoSeguimientoHandler, GenerarCodigoSeguimientoHandler, ActualizarDispositivoHandler, CrearDispositivoHandler, RevelarImeiHandler (+5 more)

### Community 66 - "Usuario.Registrar"
Cohesion: 0.28
Nodes (3): ActualizarUsuarioCommand, RegistrarUsuarioCommand, GestionDeUsuariosTests

### Community 67 - "vitest"
Cohesion: 0.06
Nodes (22): mocks, ana, Handler, mocks, hoy(), NuevaOrdenForm(), ana, iphone (+14 more)

### Community 68 - "Task"
Cohesion: 0.17
Nodes (5): ActualizarUsuarioHandler, IRolRepositorio, UsuarioDto, RolRepositorio, RolRepositorioEnMemoria

### Community 69 - "CrearVersionPresupuestoHandlerTests"
Cohesion: 0.46
Nodes (3): CrearVersionPresupuestoCommand, ItemPresupuestoInput, CrearVersionPresupuestoHandlerTests

### Community 71 - "CurrentUser"
Cohesion: 0.25
Nodes (5): CurrentUser, ObtenerUsuarioActualHandler, IdentificarUsuarioHandlerTests, FakeAccessor, ObtenerUsuarioActualHandlerTests

### Community 72 - "RecibirEquipoHandler.HandleAsync"
Cohesion: 0.14
Nodes (7): ClienteResumenDto, DispositivoValidacion, ListarDispositivosHandler, ObtenerDispositivoHandler, IDispositivoRepositorio, ArmadorDeOrdenes, CambioDeCampo

### Community 73 - "OrdenReparacionDto"
Cohesion: 0.37
Nodes (4): ResponderPresupuestoRequest, ChecklistDto, OrdenReparacionDto, ResponderPresupuestoEndpointTests

### Community 74 - "equipos-cliente.tsx"
Cohesion: 0.17
Nodes (9): listarDispositivos(), revelarImei(), EquiposCliente(), verImei(), Lista, Props, iphone, mocks (+1 more)

### Community 75 - "Stream"
Cohesion: 0.19
Nodes (6): IAlmacenDeArchivos, ArchivoDeFoto, ObtenerFotoHandler, AlmacenEnDisco, AlmacenEnMemoria, Archivos

### Community 76 - "CodigoSeguimiento.Generar"
Cohesion: 0.16
Nodes (3): CodigoSeguimiento, CodigoSeguimientoTests, SeguimientoClienteHandlersTests

### Community 77 - "dependencies"
Cohesion: 0.29
Nodes (7): dependencies, firebase@^13.2.0, next@16.4.5, react@19.3.0, react-dom@19.3.0, firebase, react-dom

### Community 78 - "scripts"
Cohesion: 0.25
Nodes (8): scripts, build, dev, lint, start, test, test:e2e, typecheck

### Community 79 - "orden-detalle.test.tsx"
Cohesion: 0.36
Nodes (10): json(), me(), mocks, orden, PERMISOS_ESTADO, stubApi(), stubCambioDeEstado(), stubDatosTecnicos() (+2 more)

### Community 81 - "@playwright/test"
Cohesion: 0.29
Nodes (3): CORS, @playwright/test@^1.64.1, @playwright/test

### Community 82 - "Session state (auto-saved — last known reality)"
Cohesion: 0.40
Nodes (4): Recent commits, Resume, Session state (auto-saved — last known reality), Uncommitted changes

### Community 84 - "useAuth"
Cohesion: 0.09
Nodes (33): nextConfig, ClientesRoute(), IngresarPage(), PanelPage(), Detalle(), OrdenRoute(), ReparacionesRoute(), BackLink() (+25 more)

### Community 86 - "eslint.config.mjs"
Cohesion: 0.33
Nodes (5): eslintConfig, eslint@^9, eslint-config-next@16.4.5, eslint, eslint-config-next

### Community 90 - "TiposDeReparacion.cs"
Cohesion: 0.29
Nodes (3): EstadosDeReparacion, TiposDeDesbloqueo, TiposDeReparacion

### Community 92 - "ArgumentException"
Cohesion: 0.22
Nodes (4): ItemDePresupuesto, TiposDeItemPresupuesto, VersionDePresupuestoTests, ItemsInvalidos

### Community 94 - "OrdenReparacion.Recibir"
Cohesion: 0.15
Nodes (4): EmailValidator, ClienteValidacion, ItemsDeChecklist, PuntoIOS.Application.Common.Validation

### Community 95 - "IDirectorioDeUsuarios"
Cohesion: 0.27
Nodes (8): HistorialDispositivoHandler, CambiarEstadoOrdenHandler, HistorialOrdenHandler, ListarOrdenesHandler, ListarTecnicosHandler, ObtenerOrdenHandler, ListarVersionesPresupuestoHandler, ResponderPresupuestoHandler

### Community 96 - "Cliente"
Cohesion: 0.18
Nodes (5): IClienteRepositorio, ClienteRepositorio, ClienteRepoEnMemoria, Clientes, Guardados

### Community 97 - "RecibirEquipoRequest"
Cohesion: 0.40
Nodes (3): ChecklistRequest, RecibirEquipoRequest, ChecklistYFotosEndpointTests

### Community 100 - "package.json"
Cohesion: 0.20
Nodes (9): jsdom@^30.1.3, tailwindcss@^4, @types/node@^24.19.2, name, private, version, jsdom, tailwindcss (+1 more)

## Knowledge Gaps
- **182 isolated node(s):** `Id`, `LocalId`, `Entidad`, `EntidadId`, `Accion` (+177 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 527 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **36 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `OrdenReparacion` connect `OrdenReparacion` to `MaquinaDeEstadosTests`, `DateTimeOffset`, `Guid`, `List`, `ReparacionesEndpointTests.cs`, `Fact`, `CambiarEstadoOrdenHandlerTests`, `OrdenReparacion.Recibir`?**
  _High betweenness centrality (0.033) - this node is a cross-community bridge._
- **What connects `Id`, `LocalId`, `Entidad` to the rest of the system?**
  _182 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Punto iOS — Software Design Document (SDD)` be split into smaller, more focused modules?**
  _Cohesion score 0.09090909090909091 - nodes in this community are weakly interconnected._
- **Why does `PuntoIosDbContext` connect `PuntoIosDbContext` to `Cliente`, `OrdenRepoEnMemoria`, `VersionDePresupuesto`, `Task`, `Dispositivo`, `PermisoConfiguration`, `UsuarioConfiguration`, `Guid`, `RolConfiguration`, `Usuario`, `RegistroAuditoria`, `HttpClient`, `PuntoIOS.Domain.Reparaciones`, `DependencyInjection.AddInfrastructure`, `EntityTypeBuilder`, `IAuditoriaRepositorio`?**
  _High betweenness centrality (0.032) - this node is a cross-community bridge._
- **Should `PuntoIOS.ArchitectureTests.csproj` be split into smaller, more focused modules?**
  _Cohesion score 0.059800664451827246 - nodes in this community are weakly interconnected._
- **Why does `RegistroAuditoria` connect `RegistroAuditoria` to `OrdenRepoEnMemoria`, `RecibirEquipoHandlerTests`, `RecibirEquipoHandler.HandleAsync`, `DateTimeOffset`, `Guid`, `List`, `PuntoIosDbContext`, `PagedResult`?**
  _High betweenness centrality (0.027) - this node is a cross-community bridge._
- **Should `3. Fases` be split into smaller, more focused modules?**
  _Cohesion score 0.09523809523809523 - nodes in this community are weakly interconnected._