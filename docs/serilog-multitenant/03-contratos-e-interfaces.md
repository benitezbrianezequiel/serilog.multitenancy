# Contratos e interfaces

## ITenantContextAccessor

Responsabilidad: exponer el tenant actual del contexto de ejecución.

```csharp
public interface ITenantContextAccessor
{
    string? TenantId { get; }
}
```

## ITenantLogLevelStore

Responsabilidad: guardar y consultar overrides dinámicos por tenant.

```csharp
public interface ITenantLogLevelStore
{
    bool TryGet(string tenantId, out LogEventLevel level);
    void Set(string tenantId, LogEventLevel level);
    bool Remove(string tenantId);
    IReadOnlyDictionary<string, LogEventLevel> Snapshot();
}
```

## IBaseLogLevelProvider

Responsabilidad: exponer el nivel base funcional definido en `Main`.

```csharp
public interface IBaseLogLevelProvider
{
    LogEventLevel BaseLevel { get; }
}
```

## ITenantLogLevelResolver

Responsabilidad: resolver el nivel efectivo con fallback seguro.

```csharp
public interface ITenantLogLevelResolver
{
    LogEventLevel ResolveEffectiveLevel(string? tenantId);
}
```

## TenantAwareLogEventFilter

Responsabilidad: permitir o descartar eventos por tenant usando nivel efectivo.

```csharp
// Regla conceptual
allow = logEvent.Level >= resolver.ResolveEffectiveLevel(tenantIdFromContext);
```

## Resolución de tenant para el filtro

- Primero intentar leer `TenantId` de propiedades del `LogEvent` (vía `LogContext`).
- Si no existe, usar `ITenantContextAccessor`.
- Si sigue sin existir, resolver nivel base.
