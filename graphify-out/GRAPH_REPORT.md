# Graph Report - Punto iOS  (2026-11-08)

## Corpus Check
- 236 files · ~63,110 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 8 file(s) not represented in the graph (top: (none) 5, .example 1, .ico 1)

## Summary
- 1794 nodes · 5282 edges · 88 communities (67 shown, 21 thin omitted)
- Extraction: 93% EXTRACTED · 7% INFERRED · 0% AMBIGUOUS · INFERRED: 380 edges (avg confidence: 0.84)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `3e5e4fb6`
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
- Usuario.Registrar
- puntoios_application_common_pagination
- Resultado
- PuntoIOS.Domain.Reparaciones
- TestApi
- TestApi.cs
- Rol
- useAuth
- DispositivosController.cs
- login-form.tsx
- Fact
- reparaciones/api.ts
- cliente-detalle.tsx
- clientes/api.ts
- MaquinaDeEstadosTests
- Dispositivo
- CurrentUser
- DateTimeOffset
- OrdenReparacion
- Cliente
- DispositivosEndpointTests
- Theory
- Guid
- ReparacionesEndpointTests.ClienteConEquipoAsync
- LayerDependencyTests
- RegistroAuditoria
- ServiceCollectionExtensions.AddCasosDeUso
- IdentificarUsuarioHandler.HandleAsync
- scripts
- PuntoIOS.Domain.Auditoria
- Task
- devDependencies
- ChecklistYFotosEndpointTests.ClienteConEquipoAsync
- Guid
- CrearClienteHandlerTests
- EntityTypeBuilder
- @playwright/test
- CambiarEstadoEndpointTests.OrdenAsync
- eslint.config.mjs
- package.json
- LocalUnico
- CambiarEstadoOrdenHandlerTests
- vitest
- @testing-library/jest-dom
- IReadOnlyList
- PageRequest
- IOrdenReparacionRepositorio
- RecibirEquipoHandler.HandleAsync
- ObtenerUsuarioActualHandler
- orden-detalle.test.tsx
- PermisoConfiguration
- FotoOrden
- ChecklistYFotosTests
- SeguimientoClienteHandlersTests
- DependencyInjection.AddInfrastructure
- RecibirEquipoHandlerTests
- HealthEndpointTests
- Session state (auto-saved — last known reality)
- DotEnv
- Permiso
- ClienteRepoEnMemoria
- RolConfiguration

## God Nodes (most connected - your core abstractions)
1. `OrdenReparacion` - 49 edges
2. `PuntoIosDbContext` - 41 edges
3. `Usuario` - 39 edges
4. `PuntoIOS.Domain.Identidad` - 34 edges
5. `Cliente` - 31 edges
6. `useAuth()` - 31 edges
7. `PuntoIOS.Domain.Auditoria` - 29 edges
8. `OrdenReparacionDto` - 28 edges
9. `Rol` - 27 edges
10. `RegistroAuditoria` - 26 edges

## Surprising Connections (you probably didn't know these)
- `TestApi` --references--> `Program`  [EXTRACTED]
  backend/tests/PuntoIOS.IntegrationTests/TestApi.cs → backend/src/PuntoIOS.Api/Program.cs
- `ReparacionesController` --references--> `CambiarEstadoOrdenHandler`  [EXTRACTED]
  backend/src/PuntoIOS.Api/Controllers/ReparacionesController.cs → backend/src/PuntoIOS.Application/Features/Reparaciones/CambiarEstado/CambiarEstadoOrdenHandler.cs
- `RegistroHistorialDto` --references--> `CambioDeCampo`  [EXTRACTED]
  backend/src/PuntoIOS.Application/Features/Auditoria/HistorialDeAuditoria.cs → backend/src/PuntoIOS.Domain/Auditoria/RegistroAuditoria.cs
- `PuntoIosDbContext` --references--> `RegistroAuditoria`  [EXTRACTED]
  backend/src/PuntoIOS.Infrastructure/Persistence/PuntoIosDbContext.cs → backend/src/PuntoIOS.Domain/Auditoria/RegistroAuditoria.cs
- `AuditoriaRepoEnMemoria` --references--> `RegistroAuditoria`  [EXTRACTED]
  backend/tests/PuntoIOS.UnitTests/Clientes/FakesClientes.cs → backend/src/PuntoIOS.Domain/Auditoria/RegistroAuditoria.cs

