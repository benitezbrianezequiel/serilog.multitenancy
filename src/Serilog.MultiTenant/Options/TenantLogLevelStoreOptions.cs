namespace Serilog.MultiTenant.Options;

public sealed class TenantLogLevelStoreOptions
{
    public int MaxEntries { get; set; } = 10000;

    public TimeSpan EntryTtl { get; set; } = TimeSpan.FromHours(6);

    public TimeSpan CleanupInterval { get; set; } = TimeSpan.FromMinutes(5);
}
