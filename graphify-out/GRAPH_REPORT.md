# Graph Report - Punto iOS  (2026-11-08)

## Corpus Check
- 250 files · ~73,132 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 8 file(s) not represented in the graph (top: (none) 5, .example 1, .ico 1)

## Summary
- 1984 nodes · 6174 edges · 114 communities (73 shown, 41 thin omitted)
- Extraction: 92% EXTRACTED · 8% INFERRED · 0% AMBIGUOUS · INFERRED: 481 edges (avg confidence: 0.84)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `aabaece5`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- Punto iOS — Software Design Document (SDD)
- PuntoIOS.ArchitectureTests.csproj
- 3. Fases
- 0001-decisiones-iniciales.md
- CambiarEstadoEndpointTests.OrdenAsync
- Dispositivo
- use-remote-checks.ts
- DispositivoHandlersTests
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
- Task
- Fact
- PuntoIOS.Domain.Identidad
- Resultado
- ReparacionesEndpointTests.OrdenesDeDosTecnicosAsync
- CurrentUser
- TestApi.cs
- Rol
- DispositivoDto
- CancellationToken
- login-form.tsx
- TestApi.CrearAsync
- reparaciones/api.ts
- PuntoIOS.Application.Common.Security
- clientes/api.ts
- MaquinaDeEstadosTests
- ClientesEndpointTests
- LocalUnico
- DateTimeOffset
- OrdenReparacion
- Cliente
- RecibirEquipoHandlerTests
- CrearClienteHandlerTests
- IReadOnlyList
- List
- LayerDependencyTests
- RegistroAuditoria
- ServiceCollectionExtensions.AddCasosDeUso
- ObtenerUsuarioActualHandler
- FotoOrden
- PuntoIOS.Domain.Auditoria
- Usuario.Registrar
- devDependencies
- IdentificarUsuarioHandler
- Guid
- ReparacionesController.cs
- EntityTypeBuilder
- DateTimeOffset
- RecibirEquipoHandler
- DatosTecnicosEndpointTests.OrdenAsync
- DependencyInjection.AddInfrastructure
- IReadOnlyDictionary
- IOrdenReparacionRepositorio
- vitest
- Roles.Administrador
- CrearVersionPresupuestoHandlerTests
- PageRequest
- RecibirEquipoHandler.HandleAsync
- ChecklistYFotosTests
- CodigoSeguimiento.Generar
- Stream
- CambiarEstadoOrdenHandlerTests
- dependencies
- scripts
- IDirectorioDeUsuarios.ListarActivosConPermisoAsync
- ReparacionesConfiguraciones.cs
- @playwright/test
- Session state (auto-saved — last known reality)
- DotEnv
- useAuth
- eslint.config.mjs
- ClienteRepoEnMemoria
- @testing-library/jest-dom
- [codigo]/page.tsx
- equipos-cliente.tsx
- ArgumentException
- Theory
- VersionDePresupuesto
- IClassFixture
- SeguimientoClienteHandlersTests
- RecibirEquipoRequest
- CifradoConDataProtection
- ListarOrdenesHandlerTests
- PatronDesbloqueo
- PermisoConfiguration
- DispositivoRepoEnMemoria
- vitest.config.mts
- UsuarioConfiguration
- @types/react@^19
- @types/react-dom@^19

## God Nodes (most connected - your core abstractions)
1. `PuntoIosDbContext` - 43 edges
2. `OrdenReparacion` - 42 edges
3. `OrdenReparacionDto` - 40 edges
4. `Usuario` - 39 edges
5. `PuntoIOS.Domain.Identidad` - 36 edges
6. `VersionDePresupuesto` - 33 edges
7. `PuntoIOS.Domain.Auditoria` - 33 edges
8. `Cliente` - 31 edges
9. `useAuth()` - 31 edges
10. `ReparacionesController` - 30 edges

## Surprising Connections (you probably didn't know these)
- `TestApi` --references--> `Program`  [EXTRACTED]
  backend/tests/PuntoIOS.IntegrationTests/TestApi.cs → backend/src/PuntoIOS.Api/Program.cs
- `ChecklistYFotosTests` --references--> `AlmacenEnMemoria`  [EXTRACTED]
  backend/tests/PuntoIOS.UnitTests/Reparaciones/ChecklistYFotosTests.cs → backend/tests/PuntoIOS.UnitTests/Clientes/FakesClientes.cs
