# Graph Report - Punto iOS  (2026-11-05)

## Corpus Check
- 50 files · ~7,374 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 8 file(s) not represented in the graph (top: (none) 5, .example 1, .ico 1)

## Summary
- 324 nodes · 391 edges · 21 communities (15 shown, 6 thin omitted)
- Extraction: 98% EXTRACTED · 2% INFERRED · 0% AMBIGUOUS · INFERRED: 7 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- Punto iOS — Software Design Document (SDD)
- PuntoIOS.ArchitectureTests.csproj
- 3. Fases
- 0001-decisiones-iniciales.md
- package.json
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
- layout.tsx
- PuntoIOS.Application/AssemblyMarker.cs
- PuntoIOS.Domain/AssemblyMarker.cs
- PuntoIOS.Infrastructure/AssemblyMarker.cs

## God Nodes (most connected - your core abstractions)
1. `Punto iOS — Software Design Document (SDD)` - 19 edges
2. `compilerOptions` - 16 edges
3. `3. Fases` - 14 edges
4. `PageRequest` - 9 edges
5. `HealthEndpointTests` - 8 edges
6. `scripts` - 8 edges
7. `postgres` - 8 edges
8. `Punto iOS — Roadmap de desarrollo` - 7 edges
9. `Punto iOS` - 7 edges
10. `LayerDependencyTests` - 6 edges

## Surprising Connections (you probably didn't know these)
- `Home()` --calls--> `StatusBoard()`  [EXTRACTED]
  frontend/src/app/page.tsx → frontend/src/features/sistema/status-board.tsx
- `StatusBoard()` --calls--> `useRemoteChecks()`  [EXTRACTED]
  frontend/src/features/sistema/status-board.tsx → frontend/src/features/sistema/use-remote-checks.ts
- `useRemoteChecks()` --calls--> `probeEndpoint()`  [EXTRACTED]
  frontend/src/features/sistema/use-remote-checks.ts → frontend/src/lib/api/probe.ts

## Import Cycles
- None detected.

## Communities (21 total, 6 thin omitted)

### Community 0 - "Punto iOS — Software Design Document (SDD)"
Cohesion: 0.09
Nodes (21): 10. Notificaciones internas, 11. PWA y offline, 12. API, 13. Seguridad y auditoría, 14. Pruebas, 15. Diseño visual, 16. Observabilidad, 17. Definition of Done por slice (+13 more)

### Community 1 - "PuntoIOS.ArchitectureTests.csproj"
Cohesion: 0.07
Nodes (31): net10.0, net10.0, Microsoft.NET.Sdk, net10.0, Microsoft.NET.Sdk, net10.0, Microsoft.NET.Sdk, net10.0 (+23 more)

### Community 2 - "3. Fases"
Cohesion: 0.10
Nodes (20): 1. Ciclo de cada slice, 2. Hitos de uso real, 3. Fases, 4. Flujos E2E mínimos (Playwright), 5. Definition of Done, 6. Estrategia Git, Fase 10 — Accesorios, repuestos e inventario, Fase 11 — Finanzas → Hito C (+12 more)

### Community 4 - "package.json"
Cohesion: 0.05
Nodes (42): CORS, eslintConfig, nextConfig, dependencies, next@16.4.5, react@19.3.0, react-dom@19.3.0, devDependencies (+34 more)

### Community 5 - "Fact"
Cohesion: 0.09
Nodes (6): LayerDependencyTests, HealthEndpointTests, PostgresFixture, ConnectionString, PuntoIOS.ArchitectureTests, PuntoIOS.IntegrationTests

### Community 6 - "use-remote-checks.ts"
Cohesion: 0.15
Nodes (14): Home(), REMOTE_CHECKS, RemoteCheckId, Row, RowState, StatusBoard(), subscribeToConnection(), useOnline() (+6 more)

### Community 7 - "PageRequest"
Cohesion: 0.13
Nodes (10): PagedResult, TotalPages, PageRequest, Page, PageSize, Skip, PagedResultTests, PageRequestTests (+2 more)

### Community 8 - "Instrucciones para Codex / Claude Code — Punto iOS"
Cohesion: 0.50
Nodes (3): Instrucciones para Codex / Claude Code — Punto iOS, Primer encargo, Reglas de trabajo

### Community 9 - "Punto iOS"
Cohesion: 0.25
Nodes (7): Cómo ejecutar, Documentación, Estado, Estructura, Pruebas, Punto iOS, Requisitos

### Community 10 - "PostgresReadyCheck"
Cohesion: 0.11
Nodes (5): DotEnv, PostgresReadyCheck, Program, PuntoIOS.Api.Configuration, PuntoIOS.Api.Health

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

### Community 16 - "layout.tsx"
Cohesion: 0.29
Nodes (4): body, display, metadata, viewport

## Knowledge Gaps
- **36 isolated node(s):** `coverlet.collector (6.0.4)`, `Microsoft.NET.Test.Sdk (17.14.1)`, `xunit (2.9.3)`, `xunit.runner.visualstudio (3.1.4)`, `coverlet.collector (6.0.4)` (+31 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 185 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **6 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `react` connect `package.json` to `use-remote-checks.ts`?**
  _High betweenness centrality (0.021) - this node is a cross-community bridge._
- **What connects `coverlet.collector (6.0.4)`, `Microsoft.NET.Test.Sdk (17.14.1)`, `xunit (2.9.3)` to the rest of the system?**
  _36 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Punto iOS — Software Design Document (SDD)` be split into smaller, more focused modules?**
  _Cohesion score 0.09090909090909091 - nodes in this community are weakly interconnected._
- **Why does `next` connect `package.json` to `layout.tsx`?**
  _High betweenness centrality (0.015) - this node is a cross-community bridge._
- **Should `PuntoIOS.ArchitectureTests.csproj` be split into smaller, more focused modules?**
  _Cohesion score 0.06747638326585695 - nodes in this community are weakly interconnected._
- **Should `3. Fases` be split into smaller, more focused modules?**
  _Cohesion score 0.09523809523809523 - nodes in this community are weakly interconnected._
- **Should `package.json` be split into smaller, more focused modules?**
  _Cohesion score 0.05411764705882353 - nodes in this community are weakly interconnected._