## Import Cycles
- None detected.

## Communities (88 total, 21 thin omitted)

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
Cohesion: 0.29
Nodes (3): CrearClienteRequest, EquipoNuevoRequest, ClientesEndpointTests

### Community 6 - "use-remote-checks.ts"
Cohesion: 0.17
Nodes (14): Home(), REMOTE_CHECKS, RemoteCheckId, Row, RowState, StatusBoard(), subscribeToConnection(), useOnline() (+6 more)

### Community 7 - "DispositivoHandlersTests"
Cohesion: 0.20
Nodes (8): ActualizarDispositivoCommand, CrearDispositivoCommand, DispositivoHandlersTests, LocalFijo, LocalId, UsuarioFijo, RelojFijo, Ahora

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
Nodes (17): ActualizarUsuarioHandler, IUsuarioRepositorio, Usuario, Activo, CreadoEn, Email, FirebaseUid, Id (+9 more)

### Community 16 - "PuntoIosDbContext"
Cohesion: 0.12
Nodes (13): DesignTimeDbContextFactory, PuntoIosDbContext, Auditoria, Clientes, Dispositivos, FotosOrden, LocalActualId, Locales (+5 more)

### Community 20 - "auth-provider.tsx"
Cohesion: 0.11
Nodes (21): dependencies, firebase@^13.2.0, next@16.4.5, react@19.3.0, body, display, metadata, RootLayout() (+13 more)

### Community 22 - "seguimiento-cliente.tsx"
Cohesion: 0.19
Nodes (19): formatoFecha, Props, SeguimientoCliente(), conToken(), enviarPorWhatsapp(), generar(), ver(), ana (+11 more)

### Community 23 - "TestApi.cs"
Cohesion: 0.22
Nodes (6): Permisos, RolesDeSistema, PuntoIOS.Api.Controllers, PuntoIOS.IntegrationTests, PuntoIOS.Domain.Identidad, PuntoIOS.Application.Features.Reparaciones

### Community 25 - "Usuario.Registrar"
Cohesion: 0.25
Nodes (4): ActualizarUsuarioCommand, RegistrarUsuarioCommand, InicializadorBaseDeDatos, GestionDeUsuariosTests

### Community 26 - "puntoios_application_common_pagination"
Cohesion: 0.09
Nodes (15): BuscarClientesHandler, ListarUsuariosHandler, IRolRepositorio, RolRepositorio, RolRepositorioEnMemoria, PuntoIOS.Application.Common.Results, PuntoIOS.Application.Features.Clientes.Puertos, PuntoIOS.Application.Features.Clientes.BuscarClientes (+7 more)

### Community 27 - "Resultado"
Cohesion: 0.17
Nodes (10): ResultadoExtensions, Error, Resultado, Error, EsExitoso, Valor, TipoError, Conflicto (+2 more)

### Community 28 - "PuntoIOS.Domain.Reparaciones"
Cohesion: 0.11
Nodes (10): MaquinaDeEstados, EstadosDeReparacion, TiposDeReparacion, PuntoIOS.Infrastructure.Persistence.Configurations, PuntoIOS.Application.Features.Reparaciones.VerDesbloqueo, PuntoIOS.Application.Features.Reparaciones.RecibirEquipo, PuntoIOS.Application.Features.Reparaciones.CambiarEstado, PuntoIOS.UnitTests.Reparaciones (+2 more)

### Community 30 - "TestApi.cs"
Cohesion: 0.09
Nodes (4): Program, AssemblyMarker, PuntoIOS.Infrastructure, PuntoIOS.Api.Extensions

### Community 31 - "Rol"
Cohesion: 0.11
Nodes (11): Rol, Descripcion, EsDeSistema, Id, Nombre, Permisos, RolPermiso, PermisoCodigo (+3 more)

### Community 32 - "useAuth"
Cohesion: 0.10
Nodes (26): nextConfig, ClientesRoute(), PanelPage(), Detalle(), OrdenRoute(), ReparacionesRoute(), metadata, useAuth() (+18 more)

