using Serilog.Events;

namespace Serilog.MultiTenant.Abstractions;

public interface ITenantLogLevelResolver
{
    LogEventLevel ResolveEffectiveLevel(string? tenantId);
}
