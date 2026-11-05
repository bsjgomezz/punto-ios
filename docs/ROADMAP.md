# Punto iOS — Roadmap de desarrollo

**Versión:** 2.0 · **Metodología:** Vertical Slice sobre arquitectura Hexagonal (Modular Monolith)
**Referencia de reglas y dominio:** `docs/SDD.md` (v2 prevalece ante cualquier contradicción).

## 1. Ciclo de cada slice
1. Criterios de aceptación explícitos → 2. caso de uso y dominio → 3. contrato API y persistencia → 4. backend → 5. frontend → 6. pruebas → 7. ejecutar tests, build y lint → 8. verificación manual → 9. actualizar docs y roadmap → 10. commit pequeño.
No se avanza con errores críticos pendientes. Cada fase termina con: código compilable, pruebas ejecutadas (resultado real), migraciones revisadas, docs actualizadas, resumen, comandos de ejecución, problemas conocidos y próximo paso.

## 2. Hitos de uso real
- **Hito A (fin Fase 5):** ya se puede usar en el local para recibir, presupuestar y entregar reparaciones, con portal de seguimiento.
- **Hito B (fin Fase 9):** ventas de celulares con reservas y catálogo público.
- **Hito C (fin Fase 11):** inventario y finanzas completos.
- **Hito D (fin Fase 13):** offline, seguridad y servidor listos para producción.

## 3. Fases

### Fase 1 — Inspección y cimientos (Fase 0 del repositorio) ✅ COMPLETADA (2026-11-05)
Backend ASP.NET Core con solución por capas, frontend Next.js (TS estricto), PostgreSQL con Docker Compose, `.env.example`, health check (`/health`, `/health/ready`), página inicial, pruebas mínimas (xUnit, arquitectura, Playwright smoke), README, ADR-0001, CI inicial (GitHub Actions) si aplica.
**Aceptación:** backend y frontend corren en local, PostgreSQL disponible, build y pruebas base pasan. *No incluye Firebase ni lógica de negocio.*

**Resultado verificado:** `dotnet build` y `dotnet test` (24/24), `npm run lint`, `typecheck`, `npm test` (7/7), `npm run test:e2e` (4/4, escritorio y celular) y `npm run build` OK. Verificación manual: las 4 luces de estado en verde, y la base se detecta al apagarla/prenderla. Commit `c3e136d`.

**Pendiente de la fase (diferido):** CI con GitHub Actions (requiere repositorio remoto).

### Fase 2 — Autenticación y permisos
- S1 Login/logout Firebase en frontend, rutas protegidas.
- S2 Validación del Firebase ID Token en backend, mapeo `uid` → usuario local.
- S3 Roles y permisos en PostgreSQL (Administrador, Recepción/Ventas, Técnico), políticas de autorización, gestión básica de usuarios.
**Pruebas:** token inválido/expirado, acceso sin permiso por rol, matriz de autorización.
**Aceptación:** el usuario inicia sesión y accede solo a lo que su rol permite.

### Fase 3 — Clientes y dispositivos
- S4 Crear/buscar cliente (nombre, apellido, teléfono obligatorios; email, dirección, observaciones opcionales; duplicados por teléfono).
- S5 Listado paginado, filtros, detalle y edición con historial.
- S6 Dispositivos del cliente (marca, modelo, almacenamiento, color, IMEI/serie protegido).
- S7 Código de seguimiento seguro por cliente (generación, regeneración).
**Aceptación:** gestionar clientes y dispositivos sin perder historial.

### Fase 4 — Recepción de reparaciones
- S8 Crear orden (cliente, dispositivo, problema informado, técnico, tipo de reparación).
- S9 Checklist de ingreso opcional y fotos (validación de archivos).
- S10 Máquina de estados con transiciones controladas y auditoría.
- S11 Listados y bandeja por rol (técnico ve asignadas).

### Fase 5 — Diagnóstico y presupuestos → Hito A parcial
- S12 Pruebas, diagnóstico, mediciones, componentes afectados, notas técnicas internas.
- S13 Presupuesto versionado.
- S14 Aprobación/rechazo (primero interno; el portal en Fase 7) con timestamp de servidor.

