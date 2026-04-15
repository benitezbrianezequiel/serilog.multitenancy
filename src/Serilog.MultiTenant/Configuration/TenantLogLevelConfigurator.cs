using Serilog.Events;
using Serilog.MultiTenant.Abstractions;

namespace Serilog.MultiTenant.Configuration;

public sealed class TenantLogLevelConfigurator(ITenantLogLevelStore tenantLogLevelStore) : ITenantLogLevelConfigurator
{
    public void SetLevel(string tenantId, LogEventLevel level)
    {
        tenantLogLevelStore.Set(tenantId, level);
    }

    public bool RemoveLevel(string tenantId)
    {
        return tenantLogLevelStore.Remove(tenantId);
    }

    public IReadOnlyDictionary<string, LogEventLevel> GetLevels()
    {
        return tenantLogLevelStore.Snapshot();
    }
}
