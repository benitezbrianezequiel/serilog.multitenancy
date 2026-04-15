using System.Collections.Concurrent;
using Serilog.Events;
using Serilog.MultiTenant.Abstractions;

namespace Serilog.MultiTenant.Configuration;

public sealed class InMemoryTenantLogLevelStore : ITenantLogLevelStore
{
    private readonly ConcurrentDictionary<string, LogEventLevel> _tenantLevels = new(StringComparer.OrdinalIgnoreCase);

    public bool TryGet(string tenantId, out LogEventLevel level)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            level = default;
            return false;
        }

        return _tenantLevels.TryGetValue(tenantId, out level);
    }

    public void Set(string tenantId, LogEventLevel level)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            throw new ArgumentException("Tenant id cannot be null or whitespace.", nameof(tenantId));
        }

        _tenantLevels.AddOrUpdate(tenantId, level, (_, _) => level);
    }

    public bool Remove(string tenantId)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            return false;
        }

        return _tenantLevels.TryRemove(tenantId, out _);
    }

    public IReadOnlyDictionary<string, LogEventLevel> Snapshot()
    {
        return _tenantLevels.ToDictionary(static pair => pair.Key, static pair => pair.Value, StringComparer.OrdinalIgnoreCase);
    }
}
