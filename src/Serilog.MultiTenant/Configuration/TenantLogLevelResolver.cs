using Serilog.Events;
using Serilog.MultiTenant.Abstractions;

namespace Serilog.MultiTenant.Configuration;

public sealed class TenantLogLevelResolver(
    ITenantLogLevelStore tenantLogLevelStore,
    IBaseLogLevelProvider baseLogLevelProvider) : ITenantLogLevelResolver
{
    public LogEventLevel ResolveEffectiveLevel(string? tenantId)
    {
        if (!string.IsNullOrWhiteSpace(tenantId) && tenantLogLevelStore.TryGet(tenantId, out LogEventLevel tenantLevel))
        {
            return tenantLevel;
        }

        return baseLogLevelProvider.BaseLevel;
    }
}
