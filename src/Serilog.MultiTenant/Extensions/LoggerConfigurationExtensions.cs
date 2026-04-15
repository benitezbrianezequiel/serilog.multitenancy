using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog.MultiTenant.Abstractions;
using Serilog.MultiTenant.Options;

namespace Serilog.MultiTenant.Extensions;

public static class LoggerConfigurationExtensions
{
    public static LoggerConfiguration UseTenantAwareLevelFiltering(
        this LoggerConfiguration loggerConfiguration,
        IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(loggerConfiguration);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        ITenantLogLevelResolver tenantLogLevelResolver = serviceProvider.GetRequiredService<ITenantLogLevelResolver>();
        ITenantContextAccessor tenantContextAccessor = serviceProvider.GetRequiredService<ITenantContextAccessor>();
        IOptions<TenantLoggingOptions> optionsAccessor = serviceProvider.GetRequiredService<IOptions<TenantLoggingOptions>>();

        loggerConfiguration.MinimumLevel.Verbose();
        loggerConfiguration.Filter.ByTenantLevel(
            tenantLogLevelResolver,
            optionsAccessor.Value,
            tenantContextAccessor);

        return loggerConfiguration;
    }
}
