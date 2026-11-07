# Graph Report - Punto iOS  (2026-11-07)

## Corpus Check
- 193 files · ~41,217 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 8 file(s) not represented in the graph (top: (none) 5, .example 1, .ico 1)

## Summary
- 1379 nodes · 3614 edges · 69 communities (49 shown, 20 thin omitted)
- Extraction: 93% EXTRACTED · 7% INFERRED · 0% AMBIGUOUS · INFERRED: 241 edges (avg confidence: 0.83)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `6efb0670`
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
- UsuariosEndpointTests
- ServiceCollectionExtensions.AddCasosDeUso
- cliente-detalle.tsx
- clientes/api.ts
- IReadOnlyList
- Dispositivo
- api/client.ts
- TestApi.cs
- CurrentUser
- Cliente
- DispositivosEndpointTests
- Theory
- IClienteRepositorio
- TestApi.CrearAsync
- LayerDependencyTests
- IDirectorioDeUsuarios.ObtenerNombresAsync
- ServiceCollectionExtensions.AddCasosDeUso
- IdentificarUsuarioHandler.HandleAsync
- scripts
- DispositivosController.cs
- IDispositivoRepositorio.ObtenerPorImeiAsync
- devDependencies
- PostgresFixture
- Guid
- ClienteRepoEnMemoria
- SeguimientoClienteEndpointTests.Genera_consulta_y_regenera_el_codigo_sin_guardarlo_en_texto_plano
- @playwright/test
- HealthEndpointTests
- eslint.config.mjs
- dependencies
- ClienteValidacion.cs
- Permiso
- vitest.config.mts
- @testing-library/jest-dom

## God Nodes (most connected - your core abstractions)
1. `Dispositivo` - 40 edges
2. `Cliente` - 39 edges
3. `Usuario` - 39 edges
4. `PuntoIosDbContext` - 34 edges
5. `PuntoIOS.Domain.Identidad` - 32 edges
6. `Rol` - 27 edges
7. `RegistroAuditoria` - 26 edges
8. `PuntoIOS.Domain.Clientes` - 26 edges
9. `useAuth()` - 23 edges
10. `PuntoIOS.Domain.Auditoria` - 22 edges

## Surprising Connections (you probably didn't know these)
- `ConsultarCodigoSeguimientoHandler` --references--> `ICifradoDeDatos`  [EXTRACTED]
  backend/src/PuntoIOS.Application/Features/Clientes/SeguimientoCliente/CodigoSeguimientoHandlers.cs → backend/src/PuntoIOS.Application/Common/Security/ICifradoDeDatos.cs
- `GenerarCodigoSeguimientoHandler` --references--> `ICifradoDeDatos`  [EXTRACTED]
  backend/src/PuntoIOS.Application/Features/Clientes/SeguimientoCliente/CodigoSeguimientoHandlers.cs → backend/src/PuntoIOS.Application/Common/Security/ICifradoDeDatos.cs
- `RegistroHistorialDto` --references--> `CambioDeCampo`  [EXTRACTED]
  backend/src/PuntoIOS.Application/Features/Auditoria/HistorialDeAuditoria.cs → backend/src/PuntoIOS.Domain/Auditoria/RegistroAuditoria.cs
- `ClienteConfiguration` --references--> `Cliente`  [EXTRACTED]
  backend/src/PuntoIOS.Infrastructure/Persistence/Configurations/ClientesConfiguraciones.cs → backend/src/PuntoIOS.Domain/Clientes/Cliente.cs
- `SeguimientoClienteHandlersTests` --references--> `Cliente`  [EXTRACTED]
  backend/tests/PuntoIOS.UnitTests/Clientes/SeguimientoClienteHandlersTests.cs → backend/src/PuntoIOS.Domain/Clientes/Cliente.cs

## Import Cycles
- None detected.

## Communities (69 total, 20 thin omitted)

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
Nodes (9): jsdom@^30.1.3, tailwindcss@^4, @types/react-dom@^19, name, private, version, jsdom, tailwindcss (+1 more)

### Community 5 - "Fact"
Cohesion: 0.30
Nodes (3): CrearClienteRequest, EquipoNuevoRequest, ClientesEndpointTests

