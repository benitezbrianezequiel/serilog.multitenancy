using Serilog.Events;

namespace Serilog.MultiTenant.Abstractions;

public interface ITenantLogLevelStore
{
    bool TryGet(string tenantId, out LogEventLevel level);

    void Set(string tenantId, LogEventLevel level);

    bool Remove(string tenantId);

    IReadOnlyDictionary<string, LogEventLevel> Snapshot();
}
