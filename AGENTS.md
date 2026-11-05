# Instrucciones para Codex / Claude Code — Punto iOS

Antes de escribir código lee completamente `docs/SDD.md` y `docs/ROADMAP.md`. Si existe `graphify-out/graph.json`, consulta primero `graphify query "<pregunta>"` antes de buscar en archivos.

## Reglas de trabajo
1. Analiza el repositorio y los documentos primero; no asumas que está vacío.
2. Resume arquitectura, fase actual, riesgos y decisiones pendientes.
3. Un Vertical Slice por vez, con criterios de aceptación explícitos. No implementes el roadmap completo de una vez.
4. Mantén Hexagonal + Vertical Slice + Modular Monolith. Registra decisiones relevantes en `docs/ADR/`.
5. Pruebas desde el primer cambio. Ejecuta tests, build y lint pertinentes antes de declarar terminado.
6. Nunca afirmes que una prueba pasó si no la ejecutaste; informa comandos y resultados reales.
7. Dinero: `decimal` en C# y `NUMERIC` en PostgreSQL; jamás `float`/`double`.
8. El backend es la autoridad de validación, seguridad y persistencia. Valida el Firebase ID Token y los permisos en backend; no confíes en IDs de usuario del cliente.
9. Toda colección con paginación (`page`, `pageSize`, `sortBy`, `sortDirection`), límites y filtros permitidos. Errores con ProblemDetails. Mutaciones críticas con `Idempotency-Key`.
10. No confirmes operaciones de dinero, stock o reservas hasta que el backend las persista. No borres movimientos financieros ni de stock; usa contraasientos.
11. Nunca expongas IMEI completo, costos, márgenes ni notas privadas en DTOs públicos.
12. Si falta una decisión de negocio, no inventes: presenta opciones, recomienda una y pide confirmación.
13. Cambios acotados; sin refactors no relacionados. Actualiza docs y roadmap al cerrar cada slice.
14. Commits pequeños y claros solo cuando el dueño lo pida o el flujo lo permita.
15. Tras modificar código, ejecuta `graphify update .` si graphify está instalado.

## Primer encargo
Realiza únicamente la **Fase 1 (cimientos)** del roadmap. No avances a autenticación ni a funcionalidades de negocio hasta que el dueño revise y apruebe la base.
