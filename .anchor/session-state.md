# Session state (auto-saved — last known reality)

_Updated: 2026-11-07 21:25:32_  ·  branch: `main`

## Uncommitted changes
```
 M backend/src/PuntoIOS.Api/Controllers/ReparacionesController.cs
 M backend/src/PuntoIOS.Api/Extensions/ServiceCollectionExtensions.cs
 M backend/src/PuntoIOS.Api/Program.cs
 M backend/src/PuntoIOS.Application/Features/Reparaciones/OrdenDtos.cs
 M backend/src/PuntoIOS.Application/Features/Reparaciones/Puertos/IOrdenReparacionRepositorio.cs
 M backend/src/PuntoIOS.Application/Features/Reparaciones/RecibirEquipo/RecibirEquipoHandler.cs
 M backend/src/PuntoIOS.Domain/Auditoria/RegistroAuditoria.cs
 M backend/src/PuntoIOS.Domain/Reparaciones/OrdenReparacion.cs
 M backend/src/PuntoIOS.Infrastructure/DependencyInjection.cs
 M backend/src/PuntoIOS.Infrastructure/Persistence/Configurations/ReparacionesConfiguraciones.cs
 M backend/src/PuntoIOS.Infrastructure/Persistence/OrdenReparacionRepositorio.cs
 M backend/src/PuntoIOS.Infrastructure/Persistence/PuntoIosDbContext.cs
 M backend/tests/PuntoIOS.IntegrationTests/TestApi.cs
 M backend/tests/PuntoIOS.UnitTests/Clientes/FakesClientes.cs
 M docs/ROADMAP.md
 M graphify-out/.graphify_build_state.json
 M graphify-out/.graphify_labels.json.sig
 M graphify-out/.graphify_labels.pending.json
 M graphify-out/GRAPH_REPORT.md
 M graphify-out/graph.html
 M graphify-out/graph.json
 M graphify-out/manifest.json
?? .anchor/
?? backend/src/PuntoIOS.Application/Common/Archivos/
?? backend/src/PuntoIOS.Application/Features/Reparaciones/Fotos/
?? backend/src/PuntoIOS.Domain/Reparaciones/ChecklistIngreso.cs
?? backend/src/PuntoIOS.Domain/Reparaciones/FotoOrden.cs
?? backend/src/PuntoIOS.Infrastructure/Archivos/
?? backend/tests/PuntoIOS.IntegrationTests/ChecklistYFotosEndpointTests.cs
?? backend/tests/PuntoIOS.UnitTests/Reparaciones/ChecklistYFotosTests.cs
```

Diff stat:
```
 .../Features/Reparaciones/OrdenDtos.cs             |  13 +-
 .../Puertos/IOrdenReparacionRepositorio.cs         |   6 +
 .../RecibirEquipo/RecibirEquipoHandler.cs          |  31 ++-
 .../PuntoIOS.Domain/Auditoria/RegistroAuditoria.cs |   2 +
 .../Reparaciones/OrdenReparacion.cs                |  40 ++-
 .../PuntoIOS.Infrastructure/DependencyInjection.cs |   6 +-
 .../Configurations/ReparacionesConfiguraciones.cs  |  25 ++
 .../Persistence/OrdenReparacionRepositorio.cs      |   8 +
 .../Persistence/PuntoIosDbContext.cs               |   3 +
 backend/tests/PuntoIOS.IntegrationTests/TestApi.cs |   1 +
 .../PuntoIOS.UnitTests/Clientes/FakesClientes.cs   |  25 ++
 docs/ROADMAP.md                                    |   2 +-
 graphify-out/.graphify_build_state.json            |   2 +-
 graphify-out/.graphify_labels.json.sig             |   2 +-
 graphify-out/.graphify_labels.pending.json         |  45 ++--
 graphify-out/GRAPH_REPORT.md                       | 300 ++++++++++++---------
 graphify-out/graph.html                            |   8 +-
 graphify-out/graph.json                            |   2 +-
 graphify-out/manifest.json                         | 270 +++++++++++++++----
 22 files changed, 646 insertions(+), 212 deletions(-)
```

## Recent commits
```
a061931 feat(reparaciones): recepción de equipos y órdenes de reparación
6efb067 feat(clientes): código de seguimiento del cliente con envío por WhatsApp
aa415f3 feat(dispositivos): equipos del cliente con IMEI protegido y alta de cliente con equipo
f312fca feat(clientes): ficha del cliente con edición e historial de cambios
6cec5a6 feat(clientes): edición de clientes e historial de cambios (auditoría)
45c2178 feat(clientes): crear y buscar clientes con deteccion de telefonos duplicados
0416e1b feat(identidad): roles y permisos en PostgreSQL, usuarios locales y politicas de autorizacion
6ff9856 feat(auth): login con email y Google, sesión recordada y rutas protegidas
```

## Resume
Read `memory/HANDOFF.md` for intent/next-step, then reconcile against the diff above. If HANDOFF.md is older than this checkpoint, trust the diff.
