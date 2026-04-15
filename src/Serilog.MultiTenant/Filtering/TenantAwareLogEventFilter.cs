using Serilog.Core;
using Serilog.Events;
using Serilog.MultiTenant.Abstractions;

namespace Serilog.MultiTenant.Filtering;

public sealed class TenantAwareLogEventFilter(
    ITenantLogLevelResolver tenantLogLevelResolver,
    string tenantPropertyName,
    ITenantContextAccessor? tenantContextAccessor = null) : ILogEventFilter
{
    private readonly string _tenantPropertyName = string.IsNullOrWhiteSpace(tenantPropertyName)
        ? throw new ArgumentException("Tenant property name cannot be null or whitespace.", nameof(tenantPropertyName))
        : tenantPropertyName;

    public bool IsEnabled(LogEvent logEvent)
    {
        try
        {
            string? tenantId = ResolveTenantId(logEvent);
            LogEventLevel effectiveLevel = tenantLogLevelResolver.ResolveEffectiveLevel(tenantId);

            return logEvent.Level >= effectiveLevel;
        }
        catch
        {
            LogEventLevel fallbackLevel = tenantLogLevelResolver.ResolveEffectiveLevel(null);
            return logEvent.Level >= fallbackLevel;
        }
    }

    private string? ResolveTenantId(LogEvent logEvent)
    {
        string? tenantIdFromEvent = TryGetTenantIdFromEvent(logEvent);
        if (!string.IsNullOrWhiteSpace(tenantIdFromEvent))
        {
            return tenantIdFromEvent;
        }

        return tenantContextAccessor?.TenantId;
    }

    private string? TryGetTenantIdFromEvent(LogEvent logEvent)
    {
        if (!logEvent.Properties.TryGetValue(_tenantPropertyName, out LogEventPropertyValue? tenantValue))
        {
            return null;
        }

        if (tenantValue is ScalarValue scalarValue)
        {
            return scalarValue.Value?.ToString();
        }

        return tenantValue.ToString();
    }
}
