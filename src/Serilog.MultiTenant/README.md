# Serilog.MultiTenant

Proyecto para encapsular la extensión multi-tenant de Serilog.

## Objetivo

- Resolver nivel de logging por tenant usando `LogContext`.
- Soportar overrides dinámicos por tenant en runtime.
- Mantener fallback al nivel base definido por la aplicación host.
- No romper el pipeline de logging ante ausencia de `TenantId`.

## Componentes incluidos

- `ITenantLogLevelStore` + `InMemoryTenantLogLevelStore`.
- `ITenantLogLevelResolver` + `TenantLogLevelResolver`.
- `ITenantContextAccessor`/`ITenantContextSetter` + `AsyncLocalTenantContextAccessor`.
- `TenantAwareLogEventFilter` para filtrar por nivel efectivo por tenant.
- `TenantLogContextMiddleware` para publicar `TenantId` en `LogContext`.
- Extensiones para DI y configuración de `LoggerConfiguration`.

## Uso rápido

```csharp
using Serilog;
using Serilog.Events;
using Serilog.MultiTenant.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilogMultiTenant(
    baseLevel: LogEventLevel.Information,
    configureTenantLogContext: options =>
    {
        options.TenantIdResolver = context => context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
    });

builder.Host.UseSerilog((context, services, loggerConfig) =>
{
    loggerConfig
        .ReadFrom.Configuration(context.Configuration)
        .UseTenantAwareLevelFiltering(services);
});

WebApplication app = builder.Build();
app.UseTenantLogContext();
```

## Overrides dinámicos

```csharp
using Serilog.Events;
using Serilog.MultiTenant.Abstractions;

ITenantLogLevelConfigurator configurator = app.Services.GetRequiredService<ITenantLogLevelConfigurator>();

configurator.SetLevel("tenant-a", LogEventLevel.Verbose);
configurator.SetLevel("tenant-b", LogEventLevel.Warning);
configurator.RemoveLevel("tenant-b");
```
