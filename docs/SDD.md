# Punto iOS — Software Design Document (SDD)

**Versión:** 2.0 · **Estado:** base de conocimiento para Codex / Claude Code
**Idioma de interfaz:** español · **Moneda:** ARS · **Zona horaria de negocio:** America/Argentina (UTC-3). En base de datos todo se guarda en UTC.
**Precedencia:** ante cualquier contradicción, este SDD (v2) prevalece sobre versiones anteriores. Las decisiones se registran en `docs/ADR/`.

## 1. Visión

Punto iOS es una PWA de gestión para un negocio de reparación avanzada de iPhone (incluida microsoldadura), venta de celulares usados/reacondicionados, accesorios y repuestos. Centraliza: reparaciones, clientes y dispositivos, presupuestos, ventas y reservas con seña, inventario, compras, proveedores, finanzas, catálogo público, portal de seguimiento para clientes, notificaciones internas y trabajo offline.

Primera etapa: un solo negocio y un solo local. La arquitectura queda preparada para varios locales y, a futuro, para comercializarla a otros técnicos.

Prioridades: integridad de datos, seguridad, trazabilidad, precisión financiera, usabilidad en celular, desarrollo incremental con pruebas.

## 2. Stack (obligatorio; no cambiar sin consultar)

| Capa | Tecnología |
|---|---|
| Frontend | Next.js (App Router), React, TypeScript `strict`, diseño responsive, PWA instalable, IndexedDB |
| Backend | C#, ASP.NET Core Web API, Hexagonal + Vertical Slice, Modular Monolith |
| Datos | PostgreSQL, Entity Framework Core, Npgsql. Roles y permisos en PostgreSQL |
| Auth | Firebase Authentication; el backend valida el Firebase ID Token |
| Pruebas | xUnit, integración con PostgreSQL real (Testcontainers), Playwright, pruebas de sync offline |

Complementos propuestos (justificar en ADR si se adoptan): FluentValidation, React Hook Form + Zod, TanStack Query, Recharts, una librería liviana para IndexedDB (p. ej. Dexie), un plugin/estrategia de Service Worker (p. ej. Serwist).

## 3. Roles y permisos

Roles iniciales (sembrados en PostgreSQL, **editables**): 

- **Administrador/Propietario:** todo, incluidos finanzas, costos, usuarios, configuración.
- **Recepción/Ventas:** clientes, recepción de equipos, estados, presupuestos y consultas, ventas, reservas, cobros autorizados. Sin acceso por defecto a costos internos, márgenes ni finanzas globales.
- **Técnico:** reparaciones asignadas, pruebas, diagnósticos, estados técnicos, trabajos, repuestos usados, fotos y notas técnicas.

Modelo: `Role` ↔ `Permission` (códigos como `repairs.diagnose`, `finance.view`, `devices.view_imei`) ↔ `UserRole`. Los permisos se verifican **siempre en backend** con políticas de autorización; el frontend solo oculta UI.

## 4. Arquitectura

**Hexagonal + Vertical Slice + Modular Monolith.** Sin microservicios. El dominio no depende de EF Core, PostgreSQL, Firebase ni ASP.NET.

Regla de dependencias: `Api → Application → Domain` e `Infrastructure → Application/Domain`. Application define puertos (interfaces); Infrastructure los implementa. Se verifica con **pruebas de arquitectura** (`PuntoIOS.ArchitectureTests`).

Módulos (límites claros; comunicación por contratos de aplicación o eventos de dominio, nunca tocando tablas ajenas): Identidad y Permisos · Clientes y Dispositivos · Reparaciones · Presupuestos · Portal Público · Catálogo de Celulares · Reservas y Ventas · Inventario y Compras · Finanzas · Notificaciones · Sincronización · Auditoría.

Cada caso de uso es un slice: `Features/<Modulo>/<CasoDeUso>/` con comando/consulta, validador, handler y DTOs. Sin lógica de negocio en controladores.

