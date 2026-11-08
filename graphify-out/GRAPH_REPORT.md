# Graph Report - Punto iOS  (2026-11-08)

## Corpus Check
- 244 files · ~69,540 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 8 file(s) not represented in the graph (top: (none) 5, .example 1, .ico 1)

## Summary
- 1899 nodes · 5700 edges · 84 communities (60 shown, 24 thin omitted)
- Extraction: 93% EXTRACTED · 7% INFERRED · 0% AMBIGUOUS · INFERRED: 423 edges (avg confidence: 0.84)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `a4cc02c3`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- Punto iOS — Software Design Document (SDD)
- PuntoIOS.ArchitectureTests.csproj
- 3. Fases
- 0001-decisiones-iniciales.md
- IAuditoriaRepositorio.cs
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
- PuntoIOS.Domain.Reparaciones
- CurrentUser
- Rol
- react
- CancellationToken
- login-form.tsx
- Fact
- reparaciones/api.ts
- cliente-detalle.tsx
- clientes/api.ts
- MaquinaDeEstadosTests
- DependencyInjection.cs
- LocalUnico
- DateTimeOffset
- OrdenReparacion
- Cliente
- RecibirEquipoHandlerTests
- CrearClienteHandlerTests
- IReadOnlyList
- RecibirEquipoHandler.ValidarDatos
- LayerDependencyTests
- RegistroAuditoria
- ServiceCollectionExtensions.AddCasosDeUso
- ObtenerUsuarioActualHandler
- FotoOrden
- PuntoIOS.Domain.Auditoria
- CancellationToken
- package.json
- IdentificarUsuarioHandler
- Guid
- puntoios_domain_clientes
- EntityTypeBuilder
- UsuariosController.Actualizar
- PostgresReadyCheck
- IdentificarUsuarioHandler.cs
- CifradoDePrueba
- AuditoriaRepoEnMemoria
- RolConfiguration
- vitest
- PageRequest
- IOrdenReparacionRepositorio
- RecibirEquipoHandler.HandleAsync
- ChecklistYFotosTests
- CodigoSeguimiento.Generar
- Stream
- PermisoConfiguration
- PuntoIOS.Application.Common.Security
- DependencyInjection.AddInfrastructure
- Session state (auto-saved — last known reality)
- DotEnv
- reparaciones-page.tsx
- Theory
- CifradoConDataProtection

## God Nodes (most connected - your core abstractions)
1. `OrdenReparacion` - 50 edges
2. `Usuario` - 39 edges
3. `PuntoIosDbContext` - 39 edges
4. `Cliente` - 31 edges
5. `useAuth()` - 31 edges
6. `PuntoIOS.Domain.Identidad` - 30 edges
7. `OrdenDetalle()` - 29 edges
8. `OrdenReparacionDto` - 29 edges
9. `PuntoIOS.Domain.Auditoria` - 28 edges
10. `Rol` - 27 edges

## Surprising Connections (you probably didn't know these)
- `TestApi` --references--> `Program`  [EXTRACTED]
  backend/tests/PuntoIOS.IntegrationTests/TestApi.cs → backend/src/PuntoIOS.Api/Program.cs
- `ListaOrdenes()` --calls--> `nombreDeEstado()`  [EXTRACTED]
  frontend/src/features/reparaciones/lista-ordenes.tsx → frontend/src/features/reparaciones/api.ts
- `listarOrdenes()` --calls--> `apiGet()`  [EXTRACTED]
  frontend/src/features/reparaciones/api.ts → frontend/src/lib/api/client.ts
- `recibirEquipo()` --calls--> `apiPost()`  [EXTRACTED]
  frontend/src/features/reparaciones/api.ts → frontend/src/lib/api/client.ts
- `cambiarEstadoDeOrden()` --calls--> `apiPost()`  [EXTRACTED]
  frontend/src/features/reparaciones/api.ts → frontend/src/lib/api/client.ts

## Import Cycles
- None detected.

## Communities (84 total, 24 thin omitted)

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
Cohesion: 0.12
Nodes (15): CambioDeCampo, Dispositivo, Almacenamiento, ClienteId, Color, CreadoEn, CreadoPorUsuarioId, Id (+7 more)