### Community 33 - "DispositivosController.cs"
Cohesion: 0.12
Nodes (8): RolesController, ListarRolesHandler, PuntoIOS.Application.Features.Dispositivos.HistorialDispositivo, PuntoIOS.Application.Features.Dispositivos, PuntoIOS.Application.Features.Identidad, PuntoIOS.Application.Features.Identidad.ObtenerUsuarioActual, PuntoIOS.Application.Features.Dispositivos.ObtenerDispositivo, PuntoIOS.Application.Features.Identidad.ListarRoles

### Community 34 - "login-form.tsx"
Cohesion: 0.16
Nodes (14): IngresarPage(), BackLink(), OjoAbierto(), OjoCerrado(), PasswordInput(), Props, describeAuthError(), LoginForm() (+6 more)

### Community 35 - "Fact"
Cohesion: 0.17
Nodes (3): RegistrarUsuarioRequest, MeEndpointTests, UsuariosEndpointTests

### Community 36 - "reparaciones/api.ts"
Cohesion: 0.09
Nodes (43): useMe(), AVISO_FECHA_ESTIMADA, cambiarEstadoDeOrden(), Checklist, CHECKLIST_CATALOGO, ESTADOS, Foto, ITEM_ENCIENDE (+35 more)

### Community 37 - "cliente-detalle.tsx"
Cohesion: 0.13
Nodes (19): ClienteRoute(), Detalle(), actualizarCliente(), CambioDeCampo, Cliente, crearCliente(), historialDeCliente(), NuevoCliente (+11 more)

### Community 38 - "clientes/api.ts"
Cohesion: 0.07
Nodes (40): Coincidencia, DatosCliente, EquipoNuevo, Aviso, ClienteForm(), enviar(), onSubmit(), leerEquipo() (+32 more)

### Community 40 - "Dispositivo"
Cohesion: 0.12
Nodes (14): Dispositivo, Almacenamiento, ClienteId, Color, CreadoEn, CreadoPorUsuarioId, Id, ImeiSerie (+6 more)

### Community 42 - "DateTimeOffset"
Cohesion: 0.07
Nodes (11): InicialIdentidad, ClientesYLocales, AuditoriaYEdicionClientes, DispositivosDeClientes, CodigoSeguimientoClientes, OrdenesDeReparacion, ChecklistYFotosDeOrdenes, PermisoCambiarEstado (+3 more)

### Community 43 - "OrdenReparacion"
Cohesion: 0.08
Nodes (21): OrdenReparacion, ChecklistItems, ClienteId, CreadoEn, CreadoPorUsuarioId, DesbloqueoCifrado, DesbloqueoTipo, DispositivoId (+13 more)

### Community 44 - "Cliente"
Cohesion: 0.07
Nodes (21): EmailValidator, ClienteValidacion, Cliente, Apellido, CodigoSeguimientoCifrado, CodigoSeguimientoGeneradoEn, CodigoSeguimientoHuella, CreadoEn (+13 more)

### Community 46 - "Theory"
Cohesion: 0.18
Nodes (3): TiposDeDesbloqueo, AutorizacionTests, Endpoints

### Community 47 - "Guid"
Cohesion: 0.12
Nodes (9): ActualizarUsuarioRequest, ActualizarClienteCommand, ClienteResumenDto, ListarFotosHandler, ArmadorDeOrdenes, ChecklistDto, FotoDto, OrdenReparacionDto (+1 more)

### Community 48 - "ReparacionesEndpointTests.ClienteConEquipoAsync"
Cohesion: 0.40
Nodes (3): OrdenResumenDto, DesbloqueoDto, ReparacionesEndpointTests

### Community 50 - "RegistroAuditoria"
Cohesion: 0.12
Nodes (14): AccionesAuditadas, CambioDeCampo, EntidadesAuditadas, RegistroAuditoria, Accion, Cambios, Entidad, EntidadId (+6 more)

### Community 51 - "ServiceCollectionExtensions.AddCasosDeUso"
Cohesion: 0.05
Nodes (4): ReparacionesController, ServiceCollectionExtensions, CifradoConDataProtection, PuntoIOS.Api.Seguridad

### Community 52 - "IdentificarUsuarioHandler.HandleAsync"
Cohesion: 0.25
Nodes (9): EstadoIdentificacion, Autorizado, Desactivado, NoRegistrado, IdentificarUsuarioHandler, ResultadoIdentificacion, Desactivado, NoRegistrado (+1 more)

