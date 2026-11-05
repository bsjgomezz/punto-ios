# Graph Report - Punto iOS  (2026-11-05)

## Corpus Check
- 74 files · ~10,898 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 8 file(s) not represented in the graph (top: (none) 5, .example 1, .ico 1)

## Summary
- 429 nodes · 618 edges · 32 communities (20 shown, 12 thin omitted)
- Extraction: 97% EXTRACTED · 3% INFERRED · 0% AMBIGUOUS · INFERRED: 20 edges (avg confidence: 0.84)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `c3e136dc`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- Punto iOS — Software Design Document (SDD)
- PuntoIOS.ArchitectureTests.csproj
- 3. Fases
- 0001-decisiones-iniciales.md
- devDependencies
- Fact
- use-remote-checks.ts
- PageRequest
- Instrucciones para Codex / Claude Code — Punto iOS
- Punto iOS
- PostgresReadyCheck
- postgres
- CLAUDE.md
- compilerOptions
- http
- scripts
- auth-provider.tsx
- PuntoIOS.Application/AssemblyMarker.cs
- PuntoIOS.Domain/AssemblyMarker.cs
- PuntoIOS.Infrastructure/AssemblyMarker.cs
- login-form.tsx
- ObtenerUsuarioActualHandlerTests.Devuelve_la_identidad_del_usuario_autenticado
- MeEndpointTests.cs
- LayerDependencyTests
- package.json
- @playwright/test
- eslint.config.mjs
- dependencies
- DotEnv
- @testing-library/jest-dom
- @types/react-dom@^19

## God Nodes (most connected - your core abstractions)
1. `Punto iOS — Software Design Document (SDD)` - 19 edges
2. `compilerOptions` - 16 edges
3. `3. Fases` - 14 edges
4. `LoginForm()` - 11 edges
5. `MeEndpointTests` - 10 edges
6. `PageRequest` - 9 edges
7. `useAuth()` - 9 edges
8. `next` - 9 edges
9. `vitest` - 9 edges
10. `HealthEndpointTests` - 8 edges

## Surprising Connections (you probably didn't know these)
- `IngresarPage()` --calls--> `LoginForm()`  [EXTRACTED]
  frontend/src/app/ingresar/page.tsx → frontend/src/features/auth/login-form.tsx
- `LoginForm()` --indirect_call--> `readRememberedEmail()`  [INFERRED]
  frontend/src/features/auth/login-form.tsx → frontend/src/features/auth/remembered-email.ts
- `Home()` --calls--> `StatusBoard()`  [EXTRACTED]
  frontend/src/app/page.tsx → frontend/src/features/sistema/status-board.tsx
- `HttpContextCurrentUserAccessor` --implements--> `ICurrentUserAccessor`  [EXTRACTED]
  backend/src/PuntoIOS.Api/Authentication/HttpContextCurrentUserAccessor.cs → backend/src/PuntoIOS.Application/Common/Security/ICurrentUserAccessor.cs
- `MeController` --references--> `ObtenerUsuarioActualHandler`  [EXTRACTED]
  backend/src/PuntoIOS.Api/Controllers/MeController.cs → backend/src/PuntoIOS.Application/Features/Identidad/ObtenerUsuarioActual/ObtenerUsuarioActualHandler.cs

## Import Cycles
- None detected.

## Communities (32 total, 12 thin omitted)

### Community 0 - "Punto iOS — Software Design Document (SDD)"
Cohesion: 0.09
Nodes (21): 10. Notificaciones internas, 11. PWA y offline, 12. API, 13. Seguridad y auditoría, 14. Pruebas, 15. Diseño visual, 16. Observabilidad, 17. Definition of Done por slice (+13 more)

### Community 1 - "PuntoIOS.ArchitectureTests.csproj"
Cohesion: 0.07
Nodes (31): net10.0, net10.0, Microsoft.NET.Sdk, net10.0, Microsoft.NET.Sdk, net10.0, Microsoft.NET.Sdk, net10.0 (+23 more)

### Community 2 - "3. Fases"
Cohesion: 0.10
Nodes (20): 1. Ciclo de cada slice, 2. Hitos de uso real, 3. Fases, 4. Flujos E2E mínimos (Playwright), 5. Definition of Done, 6. Estrategia Git, Fase 10 — Accesorios, repuestos e inventario, Fase 11 — Finanzas → Hito C (+12 more)

