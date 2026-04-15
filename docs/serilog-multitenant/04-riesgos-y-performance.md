# Riesgos y performance

## Perfil de performance esperado

- Costo por evento: lectura de tenant + lookup O(1) + comparación de enum.
- Estructura recomendada: `ConcurrentDictionary<string, LogEventLevel>`.
- Actualizaciones de nivel: `AddOrUpdate` sin frenar lecturas concurrentes.

## Riesgos principales

### 1) Falta de TenantId

- Riesgo: decisiones inconsistentes si algunos flujos no setean contexto.
- Mitigación: fallback al nivel base y continuidad total del pipeline.

### 2) Cardinalidad de tenants en crecimiento

- Riesgo: aumento de memoria por overrides que no se limpian.
- Mitigación: opcional TTL/LRU y tareas de limpieza periódica.

### 3) Volumen alto en modo verboso

- Riesgo: más eventos procesados para tenants en `Verbose`/`Debug`.
- Mitigación: ventanas temporales de diagnóstico, límites operativos, monitoreo.

### 4) Errores internos del resolver

- Riesgo: cortar el logging por excepción.
- Mitigación: fail-safe obligatorio, fallback al nivel base.

## Métricas recomendadas

- `tenant_level_override_hits`
- `tenant_level_fallback_hits`
- `tenant_context_missing_hits`
- `tenant_level_store_size`
- `events_dropped_by_tenant_filter`