### Community 53 - "scripts"
Cohesion: 0.25
Nodes (8): scripts, build, dev, lint, start, test, test:e2e, typecheck

### Community 54 - "PuntoIOS.Domain.Auditoria"
Cohesion: 0.11
Nodes (17): HistorialDeAuditoria, PuntoIOS.Application.Features.Reparaciones.Puertos, PuntoIOS.UnitTests.Clientes, PuntoIOS.Application.Features.Clientes.ActualizarCliente, PuntoIOS.Application.Common.Archivos, PuntoIOS.Application.Features.Dispositivos.CrearDispositivo, PuntoIOS.Application.Features.Dispositivos.ActualizarDispositivo, PuntoIOS.Application.Features.Dispositivos.ListarDispositivos (+9 more)

### Community 55 - "Task"
Cohesion: 0.11
Nodes (5): DispositivoRepositorio, OrdenReparacionRepositorio, DispositivoRepoEnMemoria, Dispositivos, Guardados

### Community 56 - "devDependencies"
Cohesion: 0.18
Nodes (11): devDependencies, jsdom@^30.1.3, @tailwindcss/turbopack@^4, @testing-library/react@^16.3.3, @types/react-dom@^19, typescript@^5, vitest@^5.1.0, jsdom (+3 more)

### Community 57 - "ChecklistYFotosEndpointTests.ClienteConEquipoAsync"
Cohesion: 0.40
Nodes (3): ChecklistRequest, RecibirEquipoRequest, ChecklistYFotosEndpointTests

### Community 58 - "Guid"
Cohesion: 0.11
Nodes (9): ClientesController, MeController, TipoReparacionDto, UsuariosController, ClienteDto, CrearClienteCommand, EquipoNuevo, ObtenerClienteHandler (+1 more)

### Community 60 - "EntityTypeBuilder"
Cohesion: 0.23
Nodes (6): ClienteConfiguration, DispositivoConfiguration, LocalConfiguration, UsuarioConfiguration, FotoOrdenConfiguration, OrdenReparacionConfiguration

### Community 61 - "@playwright/test"
Cohesion: 0.29
Nodes (3): CORS, @playwright/test@^1.64.1, @playwright/test

### Community 62 - "CambiarEstadoEndpointTests.OrdenAsync"
Cohesion: 0.18
Nodes (4): CrearDispositivoRequest, CambiarEstadoRequest, CambiarEstadoEndpointTests, SeguimientoClienteEndpointTests

### Community 63 - "eslint.config.mjs"
Cohesion: 0.33
Nodes (5): eslintConfig, eslint@^9, eslint-config-next@16.4.5, eslint, eslint-config-next

### Community 64 - "package.json"
Cohesion: 0.17
Nodes (11): react-dom@19.3.0, tailwindcss@^4, @types/node@^24.19.2, @types/react@^19, name, private, version, react-dom (+3 more)

### Community 65 - "LocalUnico"
Cohesion: 0.17
Nodes (10): ILocalActual, LocalId, Local, Id, Nombre, LocalesDeSistema, LocalUnico, LocalId (+2 more)

### Community 67 - "vitest"
Cohesion: 0.07
Nodes (20): mocks, ana, Handler, historial, mocks, ana, Handler, mocks (+12 more)

### Community 69 - "IReadOnlyList"
Cohesion: 0.17
Nodes (4): IClienteRepositorio, UsuarioActualDto, RolDto, ClienteRepositorio

### Community 70 - "PageRequest"
Cohesion: 0.13
Nodes (10): PagedResult, TotalPages, PageRequest, Page, PageSize, Skip, PagedResultTests, PageRequestTests (+2 more)

### Community 71 - "IOrdenReparacionRepositorio"
Cohesion: 0.14
Nodes (26): DispositivosController, ActualizarClienteHandler, CrearClienteHandler, HistorialClienteHandler, ConsultarCodigoSeguimientoHandler, GenerarCodigoSeguimientoHandler, ActualizarDispositivoHandler, CrearDispositivoHandler (+18 more)

### Community 72 - "RecibirEquipoHandler.HandleAsync"
Cohesion: 0.19
Nodes (4): CodigoSeguimientoDto, DispositivoDto, ImeiDto, DispositivoValidacion