### Estructura del repositorio
```text
backend/
  src/
    PuntoIOS.Api/             (controllers/endpoints, middleware, auth, config)
    PuntoIOS.Application/     (Features/<Modulo>/<CasoDeUso>, puertos, Common)
    PuntoIOS.Domain/          (entidades, value objects, reglas, eventos)
    PuntoIOS.Infrastructure/  (EF Core, repos, Firebase, almacenamiento de archivos)
  tests/
    PuntoIOS.UnitTests/  PuntoIOS.IntegrationTests/  PuntoIOS.ArchitectureTests/
frontend/
  src/ app/(public)  app/(dashboard)  features/  components/  lib/  hooks/  types/
  e2e/  (Playwright)
docs/  (SDD.md, ROADMAP.md, ADR/)
docker-compose.yml  .env.example  README.md  AGENTS.md
```
Nombres de dominio y features en español; infraestructura técnica en inglés.

### Preparación multi-local
Toda entidad operativa lleva `LocalId` (en la etapa 1 hay un único local sembrado). Los filtros por local se aplican de forma centralizada (query filter de EF Core). No se implementa multi-tenant completo en la etapa 1.

## 5. Modelo de dominio (resumen)

**Cliente:** nombre*, apellido*, teléfono*, email, dirección, observaciones; `CodigoSeguimiento` (ver §7). Tiene N dispositivos.
**Dispositivo (del cliente):** marca, modelo, almacenamiento, color, IMEI/serie (texto, sensible), observaciones; historial de reparaciones. El IMEI pertenece al dispositivo, no al cliente.
**Orden de reparación:** cliente, dispositivo, problema informado, estado, técnico responsable, tipo(s) de reparación (diagnóstico, pantalla, batería, conector de carga, cámara, audio, software, microsoldadura, Face ID, cambio de IC, placa, recuperación de datos, otros con descripción), pruebas, diagnóstico, mediciones, componentes afectados, trabajo realizado, piezas usadas, fotos, notas internas (privadas), checklist de ingreso (todo opcional), garantía, fechas. Los campos técnicos son opcionales.
**Checklist de ingreso (opcional):** pantalla rota/rayada, tapa dañada, marco golpeado, cámaras, Face ID, batería, botones, conectores, humedad/líquido, no enciende, accesorios recibidos, otros, fotos.
**Presupuesto:** versionado. Cada cambio crea versión nueva y exige nueva aprobación. Registra respuesta (aprobado/rechazado) con **timestamp del servidor**; quién respondió no es obligatorio.
**Garantía:** opciones Sin garantía, 15, 30, 45, 60 días. Se copian (duración y fechas) a la reparación; cambios de configuración no alteran las históricas.
**Celular en venta (ficha individual):** marca, modelo, almacenamiento, color, estado físico, salud de batería, IMEI (acceso restringido), fotos, defectos, accesorios, costo de compra, gastos de reacondicionamiento, costo total, precio, estado: Borrador · Disponible · Reservado · Vendido · Oculto · Archivado. Al venderse sale del catálogo público y conserva su historial.
**Reserva:** cliente, celular, seña, método de pago, fecha/hora, vencimiento (duración configurable), saldo, estado. Sin venta en cuotas.
**Producto de inventario:** SKU, producto, variante (p. ej. modelo de iPhone + color), stock actual/mínimo, costo, precio. Categorías: fundas, protectores, cargadores, cables, repuestos, otros. Todo cambio de stock genera un `MovimientoStock` (compra, venta, devolución, consumo en reparación, ajuste con motivo).
**Compra/Proveedor, Pago, Gasto, Retiro personal, Notificación, Auditoría.**

## 6. Estados de reparación y transiciones

Estados: Recibido, En pruebas, En diagnóstico, Esperando presupuesto, Presupuesto enviado, Esperando aprobación, Presupuesto rechazado, Aprobado, En reparación, Esperando repuesto, Reparación completada, Listo para retirar, Entregado, Cancelado, Irreparable.

Transiciones permitidas (**propuesta a validar con el dueño**; todo lo demás se rechaza en el dominio):

| Desde | Hacia |
|---|---|
| Recibido | En pruebas, Cancelado |
| En pruebas | En diagnóstico, Cancelado |
| En diagnóstico | Esperando presupuesto, Irreparable, Cancelado |
| Esperando presupuesto | Presupuesto enviado, Cancelado |
| Presupuesto enviado | Esperando aprobación |
| Esperando aprobación | Aprobado, Presupuesto rechazado |
| Presupuesto rechazado | Esperando presupuesto (nueva versión), Entregado (devolución sin reparar), Cancelado |
| Aprobado | En reparación, Esperando aprobación (si cambia el presupuesto), Cancelado |
| En reparación | Esperando repuesto, Reparación completada, Irreparable, Esperando aprobación (si cambia el presupuesto) |
| Esperando repuesto | En reparación, Cancelado |
| Reparación completada | Listo para retirar, En reparación |
| Listo para retirar | Entregado |
| Irreparable | Entregado (devolución) |
| Entregado, Cancelado | (terminales) |

