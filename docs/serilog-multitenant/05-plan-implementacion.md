# Plan de implementación

## Fase 1: núcleo de la extensión

- Implementar interfaces y servicios base (`Accessor`, `Store`, `Resolver`, `BaseProvider`).
- Implementar `TenantAwareLogEventFilter` con fallback al nivel base.

## Fase 2: integración en aplicación

- Agregar middleware para publicar `TenantId` en `LogContext`.
- Registrar componentes en DI root.
- Configurar Serilog con captura interna amplia y filtro tenant-aware.

## Fase 3: administración runtime

- Exponer API/handler para:
  - `SetTenantLevel(tenantId, level)`
  - `RemoveTenantLevel(tenantId)`
  - `GetTenantLevels()`

## Fase 4: observabilidad y resiliencia

- Incluir métricas y contadores operativos.
- Asegurar fail-safe en todos los puntos de resolución.

## Fase 5: validación

- Pruebas funcionales por escenarios clave.
- Pruebas concurrentes de actualización/lectura de niveles.
- Pruebas de carga con mezcla de tenants y niveles.