- `ConsultarCodigoSeguimientoHandler` --references--> `ICifradoDeDatos`  [EXTRACTED]
  backend/src/PuntoIOS.Application/Features/Clientes/SeguimientoCliente/CodigoSeguimientoHandlers.cs → backend/src/PuntoIOS.Application/Common/Security/ICifradoDeDatos.cs
- `GenerarCodigoSeguimientoHandler` --references--> `ICifradoDeDatos`  [EXTRACTED]
  backend/src/PuntoIOS.Application/Features/Clientes/SeguimientoCliente/CodigoSeguimientoHandlers.cs → backend/src/PuntoIOS.Application/Common/Security/ICifradoDeDatos.cs
- `CrearClienteHandlerTests` --references--> `DispositivoRepoEnMemoria`  [EXTRACTED]
  backend/tests/PuntoIOS.UnitTests/Clientes/CrearClienteHandlerTests.cs → backend/tests/PuntoIOS.UnitTests/Clientes/FakesClientes.cs

## Import Cycles
- None detected.

## Communities (114 total, 41 thin omitted)

### Community 0 - "Punto iOS — Software Design Document (SDD)"
Cohesion: 0.09
Nodes (21): 10. Notificaciones internas, 11. PWA y offline, 12. API, 13. Seguridad y auditoría, 14. Pruebas, 15. Diseño visual, 16. Observabilidad, 17. Definition of Done por slice (+13 more)

### Community 1 - "PuntoIOS.ArchitectureTests.csproj"
Cohesion: 0.06
Nodes (35): net10.0, Microsoft.EntityFrameworkCore.Design (10.0.14), net10.0, Microsoft.NET.Sdk, net10.0, Microsoft.NET.Sdk, net10.0, Microsoft.EntityFrameworkCore.Design (10.0.14) (+27 more)

### Community 2 - "3. Fases"
Cohesion: 0.10
Nodes (20): 1. Ciclo de cada slice, 2. Hitos de uso real, 3. Fases, 4. Flujos E2E mínimos (Playwright), 5. Definition of Done, 6. Estrategia Git, Fase 10 — Accesorios, repuestos e inventario, Fase 11 — Finanzas → Hito C (+12 more)

### Community 5 - "Dispositivo"
Cohesion: 0.11
Nodes (14): Dispositivo, Almacenamiento, ClienteId, Color, CreadoEn, CreadoPorUsuarioId, Id, ImeiSerie (+6 more)

### Community 6 - "use-remote-checks.ts"
Cohesion: 0.17
Nodes (14): Home(), REMOTE_CHECKS, RemoteCheckId, Row, RowState, StatusBoard(), subscribeToConnection(), useOnline() (+6 more)

### Community 7 - "DispositivoHandlersTests"
Cohesion: 0.22
Nodes (6): ActualizarDispositivoCommand, CrearDispositivoCommand, DispositivoHandlersTests, UsuarioFijo, RelojFijo, Ahora

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
Cohesion: 0.09
Nodes (14): ActualizarUsuarioHandler, ListarRolesHandler, ListarUsuariosHandler, IRolRepositorio, IUsuarioRepositorio, RolRepositorio, UsuarioRepositorio, ConRol (+6 more)

### Community 16 - "PuntoIosDbContext"
Cohesion: 0.11
Nodes (15): DesignTimeDbContextFactory, InicializadorBaseDeDatos, PuntoIosDbContext, Auditoria, Clientes, Dispositivos, FotosOrden, LocalActualId (+7 more)

### Community 20 - "auth-provider.tsx"
Cohesion: 0.17
Nodes (12): body, display, metadata, RootLayout(), viewport, AuthContext, AuthContextValue, AuthProvider() (+4 more)

### Community 22 - "seguimiento-cliente.tsx"
Cohesion: 0.19
Nodes (19): formatoFecha, Props, SeguimientoCliente(), conToken(), enviarPorWhatsapp(), generar(), ver(), ana (+11 more)

### Community 23 - "TestApi.cs"
Cohesion: 0.24
Nodes (4): PuntoIOS.Api.Controllers, PuntoIOS.IntegrationTests, PuntoIOS.Application.Features.Auditoria, PuntoIOS.Application.Features.Reparaciones

