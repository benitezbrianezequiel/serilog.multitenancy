using Serilog.Events;

namespace Serilog.MultiTenant.Configuration;

internal sealed record TenantLogLevelEntry(
    LogEventLevel Level,
    DateTimeOffset LastAccessedUtc,
    DateTimeOffset ExpiresAtUtc);