### Community 6 - "use-remote-checks.ts"
Cohesion: 0.15
Nodes (14): Home(), REMOTE_CHECKS, RemoteCheckId, Row, RowState, StatusBoard(), subscribeToConnection(), useOnline() (+6 more)

### Community 7 - "DispositivoHandlersTests"
Cohesion: 0.25
Nodes (5): ActualizarDispositivoCommand, DispositivoHandlersTests, UsuarioFijo, RelojFijo, Ahora

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
Cohesion: 0.08
Nodes (21): ActualizarUsuarioHandler, UsuarioAutorizado, IRolRepositorio, IUsuarioRepositorio, Usuario, Activo, CreadoEn, Email (+13 more)

### Community 16 - "PuntoIosDbContext"
Cohesion: 0.11
Nodes (14): DesignTimeDbContextFactory, InicializadorBaseDeDatos, PuntoIosDbContext, Auditoria, Clientes, Dispositivos, FotosOrden, LocalActualId (+6 more)

### Community 19 - "ServiceCollectionExtensions.cs"
Cohesion: 0.07
Nodes (6): CambiarEstadoRequest, ClienteResumenDto, PuntoIOS.Application.Features.Reparaciones.ActualizarDatosTecnicos, PuntoIOS.Api.Seguridad, PuntoIOS.Application.Features.Clientes, PuntoIOS.Application.Features.Clientes.SeguimientoCliente

### Community 20 - "auth-provider.tsx"
Cohesion: 0.13
Nodes (17): dependencies, firebase@^13.2.0, next@16.4.5, react@19.3.0, body, display, metadata, RootLayout() (+9 more)

### Community 22 - "seguimiento-cliente.tsx"
Cohesion: 0.18
Nodes (20): formatoFecha, Props, SeguimientoCliente(), conToken(), enviarPorWhatsapp(), generar(), ver(), ana (+12 more)

### Community 23 - "TestApi.cs"
Cohesion: 0.20
Nodes (6): HistorialDeAuditoria, PuntoIOS.Api.Controllers, PuntoIOS.IntegrationTests, PuntoIOS.Application.Features.Dispositivos, PuntoIOS.Application.Features.Auditoria, PuntoIOS.Application.Features.Reparaciones

### Community 24 - "Task"
Cohesion: 0.14
Nodes (5): RegistroHistorialDto, ClienteRepositorio, ClienteRepoEnMemoria, Clientes, Guardados

### Community 26 - "PuntoIOS.Domain.Identidad"
Cohesion: 0.09
Nodes (15): ObtenerClienteHandler, Permiso, Codigo, Descripcion, RolesDeSistema, PuntoIOS.Application.Common.Results, PuntoIOS.Application.Features.Dispositivos.HistorialDispositivo, PuntoIOS.UnitTests.Identidad (+7 more)

### Community 27 - "Resultado"
Cohesion: 0.10
Nodes (15): ResultadoExtensions, Error, Resultado, Error, EsExitoso, Valor, TipoError, Conflicto (+7 more)

### Community 28 - "PuntoIOS.Domain.Reparaciones"
Cohesion: 0.22
Nodes (6): PuntoIOS.Application.Features.Reparaciones.VerDesbloqueo, PuntoIOS.Application.Features.Reparaciones.RecibirEquipo, PuntoIOS.Application.Features.Reparaciones.CambiarEstado, PuntoIOS.UnitTests.Reparaciones, PuntoIOS.Application.Features.Reparaciones.Fotos, PuntoIOS.Domain.Reparaciones

### Community 31 - "Rol"
Cohesion: 0.09
Nodes (14): ActualizarUsuarioCommand, RegistrarUsuarioCommand, Rol, Descripcion, EsDeSistema, Id, Nombre, Permisos (+6 more)

### Community 32 - "react"
Cohesion: 0.18
Nodes (13): ClienteRoute(), Detalle(), PanelPage(), Detalle(), OrdenRoute(), ReparacionesRoute(), RequireAuth(), ServerIdentity() (+5 more)

### Community 33 - "CancellationToken"
Cohesion: 0.12
Nodes (7): FiltroDeOrdenes, OrdenReparacionRepositorio, OrdenRepoEnMemoria, Fotos, Guardados, Ordenes, ListarOrdenesHandlerTests