### Community 24 - "Task"
Cohesion: 0.20
Nodes (3): HistorialDeAuditoria, RegistroHistorialDto, OrdenResumenDto

### Community 26 - "PuntoIOS.Domain.Identidad"
Cohesion: 0.07
Nodes (19): BuscarClientesHandler, ObtenerClienteHandler, Permiso, Codigo, Descripcion, RolesDeSistema, PuntoIOS.Application.Common.Results, PuntoIOS.Application.Features.Clientes.Puertos (+11 more)

### Community 27 - "Resultado"
Cohesion: 0.17
Nodes (10): ResultadoExtensions, Error, Resultado, Error, EsExitoso, Valor, TipoError, Conflicto (+2 more)

### Community 30 - "TestApi.cs"
Cohesion: 0.09
Nodes (4): Program, AssemblyMarker, PuntoIOS.Infrastructure, PuntoIOS.Api.Extensions

### Community 31 - "Rol"
Cohesion: 0.15
Nodes (9): Rol, Descripcion, EsDeSistema, Id, Nombre, Permisos, RolPermiso, PermisoCodigo (+1 more)

### Community 32 - "DispositivoDto"
Cohesion: 0.42
Nodes (3): ActualizarDispositivoRequest, ImeiDto, DispositivosEndpointTests

### Community 33 - "CancellationToken"
Cohesion: 0.11
Nodes (6): OrdenReparacionRepositorio, OrdenRepoEnMemoria, Fotos, Guardados, Ordenes, VersionesPresupuesto

### Community 34 - "login-form.tsx"
Cohesion: 0.18
Nodes (13): IngresarPage(), OjoAbierto(), OjoCerrado(), PasswordInput(), Props, describeAuthError(), LoginForm(), onGoogle() (+5 more)

### Community 35 - "TestApi.CrearAsync"
Cohesion: 0.10
Nodes (5): RegistrarUsuarioRequest, MeEndpointTests, TestApi, Servicios, UsuariosEndpointTests

### Community 36 - "reparaciones/api.ts"
Cohesion: 0.06
Nodes (59): CambioDeCampo, Dispositivo, actualizarDatosTecnicos(), AVISO_FECHA_ESTIMADA, cambiarEstadoDeOrden(), Checklist, CHECKLIST_CATALOGO, DatosTecnicos (+51 more)

### Community 37 - "PuntoIOS.Application.Common.Security"
Cohesion: 0.10
Nodes (14): FirebaseClaims, UsuarioLocalClaims, ICurrentUserAccessor, EstadoIdentificacion, Autorizado, Desactivado, NoRegistrado, ResultadoIdentificacion (+6 more)

### Community 38 - "clientes/api.ts"
Cohesion: 0.09
Nodes (35): actualizarCliente(), Coincidencia, crearCliente(), DatosCliente, EquipoNuevo, NuevoCliente, Pagina, TAMANO_PAGINA (+27 more)

### Community 40 - "ClientesEndpointTests"
Cohesion: 0.24
Nodes (4): ActualizarClienteRequest, CrearClienteRequest, EquipoNuevoRequest, ClientesEndpointTests

### Community 41 - "LocalUnico"
Cohesion: 0.17
Nodes (10): ILocalActual, LocalId, Local, Id, Nombre, LocalesDeSistema, LocalUnico, LocalId (+2 more)

### Community 42 - "DateTimeOffset"
Cohesion: 0.07
Nodes (12): InicialIdentidad, ClientesYLocales, AuditoriaYEdicionClientes, DispositivosDeClientes, CodigoSeguimientoClientes, OrdenesDeReparacion, ChecklistYFotosDeOrdenes, PermisoCambiarEstado (+4 more)

### Community 43 - "OrdenReparacion"
Cohesion: 0.07
Nodes (26): OrdenReparacion, ChecklistItems, ClienteId, ComponentesAfectados, CreadoEn, CreadoPorUsuarioId, DesbloqueoCifrado, DesbloqueoTipo (+18 more)

### Community 44 - "Cliente"
Cohesion: 0.08
Nodes (19): EmailValidator, ClienteValidacion, Cliente, Apellido, CodigoSeguimientoCifrado, CodigoSeguimientoGeneradoEn, CodigoSeguimientoHuella, CreadoEn (+11 more)