Cada transición exige permiso, se audita (quién, cuándo, desde/hacia) y puede disparar notificaciones. Las órdenes cerradas no se borran físicamente.

## 7. Portal público de seguimiento

- Cada cliente recibe un **código de seguimiento** aleatorio, no secuencial, de alta entropía (CSPRNG, mínimo 128 bits o equivalente legible con checksum), **no derivado** de teléfono, DNI ni ID interno. Se guarda hasheado o con índice único; es regenerable.
- Sin cuenta de cliente. Con el código se consulta: dispositivos, reparaciones anteriores, estado actual, fechas, presupuestos pendientes/aprobados/rechazados, "listo para retirar". El cliente puede aprobar o rechazar presupuestos vigentes.
- **Nunca** se expone: notas privadas, costos internos, márgenes, IMEI completo (solo últimos dígitos), datos de otros clientes, finanzas.
- DTOs públicos separados de los privados. Rate limiting estricto por IP y por código, respuestas uniformes ante código inválido, bloqueo progresivo, sin enumeración, registro de intentos sin guardar el código completo.
- Aprobar/rechazar es una mutación crítica: idempotente, con timestamp de servidor y protección contra repetición.

## 8. Reservas, ventas y concurrencia

- Reservar/vender una unidad es transaccional y bloquea contra concurrencia (`xmin` / token de concurrencia optimista de Npgsql, o `SELECT ... FOR UPDATE`; restricción única parcial: una unidad solo puede tener una reserva activa). Dos clientes no pueden reservar o comprar la misma unidad.
- Reserva vencida → la unidad vuelve a Disponible mediante un proceso programado (con notificación previa "próxima a vencer" y "vencida").
- Al convertir reserva en venta, la seña **se aplica como pago de esa venta**, no se registra otra vez como ingreso.
- En offline, reservar o vender queda **Pendiente de confirmación** hasta que el backend confirme disponibilidad.

## 9. Finanzas

- `decimal` en C#, `NUMERIC(18,2)` (o la escala definida por ADR) en PostgreSQL. Nunca `float`/`double`.
- Métodos de pago: Efectivo, Transferencia bancaria, Tarjeta de débito, Tarjeta de crédito.
- Movimientos: ingresos, egresos, pagos, compras, gastos, señas, devoluciones, retiros personales. Categoría de gasto exacta: **Retiro por gasto personal**, separada de gastos operativos.
- **Ganancia bruta:** ventas menos costos directos atribuibles. **Ganancia neta:** ganancia bruta menos gastos operativos (los retiros personales **no** son gasto operativo). **Flujo de caja:** entradas reales menos salidas reales, con retiros identificados.
- Una seña, pago o venta se cuenta **una sola vez**: reglas de reconocimiento documentadas por ADR (la seña es cobro de caja al recibirse; el ingreso/venta se reconoce al concretarse la venta).
- **Snapshots** de costo, precio y garantía en cada operación; cambios futuros no alteran reportes históricos.
- No se borran físicamente movimientos financieros; se anulan con contraasiento trazable.

## 10. Notificaciones internas

Presupuesto pendiente/aprobado/rechazado · reparación recibida/en diagnóstico/en reparación/esperando repuesto/lista/entregada · reserva por vencer/vencida · stock bajo · pago registrado · error o conflicto de sincronización. Se pueden marcar leídas y enlazan al registro. WhatsApp y email quedan fuera de la primera versión.

## 11. PWA y offline