### Community 6 - "use-remote-checks.ts"
Cohesion: 0.15
Nodes (14): Home(), REMOTE_CHECKS, RemoteCheckId, Row, RowState, StatusBoard(), subscribeToConnection(), useOnline() (+6 more)

### Community 7 - "DispositivoHandlersTests"
Cohesion: 0.07
Nodes (14): CrearClienteCommand, EquipoNuevo, ActualizarDispositivoCommand, CrearDispositivoCommand, CodigoSeguimiento, CodigoSeguimientoTests, CrearClienteHandlerTests, DispositivoHandlersTests (+6 more)

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
Cohesion: 0.07
Nodes (22): ActualizarUsuarioRequest, UsuariosController, ActualizarUsuarioHandler, IdentificarUsuarioHandler, ListarUsuariosHandler, IUsuarioRepositorio, UsuarioDto, Usuario (+14 more)

### Community 16 - "PuntoIosDbContext"
Cohesion: 0.05
Nodes (32): RegistroAuditoria, Accion, Cambios, Entidad, EntidadId, Fecha, Id, LocalId (+24 more)

### Community 19 - "ServiceCollectionExtensions.cs"
Cohesion: 0.09
Nodes (5): ActualizarClienteRequest, ICurrentUserAccessor, PuntoIOS.Application.Common.Security, PuntoIOS.Api.Seguridad, PuntoIOS.Application.Features.Clientes.SeguimientoCliente

### Community 20 - "auth-provider.tsx"
Cohesion: 0.17
Nodes (13): body, display, metadata, RootLayout(), viewport, AuthContext, AuthContextValue, AuthProvider() (+5 more)

### Community 22 - "seguimiento-cliente.tsx"
Cohesion: 0.18
Nodes (20): formatoFecha, Props, SeguimientoCliente(), conToken(), enviarPorWhatsapp(), generar(), ver(), ana (+12 more)

### Community 23 - "TestApi.cs"
Cohesion: 0.22
Nodes (4): PuntoIOS.Api.Controllers, PuntoIOS.IntegrationTests, PuntoIOS.Application.Features.Dispositivos, PuntoIOS.Application.Features.Auditoria

### Community 25 - "DateTimeOffset"
Cohesion: 0.07
Nodes (18): ILocalActual, LocalId, Local, Id, Nombre, LocalesDeSistema, LocalUnico, LocalId (+10 more)

### Community 26 - "PuntoIOS.Domain.Identidad"
Cohesion: 0.09
Nodes (15): ResultadoExtensions, RolesController, ListarRolesHandler, Permisos, RolesDeSistema, PuntoIOS.Application.Common.Results, PuntoIOS.UnitTests.Identidad, PuntoIOS.Application.Features.Identidad.ActualizarUsuario (+7 more)

### Community 27 - "Resultado"
Cohesion: 0.23
Nodes (9): Error, Resultado, Error, EsExitoso, Valor, TipoError, Conflicto, NoEncontrado (+1 more)

### Community 28 - "puntoios_application_common_pagination"
Cohesion: 0.18
Nodes (6): HistorialDeAuditoria, IDirectorioDeUsuarios, DirectorioDeUsuarios, PuntoIOS.Application.Features.Clientes.Puertos, PuntoIOS.Application.Features.Clientes.BuscarClientes, PuntoIOS.Application.Features.Auditoria.Puertos

### Community 30 - "equipos-cliente.tsx"
Cohesion: 0.17
Nodes (9): listarDispositivos(), revelarImei(), EquiposCliente(), verImei(), Lista, Props, iphone, mocks (+1 more)

### Community 31 - "Rol"
Cohesion: 0.08
Nodes (15): ActualizarUsuarioCommand, RegistrarUsuarioCommand, Rol, Descripcion, EsDeSistema, Id, Nombre, Permisos (+7 more)

### Community 32 - "clientes-page.tsx"
Cohesion: 0.14
Nodes (15): nextConfig, ClientesRoute(), PanelPage(), metadata, useAuth(), RequireAuth(), mocks, ServerIdentity() (+7 more)