### Community 45 - "RecibirEquipoHandlerTests"
Cohesion: 0.15
Nodes (7): RecibirEquipoCommand, DesbloqueoDto, ItemsDeChecklist, AuditoriaRepoEnMemoria, Registros, RecibirEquipoHandlerTests, Invalidos

### Community 46 - "CrearClienteHandlerTests"
Cohesion: 0.31
Nodes (3): CrearClienteCommand, EquipoNuevo, CrearClienteHandlerTests

### Community 47 - "IReadOnlyList"
Cohesion: 0.10
Nodes (7): ActualizarUsuarioRequest, UsuarioAutorizado, UsuarioActualDto, RolDto, ClienteRepositorio, DispositivoRepositorio, DirectorioFijo

### Community 48 - "List"
Cohesion: 0.37
Nodes (6): CrearVersionPresupuestoRequest, ItemPresupuestoRequest, ItemPresupuestoDto, OrdenReparacionDto, VersionDePresupuestoDto, PresupuestoEndpointTests

### Community 50 - "RegistroAuditoria"
Cohesion: 0.13
Nodes (13): AccionesAuditadas, EntidadesAuditadas, RegistroAuditoria, Accion, Cambios, Entidad, EntidadId, Fecha (+5 more)

### Community 52 - "ObtenerUsuarioActualHandler"
Cohesion: 0.54
Nodes (3): ObtenerUsuarioActualHandler, FakeAccessor, ObtenerUsuarioActualHandlerTests

### Community 53 - "FotoOrden"
Cohesion: 0.17
Nodes (9): FotoOrden, CreadoEn, CreadoPorUsuarioId, Id, LocalId, OrdenId, Ruta, Tamano (+1 more)

### Community 54 - "PuntoIOS.Domain.Auditoria"
Cohesion: 0.11
Nodes (17): PuntoIOS.Application.Features.Reparaciones.Puertos, PuntoIOS.Application.Features.Clientes.ActualizarCliente, PuntoIOS.Application.Common.Archivos, PuntoIOS.Application.Features.Dispositivos.HistorialDispositivo, PuntoIOS.Application.Features.Dispositivos.CrearDispositivo, PuntoIOS.Application.Features.Dispositivos, PuntoIOS.Application.Features.Dispositivos.ActualizarDispositivo, PuntoIOS.Application.Features.Dispositivos.ListarDispositivos (+9 more)

### Community 55 - "Usuario.Registrar"
Cohesion: 0.35
Nodes (3): ActualizarUsuarioCommand, RegistrarUsuarioCommand, GestionDeUsuariosTests

### Community 56 - "devDependencies"
Cohesion: 0.15
Nodes (13): devDependencies, jsdom@^30.1.3, tailwindcss@^4, @tailwindcss/turbopack@^4, @testing-library/react@^16.3.3, @types/node@^24.19.2, typescript@^5, vitest@^5.1.0 (+5 more)

### Community 58 - "Guid"
Cohesion: 0.13
Nodes (9): ClientesController, DispositivosController, MeController, ReparacionesController, TipoReparacionDto, RolesController, UsuariosController, ClienteDto (+1 more)

### Community 59 - "ReparacionesController.cs"
Cohesion: 0.10
Nodes (12): PuntoIOS.Application.Features.Reparaciones.CrearVersionPresupuesto, PuntoIOS.Application.Features.Reparaciones.ListarVersionesPresupuesto, PuntoIOS.UnitTests.Clientes, PuntoIOS.Application.Features.Reparaciones.ConsultarOrdenes, PuntoIOS.Application.Features.Reparaciones.VerDesbloqueo, PuntoIOS.Application.Features.Reparaciones.RecibirEquipo, PuntoIOS.Application.Features.Reparaciones.ActualizarDatosTecnicos, PuntoIOS.Application.Features.Reparaciones.CambiarEstado (+4 more)

### Community 60 - "EntityTypeBuilder"
Cohesion: 0.20
Nodes (7): ClienteConfiguration, DispositivoConfiguration, LocalConfiguration, RolConfiguration, FotoOrdenConfiguration, OrdenReparacionConfiguration, VersionDePresupuestoConfiguration