- PWA instalable: manifest, iconos, Service Worker (cache del shell), actualizaciones controladas, indicador de conexión y de sincronización.
- IndexedDB: datos locales, borradores de formularios, **cola de operaciones pendientes**.
- Estados visuales por registro: Pendiente · Sincronizando · Sincronizado · Error.
- **Decisiones de diseño tomadas desde el inicio** (aunque la sincronización se implemente en la fase 12): IDs generados en el cliente (UUID, preferentemente v7), cada mutación crítica lleva `Idempotency-Key`, el backend responde de forma idempotente, el cliente nunca muestra dinero/stock/reserva como confirmado hasta recibir confirmación del backend.
- Conflictos de dinero, stock, reservas y estados **no** se resuelven con "última escritura gana": se rechazan y se presentan al usuario para resolución explícita; se genera notificación.
- Datos que se pueden crear offline sin riesgo: clientes, dispositivos, recepciones, notas, borradores. Lo que requiere confirmación global (reservas, ventas, pagos) queda pendiente.

## 12. API

- REST bajo `/api/v1`. DTOs específicos; nunca se devuelven entidades de EF.
- Toda colección: `page`, `pageSize` (máximo definido, p. ej. 100), `sortBy`, `sortDirection` (lista blanca) y filtros específicos. Respuesta: `{ items, page, pageSize, totalCount, totalPages }`.
- Validación en backend. Errores con **ProblemDetails** (RFC 9457) consistentes.
- Mutaciones críticas aceptan `Idempotency-Key`.
- OpenAPI/Swagger en desarrollo. Health check: `GET /health` (liveness) y `/health/ready` (incluye PostgreSQL).
- Configuración por variables de entorno para localhost y servidor futuro.

## 13. Seguridad y auditoría

- Firebase ID Token: el backend verifica firma, emisor, audiencia, expiración y claims; mapea `uid` de Firebase a un usuario local con roles en PostgreSQL. Nunca se confía en IDs de usuario del cliente.
- Sin contraseñas almacenadas; sin secretos en el código ni en el frontend; sin tokens ni datos sensibles en logs.
- IMEI y datos identificatorios protegidos (permiso específico; enmascarados por defecto en listados).
- Rate limiting en endpoints públicos; HTTPS en servidor; validación de archivos subidos (tipo, tamaño, contenido) y almacenamiento fuera del webroot.
- Auditoría de cambios críticos (estados, precios, pagos, stock, permisos, aprobaciones).
- Sin borrado físico de órdenes cerradas ni movimientos financieros.
- Pruebas de autorización por rol en cada endpoint.

## 14. Pruebas

Las pruebas acompañan cada slice. El objetivo es **cubrir con rigor las reglas de negocio y los flujos críticos**, no perseguir 100 % de líneas (ver ADR-0001).

- **Unitarias (xUnit):** reglas de dominio, transiciones, cálculos financieros, validaciones, casos límite.
- **Integración:** PostgreSQL real (Testcontainers), migraciones, transacciones, endpoints, autenticación y autorización, **concurrencia** de reservas y stock.
- **End-to-end (Playwright):** login, cliente + reparación, presupuesto aprobado desde el portal, entrega con garantía, venta de celular con reserva, accesorios, finanzas, navegación móvil.
- **Offline/sync:** cola, reintentos, idempotencia, conflictos, pérdida de conexión durante un registro.
- **Arquitectura:** el dominio no referencia EF/Firebase/ASP.NET.
- Al cerrar un slice se reportan comandos y resultados **reales**.

## 15. Diseño visual

Moderno, profesional, claro y responsive (PC, tablet, celular). Estética morado + negro/grafito con buen contraste y legibilidad. Estados de carga, vacío, error y éxito en toda pantalla. Indicadores visibles de conexión y sincronización. Panel privado orientado a eficiencia operativa; portal público simple.

## 16. Observabilidad

Logs estructurados, correlation ID, duración de solicitudes, eventos de auditoría. Sin tokens ni datos sensibles.

## 17. Definition of Done por slice

Requisitos y criterios de aceptación cumplidos · backend + frontend · validaciones y manejo de errores · migraciones revisadas · pruebas pertinentes ejecutadas y aprobadas · documentación y roadmap actualizados · compila · sin errores críticos conocidos · verificación manual · commit pequeño.

## 18. Decisiones pendientes

Transiciones de estado definitivas (§6) · permisos finos por rol · reglas de reconocimiento financiero y de anulaciones/devoluciones · duración por defecto de reservas · política de seña no reembolsable o reembolsable · backups y retención · hosting (se sugiere VPS + Docker Compose + HTTPS) · almacenamiento de fotos · proyecto de Firebase y método de login (email/contraseña, Google) · integración WhatsApp (posterior) · estrategia de comercialización multi-local.