### Community 34 - "login-form.tsx"
Cohesion: 0.19
Nodes (12): OjoAbierto(), OjoCerrado(), PasswordInput(), Props, describeAuthError(), LoginForm(), onGoogle(), onSubmit() (+4 more)

### Community 35 - "Fact"
Cohesion: 0.05
Nodes (24): ActualizarClienteRequest, CrearClienteRequest, EquipoNuevoRequest, ActualizarDispositivoRequest, CrearDispositivoRequest, ActualizarDatosTecnicosRequest, ChecklistRequest, RecibirEquipoRequest (+16 more)

### Community 36 - "reparaciones/api.ts"
Cohesion: 0.06
Nodes (55): actualizarDatosTecnicos(), AVISO_FECHA_ESTIMADA, cambiarEstadoDeOrden(), Checklist, CHECKLIST_CATALOGO, DatosTecnicos, ESTADOS, Foto (+47 more)

### Community 37 - "cliente-detalle.tsx"
Cohesion: 0.10
Nodes (29): ClientesRoute(), useAuth(), SessionBar(), useMe(), actualizarCliente(), CambioDeCampo, Cliente, crearCliente() (+21 more)

### Community 38 - "clientes/api.ts"
Cohesion: 0.10
Nodes (31): Coincidencia, DatosCliente, Aviso, Props, actualizarDispositivo(), crearDispositivo(), DatosDispositivo, Dispositivo (+23 more)

### Community 40 - "DependencyInjection.cs"
Cohesion: 0.14
Nodes (6): Program, AssemblyMarker, PuntoIOS.Application.Common.Archivos, PuntoIOS.Infrastructure, PuntoIOS.Api.Extensions, PuntoIOS.Infrastructure.Archivos

### Community 41 - "LocalUnico"
Cohesion: 0.17
Nodes (10): ILocalActual, LocalId, Local, Id, Nombre, LocalesDeSistema, LocalUnico, LocalId (+2 more)

### Community 42 - "DateTimeOffset"
Cohesion: 0.07
Nodes (14): OrdenResumenDto, InicialIdentidad, ClientesYLocales, AuditoriaYEdicionClientes, DispositivosDeClientes, CodigoSeguimientoClientes, OrdenesDeReparacion, ChecklistYFotosDeOrdenes (+6 more)

### Community 43 - "OrdenReparacion"
Cohesion: 0.07
Nodes (26): OrdenReparacion, ChecklistItems, ClienteId, ComponentesAfectados, CreadoEn, CreadoPorUsuarioId, DesbloqueoCifrado, DesbloqueoTipo (+18 more)

### Community 44 - "Cliente"
Cohesion: 0.10
Nodes (18): ActualizarClienteCommand, Cliente, Apellido, CodigoSeguimientoCifrado, CodigoSeguimientoGeneradoEn, CodigoSeguimientoHuella, CreadoEn, CreadoPorUsuarioId (+10 more)

### Community 45 - "RecibirEquipoHandlerTests"
Cohesion: 0.32
Nodes (4): RecibirEquipoCommand, DesbloqueoDto, RecibirEquipoHandlerTests, Invalidos

### Community 47 - "IReadOnlyList"
Cohesion: 0.15
Nodes (5): IDirectorioDeUsuarios, UsuarioResumen, RolDto, DirectorioDeUsuarios, DirectorioFijo

### Community 48 - "RecibirEquipoHandler.ValidarDatos"
Cohesion: 0.23
Nodes (3): EmailValidator, ClienteValidacion, PuntoIOS.Application.Common.Validation

### Community 50 - "RegistroAuditoria"
Cohesion: 0.13
Nodes (13): AccionesAuditadas, EntidadesAuditadas, RegistroAuditoria, Accion, Cambios, Entidad, EntidadId, Fecha (+5 more)

### Community 51 - "ServiceCollectionExtensions.AddCasosDeUso"
Cohesion: 0.06
Nodes (7): ClientesController, MeController, ReparacionesController, RolesController, ServiceCollectionExtensions, ListarRolesHandler, PuntoIOS.Application.Features.Identidad.ListarRoles