### Community 4 - "devDependencies"
Cohesion: 0.18
Nodes (11): devDependencies, jsdom@^30.1.3, @tailwindcss/turbopack@^4, @testing-library/react@^16.3.3, @types/react@^19, typescript@^5, vitest@^5.1.0, jsdom (+3 more)

### Community 5 - "Fact"
Cohesion: 0.15
Nodes (4): HealthEndpointTests, MeEndpointTests, PostgresFixture, ConnectionString

### Community 6 - "use-remote-checks.ts"
Cohesion: 0.16
Nodes (15): Home(), REMOTE_CHECKS, RemoteCheckId, Row, RowState, StatusBoard(), subscribeToConnection(), useOnline() (+7 more)

### Community 7 - "PageRequest"
Cohesion: 0.13
Nodes (9): PagedResult, TotalPages, PageRequest, Page, PageSize, Skip, PagedResultTests, PageRequestTests (+1 more)

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

### Community 15 - "scripts"
Cohesion: 0.25
Nodes (8): scripts, build, dev, lint, start, test, test:e2e, typecheck

### Community 16 - "auth-provider.tsx"
Cohesion: 0.17
Nodes (13): body, display, metadata, RootLayout(), viewport, AuthContext, AuthContextValue, AuthProvider() (+5 more)

### Community 20 - "login-form.tsx"
Cohesion: 0.09
Nodes (24): nextConfig, IngresarPage(), PanelPage(), describeAuthError(), useAuth(), LoginForm(), onGoogle(), onSubmit() (+16 more)

### Community 22 - "ObtenerUsuarioActualHandlerTests.Devuelve_la_identidad_del_usuario_autenticado"
Cohesion: 0.10
Nodes (13): HttpContextCurrentUserAccessor, MeController, CurrentUser, ICurrentUserAccessor, ObtenerUsuarioActualHandler, UsuarioActualDto, FakeAccessor, ObtenerUsuarioActualHandlerTests (+5 more)

### Community 25 - "package.json"
Cohesion: 0.20
Nodes (9): tailwindcss@^4, @types/node@^24.19.2, @vitejs/plugin-react@^6.2.1, name, private, version, tailwindcss, @types/node (+1 more)

### Community 26 - "@playwright/test"
Cohesion: 0.29
Nodes (3): CORS, @playwright/test@^1.64.1, @playwright/test

### Community 27 - "eslint.config.mjs"
Cohesion: 0.33
Nodes (5): eslintConfig, eslint@^9, eslint-config-next@16.4.5, eslint, eslint-config-next

### Community 28 - "dependencies"
Cohesion: 0.33
Nodes (6): dependencies, firebase@^13.2.0, next@16.4.5, react@19.3.0, react-dom@19.3.0, react-dom

## Knowledge Gaps
- **36 isolated node(s):** `coverlet.collector (6.0.4)`, `Microsoft.NET.Test.Sdk (17.14.1)`, `xunit (2.9.3)`, `xunit.runner.visualstudio (3.1.4)`, `coverlet.collector (6.0.4)` (+31 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 212 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **12 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `react` connect `use-remote-checks.ts` to `auth-provider.tsx`, `package.json`, `dependencies`, `login-form.tsx`?**
  _High betweenness centrality (0.028) - this node is a cross-community bridge._
- **What connects `coverlet.collector (6.0.4)`, `Microsoft.NET.Test.Sdk (17.14.1)`, `xunit (2.9.3)` to the rest of the system?**
  _36 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Punto iOS — Software Design Document (SDD)` be split into smaller, more focused modules?**
  _Cohesion score 0.09090909090909091 - nodes in this community are weakly interconnected._
- **Why does `next` connect `login-form.tsx` to `auth-provider.tsx`, `package.json`, `dependencies`, `use-remote-checks.ts`?**
  _High betweenness centrality (0.018) - this node is a cross-community bridge._
- **Should `PuntoIOS.ArchitectureTests.csproj` be split into smaller, more focused modules?**
  _Cohesion score 0.06747638326585695 - nodes in this community are weakly interconnected._
- **Why does `MeEndpointTests` connect `Fact` to `MeEndpointTests.cs`?**
  _High betweenness centrality (0.016) - this node is a cross-community bridge._
- **Should `3. Fases` be split into smaller, more focused modules?**
  _Cohesion score 0.09523809523809523 - nodes in this community are weakly interconnected._