### Fase 6 — Reparación, entrega y garantía
- S15 Trabajo realizado, repuestos usados (cuando exista inventario se descuenta con movimiento), fechas.
- S16 Reparación completada → listo para retirar → entregado.
- S17 Cobro y garantía (Sin garantía/15/30/45/60 días) con snapshot.
**Aceptación:** una reparación completa desde ingreso hasta entrega, con historial.

### Fase 7 — Portal de seguimiento y notificaciones → Hito A
- S18 Portal público: consulta por código, DTOs públicos, rate limiting y anti-enumeración.
- S19 Aprobar/rechazar presupuesto desde el portal (idempotente, timestamp de servidor).
- S20 Notificaciones internas (bandeja, leídas, enlaces) para eventos de reparación y presupuestos.

### Fase 8 — Celulares y catálogo
- S21 Ficha individual de celular (estados Borrador…Archivado, IMEI restringido, fotos).
- S22 Reacondicionamiento y costos; precio y rentabilidad estimada.
- S23 Catálogo público (sin IMEI completo, sin costos ni márgenes).

### Fase 9 — Reservas y ventas → Hito B
- S24 Reserva con seña (duración configurable, saldo, vencimiento) con control de concurrencia.
- S25 Vencimiento automático y notificaciones de reserva.
- S26 Venta de celular (la seña se aplica sin duplicar ingreso), snapshot de costo, baja del catálogo.
**Pruebas:** concurrencia (dos clientes, una unidad), no doble conteo.

### Fase 10 — Accesorios, repuestos e inventario
- S27 Productos y variantes (SKU, stock mínimo, costo, precio).
- S28 Movimientos de stock trazables (compra, venta, devolución, consumo, ajuste con motivo).
- S29 Proveedores y compras; alerta de stock bajo.
- S30 Venta de accesorios; consumo de repuestos en reparaciones.
**Pruebas:** concurrencia de stock, stock = suma de movimientos.

### Fase 11 — Finanzas → Hito C
- S31 Pagos y métodos (efectivo, transferencia, débito, crédito), señas, devoluciones.
- S32 Gastos y categoría exacta **Retiro por gasto personal** (separada de gastos operativos).
- S33 Ganancia bruta, ganancia neta y flujo de caja; reglas anti doble conteo documentadas en ADR.
- S34 Reportes diarios/semanales/mensuales y exportación CSV.
**Pruebas:** cálculos financieros con casos reales, snapshots históricos.

### Fase 12 — PWA y sincronización offline
- S35 Manifest, Service Worker, instalación, actualizaciones, indicador de conexión.
- S36 IndexedDB, borradores y cola de operaciones; estados Pendiente/Sincronizando/Sincronizado/Error.
- S37 Sincronización idempotente, reintentos y resolución explícita de conflictos (nunca "última escritura gana" en dinero, stock, reservas o estados).
- S38 Reservas/ventas offline como pendientes de confirmación.
**Pruebas:** corte de conexión durante un registro, reenvío duplicado, conflictos.

### Fase 13 — Seguridad, pruebas finales y servidor → Hito D
Revisión de permisos y secretos, rate limiting, validación de archivos, auditoría completa, pruebas E2E de flujos críticos, HTTPS, backups y recuperación, logs y monitoreo, documentación de despliegue.

## 4. Flujos E2E mínimos (Playwright)
1. Crear cliente + dispositivo + reparación. 2. Presupuesto aprobado desde el portal. 3. Reparar, cobrar y entregar con garantía. 4. Comprar, reservar con seña y vender un celular. 5. Vender accesorios con descuento de stock. 6. Registrar gastos/retiros y consultar ganancias. 7. Navegación en móvil. 8. Flujo offline → sincronización.

## 5. Definition of Done
Backend y frontend implementados · validaciones y errores · migraciones revisadas · pruebas unitarias/integración/E2E aplicables aprobadas · docs actualizadas · compila · sin errores críticos · verificación manual · commit.

## 6. Estrategia Git
Commits pequeños (`feat(clientes): ...`, `test(finanzas): ...`, `fix(ventas): ...`). Nunca un commit que rompa build o pruebas principales.
