using Serilog;
using Serilog.Configuration;
using Serilog.MultiTenant.Abstractions;
using Serilog.MultiTenant.Filtering;
using Serilog.MultiTenant.Options;

namespace Serilog.MultiTenant.Extensions;

public static class LoggerFilterConfigurationExtensions
{
    public static LoggerConfiguration ByTenantLevel(
        this LoggerFilterConfiguration loggerFilterConfiguration,
        ITenantLogLevelResolver tenantLogLevelResolver,
        ITenantContextAccessor? tenantContextAccessor = null,
        string tenantPropertyName = "TenantId")
    {
        ArgumentNullException.ThrowIfNull(loggerFilterConfiguration);
        ArgumentNullException.ThrowIfNull(tenantLogLevelResolver);

        return loggerFilterConfiguration.With(
            new TenantAwareLogEventFilter(
                tenantLogLevelResolver,
                tenantPropertyName,
                tenantContextAccessor));
    }

    public static LoggerConfiguration ByTenantLevel(
        this LoggerFilterConfiguration loggerFilterConfiguration,
        ITenantLogLevelResolver tenantLogLevelResolver,
        TenantLoggingOptions options,
        ITenantContextAccessor? tenantContextAccessor = null)
    {
        ArgumentNullException.ThrowIfNull(options);

        return loggerFilterConfiguration.ByTenantLevel(
            tenantLogLevelResolver,
            tenantContextAccessor,
            options.TenantPropertyName);
    }
}