### Community 52 - "ObtenerUsuarioActualHandler"
Cohesion: 0.33
Nodes (4): ObtenerUsuarioActualHandler, FakeAccessor, ObtenerUsuarioActualHandlerTests, PuntoIOS.Application.Features.Identidad.ObtenerUsuarioActual

### Community 53 - "FotoOrden"
Cohesion: 0.18
Nodes (9): FotoOrden, CreadoEn, CreadoPorUsuarioId, Id, LocalId, OrdenId, Ruta, Tamano (+1 more)

### Community 54 - "PuntoIOS.Domain.Auditoria"
Cohesion: 0.14
Nodes (11): PuntoIOS.UnitTests.Clientes, PuntoIOS.Application.Features.Clientes.ActualizarCliente, PuntoIOS.Application.Features.Dispositivos.CrearDispositivo, PuntoIOS.Application.Features.Dispositivos.ActualizarDispositivo, PuntoIOS.Application.Features.Dispositivos.ListarDispositivos, PuntoIOS.Application.Features.Dispositivos.RevelarImei, PuntoIOS.Application.Features.Clientes.CrearCliente, PuntoIOS.Application.Features.Dispositivos.Puertos (+3 more)

### Community 55 - "CancellationToken"
Cohesion: 0.15
Nodes (4): DispositivoRepositorio, DispositivoRepoEnMemoria, Dispositivos, Guardados

### Community 56 - "package.json"
Cohesion: 0.05
Nodes (42): CORS, eslintConfig, react-dom@19.3.0, devDependencies, eslint@^9, eslint-config-next@16.4.5, jsdom@^30.1.3, @playwright/test@^1.64.1 (+34 more)

### Community 58 - "Guid"
Cohesion: 0.13
Nodes (7): TipoReparacionDto, ClienteDto, CrearClienteCommand, EquipoNuevo, CodigoSeguimientoDto, UsuarioActualDto, FotoDto

### Community 59 - "puntoios_domain_clientes"
Cohesion: 0.13
Nodes (6): BuscarClientesHandler, PuntoIOS.Infrastructure.Persistence.Configurations, PuntoIOS.Application.Features.Reparaciones.Puertos, PuntoIOS.Application.Features.Clientes.Puertos, PuntoIOS.Application.Features.Clientes.BuscarClientes, PuntoIOS.Application.Features.Reparaciones.ConsultarOrdenes

### Community 60 - "EntityTypeBuilder"
Cohesion: 0.23
Nodes (6): ClienteConfiguration, DispositivoConfiguration, LocalConfiguration, UsuarioConfiguration, FotoOrdenConfiguration, OrdenReparacionConfiguration

### Community 61 - "UsuariosController.Actualizar"
Cohesion: 0.33
Nodes (4): ActualizarUsuarioRequest, UsuariosController, ListarUsuariosHandler, UsuarioDto

### Community 63 - "IdentificarUsuarioHandler.cs"
Cohesion: 0.29
Nodes (7): EstadoIdentificacion, Autorizado, Desactivado, NoRegistrado, ResultadoIdentificacion, Desactivado, NoRegistrado

### Community 67 - "vitest"
Cohesion: 0.05
Nodes (30): mocks, ana, Handler, historial, mocks, ana, Handler, mocks (+22 more)

### Community 70 - "PageRequest"
Cohesion: 0.14
Nodes (10): PagedResult, TotalPages, PageRequest, Page, PageSize, Skip, PagedResultTests, PageRequestTests (+2 more)

### Community 71 - "IOrdenReparacionRepositorio"
Cohesion: 0.19
Nodes (24): DispositivosController, ActualizarClienteHandler, CrearClienteHandler, HistorialClienteHandler, ConsultarCodigoSeguimientoHandler, GenerarCodigoSeguimientoHandler, ActualizarDispositivoHandler, CrearDispositivoHandler (+16 more)

### Community 72 - "RecibirEquipoHandler.HandleAsync"
Cohesion: 0.18
Nodes (3): IClienteRepositorio, DispositivoValidacion, ArmadorDeOrdenes