### Community 33 - "FakesClientes.cs"
Cohesion: 0.17
Nodes (7): PuntoIOS.UnitTests.Clientes, PuntoIOS.Application.Features.Clientes.ActualizarCliente, PuntoIOS.Application.Features.Clientes.CrearCliente, PuntoIOS.Application.Features.Dispositivos.Puertos, PuntoIOS.Application.Features.Clientes.HistorialCliente, PuntoIOS.Domain.Clientes, PuntoIOS.Domain.Auditoria

### Community 34 - "login-form.tsx"
Cohesion: 0.11
Nodes (18): OjoAbierto(), OjoCerrado(), PasswordInput(), Props, describeAuthError(), LoginForm(), onGoogle(), onSubmit() (+10 more)

### Community 36 - "ServiceCollectionExtensions.AddCasosDeUso"
Cohesion: 0.25
Nodes (15): ClientesController, DispositivosController, IAuditoriaRepositorio, ActualizarClienteHandler, CrearClienteHandler, HistorialClienteHandler, ConsultarCodigoSeguimientoHandler, GenerarCodigoSeguimientoHandler (+7 more)

### Community 37 - "cliente-detalle.tsx"
Cohesion: 0.11
Nodes (17): ClienteRoute(), Detalle(), IngresarPage(), BackLink(), actualizarCliente(), historialDeCliente(), obtenerCliente(), ACCIONES (+9 more)

### Community 38 - "clientes/api.ts"
Cohesion: 0.09
Nodes (33): CambioDeCampo, Cliente, Coincidencia, crearCliente(), DatosCliente, EquipoNuevo, NuevoCliente, Pagina (+25 more)

### Community 39 - "IReadOnlyList"
Cohesion: 0.25
Nodes (5): UsuarioAutorizado, IRolRepositorio, RolDto, RolRepositorio, RolRepositorioEnMemoria

### Community 40 - "Dispositivo"
Cohesion: 0.05
Nodes (30): PagedResult, TotalPages, PageRequest, Page, PageSize, Skip, AccionesAuditadas, CambioDeCampo (+22 more)

### Community 41 - "api/client.ts"
Cohesion: 0.29
Nodes (8): State, EstadoMe, UsuarioActual, ApiError, apiGet(), apiPut(), ProblemDetails, request()

### Community 42 - "TestApi.cs"
Cohesion: 0.06
Nodes (10): FirebaseClaims, HttpContextCurrentUserAccessor, UsuarioLocalClaims, DotEnv, Program, AssemblyMarker, PuntoIOS.Api.Configuration, PuntoIOS.Infrastructure (+2 more)

### Community 43 - "CurrentUser"
Cohesion: 0.15
Nodes (8): MeController, CurrentUser, ObtenerUsuarioActualHandler, UsuarioActualDto, IdentificarUsuarioHandlerTests, FakeAccessor, ObtenerUsuarioActualHandlerTests, PuntoIOS.Application.Features.Identidad.ObtenerUsuarioActual

### Community 44 - "Cliente"
Cohesion: 0.07
Nodes (22): EmailValidator, ClienteResumenDto, ClienteValidacion, DispositivoValidacion, Cliente, Apellido, CodigoSeguimientoCifrado, CodigoSeguimientoGeneradoEn (+14 more)

### Community 45 - "DispositivosEndpointTests"
Cohesion: 0.42
Nodes (3): ActualizarDispositivoRequest, CrearDispositivoRequest, DispositivosEndpointTests

### Community 47 - "IClienteRepositorio"
Cohesion: 0.22
Nodes (4): BuscarClientesHandler, ObtenerClienteHandler, IClienteRepositorio, ClienteRepositorio

### Community 51 - "ServiceCollectionExtensions.AddCasosDeUso"
Cohesion: 0.05
Nodes (6): ServiceCollectionExtensions, CifradoConDataProtection, ICifradoDeDatos, RegistrarUsuarioHandler, DependencyInjection, CifradoDePrueba

### Community 52 - "IdentificarUsuarioHandler.HandleAsync"
Cohesion: 0.25
Nodes (8): EstadoIdentificacion, Autorizado, Desactivado, NoRegistrado, ResultadoIdentificacion, Desactivado, NoRegistrado, PuntoIOS.Application.Features.Identidad.IdentificarUsuario

