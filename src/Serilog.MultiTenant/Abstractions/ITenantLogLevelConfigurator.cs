using Serilog.Events;

namespace Serilog.MultiTenant.Abstractions;

public interface ITenantLogLevelConfigurator
{
    void SetLevel(string tenantId, LogEventLevel level);

    bool RemoveLevel(string tenantId);

    IReadOnlyDictionary<string, LogEventLevel> GetLevels();
}