### Community 73 - "ChecklistYFotosTests"
Cohesion: 0.26
Nodes (6): ChecklistIngreso, ItemChecklist, ItemsDeChecklist, LocalFijo, LocalId, ChecklistYFotosTests

### Community 74 - "CodigoSeguimiento.Generar"
Cohesion: 0.08
Nodes (8): FormatoDeImagen, ActualizarDatosTecnicosCommand, CambiarEstadoOrdenCommand, CodigoSeguimiento, CodigoSeguimientoTests, SeguimientoClienteHandlersTests, CambiarEstadoOrdenHandlerTests, ActualizarDatosTecnicosHandlerTests

### Community 75 - "Stream"
Cohesion: 0.18
Nodes (6): IAlmacenDeArchivos, ArchivoDeFoto, ObtenerFotoHandler, AlmacenEnDisco, AlmacenEnMemoria, Archivos

### Community 78 - "PuntoIOS.Application.Common.Security"
Cohesion: 0.16
Nodes (6): FirebaseClaims, UsuarioLocalClaims, ICurrentUserAccessor, PuntoIOS.Application.Features.Identidad.IdentificarUsuario, PuntoIOS.Application.Common.Security, PuntoIOS.Api.Authentication

### Community 82 - "Session state (auto-saved — last known reality)"
Cohesion: 0.40
Nodes (4): Recent commits, Resume, Session state (auto-saved — last known reality), Uncommitted changes

### Community 84 - "reparaciones-page.tsx"
Cohesion: 0.14
Nodes (15): nextConfig, IngresarPage(), metadata, BackLink(), buscarClientes(), formatearFecha(), listarOrdenes(), OrdenResumen (+7 more)

### Community 93 - "Theory"
Cohesion: 0.17
Nodes (3): CrearDispositivoCommand, AutorizacionTests, Endpoints

## Knowledge Gaps
- **168 isolated node(s):** `Microsoft.EntityFrameworkCore.Design (10.0.14)`, `Microsoft.EntityFrameworkCore.Design (10.0.14)`, `coverlet.collector (6.0.4)`, `Microsoft.NET.Test.Sdk (17.14.1)`, `xunit (2.9.3)` (+163 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 506 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **24 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `OrdenReparacion` connect `OrdenReparacion` to `MaquinaDeEstadosTests`, `RecibirEquipoHandler.HandleAsync`, `DateTimeOffset`, `CodigoSeguimiento.Generar`, `IReadOnlyList`, `PuntoIosDbContext`, `FotoOrden`, `Task`, `Fact`, `Guid`, `EntityTypeBuilder`?**
  _High betweenness centrality (0.053) - this node is a cross-community bridge._
- **What connects `Microsoft.EntityFrameworkCore.Design (10.0.14)`, `Microsoft.EntityFrameworkCore.Design (10.0.14)`, `coverlet.collector (6.0.4)` to the rest of the system?**
  _168 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Punto iOS — Software Design Document (SDD)` be split into smaller, more focused modules?**
  _Cohesion score 0.09090909090909091 - nodes in this community are weakly interconnected._
- **Why does `PuntoIosDbContext` connect `PuntoIosDbContext` to `RolConfiguration`, `Fact`, `IOrdenReparacionRepositorio`, `OrdenReparacion`, `PermisoConfiguration`, `IReadOnlyList`, `DependencyInjection.AddInfrastructure`, `Usuario`, `RegistroAuditoria`, `FotoOrden`, `CancellationToken`, `Task`, `Guid`, `puntoios_domain_clientes`, `EntityTypeBuilder`?**
  _High betweenness centrality (0.035) - this node is a cross-community bridge._
- **Should `PuntoIOS.ArchitectureTests.csproj` be split into smaller, more focused modules?**
  _Cohesion score 0.059800664451827246 - nodes in this community are weakly interconnected._
- **Why does `ReparacionesController` connect `ServiceCollectionExtensions.AddCasosDeUso` to `Guid`, `ServiceCollectionExtensions.cs`, `IOrdenReparacionRepositorio`?**
  _High betweenness centrality (0.019) - this node is a cross-community bridge._
- **Should `3. Fases` be split into smaller, more focused modules?**
  _Cohesion score 0.09523809523809523 - nodes in this community are weakly interconnected._