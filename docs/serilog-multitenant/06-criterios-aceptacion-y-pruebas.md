# Criterios de aceptación y pruebas

## Criterios de aceptación

1. Con `TenantId` configurado y override existente, se aplica nivel del tenant.
2. Con `TenantId` ausente o sin override, se aplica nivel base de `Main`.
3. El pipeline nunca falla por ausencia de tenant o error del resolver.
4. Los cambios de nivel por tenant se aplican en runtime sin reinicio.
5. Se pueden habilitar overrides más verbosos que el nivel base funcional.

## Escenarios de prueba mínimos

- `Base=Information`, `TenantA=Verbose`: `TenantA` emite verbose, otros no.
- `Base=Information`, `TenantB=Warning`: `TenantB` filtra info/debug.
- Evento sin `TenantId`: se evalúa con `Base` y no rompe.
- Tenant desconocido sin override: usa `Base`.
- Cambiar `TenantA` de `Verbose` a `Error` en caliente y verificar efecto inmediato.

## Pruebas de no regresión

- Verificar que sinks existentes mantienen formato y entrega.
- Verificar que logs sin contexto tenant siguen comportándose como antes (según base).
