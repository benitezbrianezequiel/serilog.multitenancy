# Serilog Multi-tenant

Workspace dedicado para diseñar e implementar la extensión multi-tenant de Serilog.

## Contenido

- `docs/serilog-multitenant`: decisiones, arquitectura, riesgos, plan y pruebas.
- `src/Serilog.MultiTenant`: librería con store, resolver, filtro y middleware multi-tenant.

## Build

```powershell
dotnet build src/Serilog.MultiTenant/Serilog.MultiTenant.csproj
```