### Community 61 - "DateTimeOffset"
Cohesion: 0.15
Nodes (10): Usuario, Activo, CreadoEn, Email, FirebaseUid, Id, Nombre, Rol (+2 more)

### Community 62 - "RecibirEquipoHandler"
Cohesion: 0.32
Nodes (15): ActualizarClienteHandler, CrearClienteHandler, HistorialClienteHandler, ConsultarCodigoSeguimientoHandler, GenerarCodigoSeguimientoHandler, ActualizarDispositivoHandler, CrearDispositivoHandler, HistorialDispositivoHandler (+7 more)

### Community 65 - "IReadOnlyDictionary"
Cohesion: 0.17
Nodes (6): ChecklistDto, Permisos, TiposDeItemPresupuesto, EstadosDeReparacion, TiposDeDesbloqueo, TiposDeReparacion

### Community 66 - "IOrdenReparacionRepositorio"
Cohesion: 0.33
Nodes (8): CambiarEstadoOrdenHandler, HistorialOrdenHandler, ListarOrdenesHandler, ObtenerOrdenHandler, CrearVersionPresupuestoHandler, ListarFotosHandler, ListarVersionesPresupuestoHandler, IOrdenReparacionRepositorio

### Community 67 - "vitest"
Cohesion: 0.06
Nodes (27): mocks, ana, Handler, historial, mocks, ana, Handler, mocks (+19 more)

### Community 69 - "CrearVersionPresupuestoHandlerTests"
Cohesion: 0.46
Nodes (3): CrearVersionPresupuestoCommand, ItemPresupuestoInput, CrearVersionPresupuestoHandlerTests

### Community 70 - "PageRequest"
Cohesion: 0.13
Nodes (10): PagedResult, TotalPages, PageRequest, Page, PageSize, Skip, PagedResultTests, PageRequestTests (+2 more)

### Community 72 - "RecibirEquipoHandler.HandleAsync"
Cohesion: 0.12
Nodes (11): IAuditoriaRepositorio, ActualizarClienteCommand, ClienteResumenDto, IClienteRepositorio, CodigoSeguimientoDto, DispositivoDto, DispositivoValidacion, ObtenerDispositivoHandler (+3 more)

### Community 73 - "ChecklistYFotosTests"
Cohesion: 0.40
Nodes (3): ChecklistIngreso, ItemChecklist, ChecklistYFotosTests

### Community 75 - "Stream"
Cohesion: 0.27
Nodes (5): FormatoDeImagen, IAlmacenDeArchivos, ArchivoDeFoto, ObtenerFotoHandler, AlmacenEnDisco

### Community 76 - "CambiarEstadoOrdenHandlerTests"
Cohesion: 0.21
Nodes (4): ActualizarDatosTecnicosCommand, CambiarEstadoOrdenCommand, CambiarEstadoOrdenHandlerTests, ActualizarDatosTecnicosHandlerTests

### Community 77 - "dependencies"
Cohesion: 0.22
Nodes (10): dependencies, firebase@^13.2.0, next@16.4.5, react@19.3.0, react-dom@19.3.0, name, private, version (+2 more)

### Community 78 - "scripts"
Cohesion: 0.25
Nodes (8): scripts, build, dev, lint, start, test, test:e2e, typecheck

### Community 79 - "IDirectorioDeUsuarios.ListarActivosConPermisoAsync"
Cohesion: 0.36
Nodes (4): IDirectorioDeUsuarios, UsuarioResumen, ListarTecnicosHandler, DirectorioDeUsuarios

### Community 81 - "@playwright/test"
Cohesion: 0.29
Nodes (3): CORS, @playwright/test@^1.64.1, @playwright/test

### Community 82 - "Session state (auto-saved — last known reality)"
Cohesion: 0.40
Nodes (4): Recent commits, Resume, Session state (auto-saved — last known reality), Uncommitted changes

### Community 84 - "useAuth"
Cohesion: 0.08
Nodes (38): nextConfig, ClienteRoute(), Detalle(), ClientesRoute(), PanelPage(), Detalle(), OrdenRoute(), ReparacionesRoute() (+30 more)

### Community 86 - "eslint.config.mjs"
Cohesion: 0.33
Nodes (5): eslintConfig, eslint@^9, eslint-config-next@16.4.5, eslint, eslint-config-next

