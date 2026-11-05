# ADR-0001 — Decisiones iniciales

**Fecha:** 2026-11-04 · **Estado:** Aceptada

1. **La especificación del dueño prevalece** sobre los documentos genéricos iniciales (roles, 15 estados de reparación, medios de pago, offline, portal público).
2. **Roles:** Administrador, Recepción/Ventas, Técnico; configurables en PostgreSQL.
3. **Firebase** se integra en la Fase 2, no en los cimientos; en la Fase 1 solo hay variables vacías en `.env.example`.
4. **Multi-local:** toda entidad operativa lleva `LocalId`; un único local sembrado en la etapa 1.
5. **Idioma:** dominio y features en español; infraestructura técnica en inglés.
6. **Offline desde el diseño:** IDs generados en el cliente (UUID) y `Idempotency-Key` en mutaciones críticas desde las primeras fases, aunque la sincronización se implemente en la Fase 12.
7. **Pruebas:** cobertura significativa de reglas de negocio y flujos críticos, no 100 % mecánico de líneas; el dominio y la aplicación apuntan a cobertura alta, sin usarla como único criterio.
8. **Verificación:** los comandos de build y test se ejecutan en la máquina del dueño (Windows con .NET y Docker) o en un entorno que los tenga; los resultados reportados deben ser reales.
9. **Herramientas:** graphify (`graphifyy`) para el grafo de conocimiento del repositorio.
