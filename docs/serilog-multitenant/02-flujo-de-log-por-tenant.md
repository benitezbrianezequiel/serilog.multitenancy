# Flujo de log por tenant

## Flujo HTTP

1. Middleware identifica el tenant de la request.
2. Middleware agrega `TenantId` al `LogContext`.
3. El evento se crea en Serilog.
4. El filtro tenant-aware resuelve nivel efectivo:
   - con override dinámico si existe;
   - si no, nivel base del host.
5. Si `logEvent.Level` cumple el umbral efectivo, se envía a sinks.

## Flujo no HTTP (jobs/background)

1. El proceso establece `TenantId` en scope de `LogContext` cuando corresponda.
2. Si no hay `TenantId`, se aplica fallback automático al nivel base.
3. El pipeline no se interrumpe.

## Orden de resolución de TenantId

1. `LogContext` (primario).
2. `ITenantContextAccessor` (respaldo).
3. Sin tenant: fallback a nivel base.