### Community 88 - "ClienteRepoEnMemoria"
Cohesion: 0.50
Nodes (3): ClienteRepoEnMemoria, Clientes, Guardados

### Community 91 - "equipos-cliente.tsx"
Cohesion: 0.17
Nodes (9): listarDispositivos(), revelarImei(), EquiposCliente(), verImei(), Lista, Props, iphone, mocks (+1 more)

### Community 92 - "ArgumentException"
Cohesion: 0.29
Nodes (3): ItemDePresupuesto, VersionDePresupuestoTests, ItemsInvalidos

### Community 94 - "VersionDePresupuesto"
Cohesion: 0.18
Nodes (9): VersionDePresupuesto, CreadoEn, CreadoPorUsuarioId, Id, Items, LocalId, Numero, OrdenId (+1 more)

### Community 96 - "SeguimientoClienteHandlersTests"
Cohesion: 0.19
Nodes (5): ICifradoDeDatos, LocalFijo, LocalId, CifradoDePrueba, SeguimientoClienteHandlersTests

### Community 97 - "RecibirEquipoRequest"
Cohesion: 0.38
Nodes (4): CrearDispositivoRequest, ChecklistRequest, RecibirEquipoRequest, ChecklistYFotosEndpointTests

### Community 102 - "PatronDesbloqueo"
Cohesion: 0.43
Nodes (5): centro(), PatronDesbloqueo(), Props, PUNTOS, Editor()

### Community 104 - "DispositivoRepoEnMemoria"
Cohesion: 0.50
Nodes (3): DispositivoRepoEnMemoria, Dispositivos, Guardados

## Knowledge Gaps
- **179 isolated node(s):** `Microsoft.EntityFrameworkCore.Design (10.0.14)`, `Microsoft.EntityFrameworkCore.Design (10.0.14)`, `coverlet.collector (6.0.4)`, `Microsoft.NET.Test.Sdk (17.14.1)`, `xunit (2.9.3)` (+174 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 528 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **41 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `PuntoIosDbContext` connect `PuntoIosDbContext` to `DependencyInjection.AddInfrastructure`, `CancellationToken`, `TestApi.CrearAsync`, `PermisoConfiguration`, `UsuarioConfiguration`, `IReadOnlyList`, `IDirectorioDeUsuarios.ListarActivosConPermisoAsync`, `ReparacionesConfiguraciones.cs`, `RegistroAuditoria`, `Usuario`, `List`, `RecibirEquipoHandler`, `EntityTypeBuilder`, `VersionDePresupuesto`?**
  _High betweenness centrality (0.057) - this node is a cross-community bridge._
- **What connects `Microsoft.EntityFrameworkCore.Design (10.0.14)`, `Microsoft.EntityFrameworkCore.Design (10.0.14)`, `coverlet.collector (6.0.4)` to the rest of the system?**
  _179 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Punto iOS — Software Design Document (SDD)` be split into smaller, more focused modules?**
  _Cohesion score 0.09090909090909091 - nodes in this community are weakly interconnected._
- **Why does `PuntoIOS.Domain.Identidad` connect `PuntoIOS.Domain.Identidad` to `IReadOnlyDictionary`, `PuntoIOS.Application.Common.Security`, `DateTimeOffset`, `ReparacionesConfiguraciones.cs`, `ServiceCollectionExtensions.cs`, `PuntoIOS.Domain.Auditoria`, `TestApi.cs`, `ReparacionesController.cs`, `DateTimeOffset`, `Rol`?**
  _High betweenness centrality (0.030) - this node is a cross-community bridge._
- **Should `PuntoIOS.ArchitectureTests.csproj` be split into smaller, more focused modules?**
  _Cohesion score 0.059800664451827246 - nodes in this community are weakly interconnected._
- **Why does `OrdenReparacion` connect `OrdenReparacion` to `MaquinaDeEstadosTests`, `DateTimeOffset`, `CambiarEstadoOrdenHandlerTests`, `RecibirEquipoHandlerTests`, `IReadOnlyList`, `List`, `FotoOrden`, `Fact`, `ReparacionesController.cs`?**
  _High betweenness centrality (0.020) - this node is a cross-community bridge._
- **Should `3. Fases` be split into smaller, more focused modules?**
  _Cohesion score 0.09523809523809523 - nodes in this community are weakly interconnected._