### Community 53 - "scripts"
Cohesion: 0.25
Nodes (8): scripts, build, dev, lint, start, test, test:e2e, typecheck

### Community 54 - "DispositivosController.cs"
Cohesion: 0.31
Nodes (6): PuntoIOS.Application.Features.Dispositivos.HistorialDispositivo, PuntoIOS.Application.Features.Dispositivos.CrearDispositivo, PuntoIOS.Application.Features.Dispositivos.ActualizarDispositivo, PuntoIOS.Application.Features.Dispositivos.ListarDispositivos, PuntoIOS.Application.Features.Dispositivos.RevelarImei, PuntoIOS.Application.Features.Dispositivos.ObtenerDispositivo

### Community 56 - "devDependencies"
Cohesion: 0.18
Nodes (11): devDependencies, @tailwindcss/turbopack@^4, @testing-library/react@^16.3.3, @types/node@^24.19.2, @types/react@^19, typescript@^5, vitest@^5.1.0, @tailwindcss/turbopack (+3 more)

### Community 58 - "Guid"
Cohesion: 0.17
Nodes (5): ActualizarClienteCommand, ClienteDto, CodigoSeguimientoDto, DispositivoDto, ImeiDto

### Community 59 - "ClienteRepoEnMemoria"
Cohesion: 0.33
Nodes (3): ClienteRepoEnMemoria, Clientes, Guardados

### Community 61 - "@playwright/test"
Cohesion: 0.29
Nodes (3): CORS, @playwright/test@^1.64.1, @playwright/test

### Community 63 - "eslint.config.mjs"
Cohesion: 0.33
Nodes (5): eslintConfig, eslint@^9, eslint-config-next@16.4.5, eslint, eslint-config-next

### Community 64 - "dependencies"
Cohesion: 0.33
Nodes (6): dependencies, firebase@^13.2.0, next@16.4.5, react@19.3.0, react-dom@19.3.0, react-dom

### Community 66 - "Permiso"
Cohesion: 0.50
Nodes (3): Permiso, Codigo, Descripcion

## Knowledge Gaps
- **128 isolated node(s):** `Id`, `LocalId`, `Entidad`, `EntidadId`, `Accion` (+123 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 413 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **20 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `PuntoIosDbContext` connect `PuntoIosDbContext` to `FakesClientes.cs`, `ServiceCollectionExtensions.AddCasosDeUso`, `IReadOnlyList`, `Dispositivo`, `IClienteRepositorio`, `Usuario`, `ServiceCollectionExtensions.AddCasosDeUso`, `IDispositivoRepositorio.ObtenerPorImeiAsync`, `Guid`, `ClienteRepoEnMemoria`, `puntoios_application_common_pagination`, `Rol`?**
  _High betweenness centrality (0.049) - this node is a cross-community bridge._
- **What connects `Id`, `LocalId`, `Entidad` to the rest of the system?**
  _128 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Punto iOS — Software Design Document (SDD)` be split into smaller, more focused modules?**
  _Cohesion score 0.09090909090909091 - nodes in this community are weakly interconnected._
- **Why does `PuntoIOS.Domain.Identidad` connect `PuntoIOS.Domain.Identidad` to `FakesClientes.cs`, `Permiso`, `TestApi.cs`, `Usuario`, `PuntoIosDbContext`, `ServiceCollectionExtensions.cs`, `IdentificarUsuarioHandler.HandleAsync`, `DispositivosController.cs`, `TestApi.cs`, `DateTimeOffset`, `Rol`?**
  _High betweenness centrality (0.049) - this node is a cross-community bridge._
- **Should `PuntoIOS.ArchitectureTests.csproj` be split into smaller, more focused modules?**
  _Cohesion score 0.059800664451827246 - nodes in this community are weakly interconnected._
- **Why does `Cliente` connect `Cliente` to `FakesClientes.cs`, `DispositivoHandlersTests`, `IClienteRepositorio`, `PuntoIosDbContext`, `Task`, `DateTimeOffset`, `Guid`?**
  _High betweenness centrality (0.029) - this node is a cross-community bridge._
- **Should `3. Fases` be split into smaller, more focused modules?**
  _Cohesion score 0.09523809523809523 - nodes in this community are weakly interconnected._