### Community 73 - "ObtenerUsuarioActualHandler"
Cohesion: 0.54
Nodes (3): ObtenerUsuarioActualHandler, FakeAccessor, ObtenerUsuarioActualHandlerTests

### Community 74 - "orden-detalle.test.tsx"
Cohesion: 0.39
Nodes (7): json(), me(), mocks, orden, PERMISOS_ESTADO, stubApi(), stubCambioDeEstado()

### Community 76 - "FotoOrden"
Cohesion: 0.14
Nodes (13): FotoOrden, CreadoEn, CreadoPorUsuarioId, Id, LocalId, OrdenId, Ruta, Tamano (+5 more)

### Community 77 - "ChecklistYFotosTests"
Cohesion: 0.40
Nodes (3): ChecklistIngreso, ItemChecklist, ChecklistYFotosTests

### Community 78 - "SeguimientoClienteHandlersTests"
Cohesion: 0.06
Nodes (12): FirebaseClaims, HttpContextCurrentUserAccessor, UsuarioLocalClaims, ICifradoDeDatos, ICurrentUserAccessor, CodigoSeguimiento, CodigoSeguimientoTests, CifradoDePrueba (+4 more)

### Community 79 - "DependencyInjection.AddInfrastructure"
Cohesion: 0.08
Nodes (9): FormatoDeImagen, IAlmacenDeArchivos, RegistrarUsuarioHandler, ArchivoDeFoto, ObtenerFotoHandler, AlmacenEnDisco, DependencyInjection, AlmacenEnMemoria (+1 more)

### Community 80 - "RecibirEquipoHandlerTests"
Cohesion: 0.26
Nodes (5): RecibirEquipoCommand, AuditoriaRepoEnMemoria, Registros, RecibirEquipoHandlerTests, Invalidos

### Community 82 - "Session state (auto-saved — last known reality)"
Cohesion: 0.40
Nodes (4): Recent commits, Resume, Session state (auto-saved — last known reality), Uncommitted changes

### Community 84 - "Permiso"
Cohesion: 0.50
Nodes (3): Permiso, Codigo, Descripcion

### Community 85 - "ClienteRepoEnMemoria"
Cohesion: 0.50
Nodes (3): ClienteRepoEnMemoria, Clientes, Guardados

## Knowledge Gaps
- **163 isolated node(s):** `Id`, `LocalId`, `Entidad`, `EntidadId`, `Accion` (+158 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 495 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **21 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `PuntoIosDbContext` connect `PuntoIosDbContext` to `IReadOnlyList`, `IOrdenReparacionRepositorio`, `OrdenReparacion`, `FotoOrden`, `PermisoConfiguration`, `EntityTypeBuilder`, `DependencyInjection.AddInfrastructure`, `Guid`, `Usuario`, `RegistroAuditoria`, `ReparacionesEndpointTests.ClienteConEquipoAsync`, `RolConfiguration`, `Task`, `Usuario.Registrar`, `puntoios_application_common_pagination`, `PuntoIOS.Domain.Reparaciones`, `TestApi`?**
  _High betweenness centrality (0.044) - this node is a cross-community bridge._
- **What connects `Id`, `LocalId`, `Entidad` to the rest of the system?**
  _163 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Punto iOS — Software Design Document (SDD)` be split into smaller, more focused modules?**
  _Cohesion score 0.09090909090909091 - nodes in this community are weakly interconnected._
- **Why does `Cliente` connect `Cliente` to `DateTimeOffset`, `SeguimientoClienteHandlersTests`, `Guid`, `PuntoIOS.Domain.Auditoria`, `Guid`, `EntityTypeBuilder`?**
  _High betweenness centrality (0.029) - this node is a cross-community bridge._
- **Should `PuntoIOS.ArchitectureTests.csproj` be split into smaller, more focused modules?**
  _Cohesion score 0.059800664451827246 - nodes in this community are weakly interconnected._
- **Why does `Usuario` connect `Usuario` to `CurrentUser`, `DateTimeOffset`, `Guid`, `Task`, `Task`, `Usuario.Registrar`, `Rol`?**
  _High betweenness centrality (0.022) - this node is a cross-community bridge._
- **Should `3. Fases` be split into smaller, more focused modules?**
  _Cohesion score 0.09523809523809523 - nodes in this community are weakly interconnected._