using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;
using Serilog.MultiTenant.Abstractions;
using Serilog.MultiTenant.Configuration;
using Serilog.MultiTenant.Context;
using Serilog.MultiTenant.Options;

namespace Serilog.MultiTenant.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSerilogMultiTenant(
        this IServiceCollection services,
        LogEventLevel baseLevel,
        Action<TenantLoggingOptions>? configureTenantLogging = null,
        Action<TenantLogContextMiddlewareOptions>? configureTenantLogContext = null,
        Action<TenantLogLevelStoreOptions>? configureTenantLogLevelStore = null)
    {
        services.AddOptions<TenantLoggingOptions>();
        services.AddOptions<TenantLogContextMiddlewareOptions>();
        services.AddOptions<TenantLogLevelStoreOptions>();

        services.AddSingleton<IBaseLogLevelProvider>(_ => new FixedBaseLogLevelProvider(baseLevel));
        services.AddSingleton<ITenantLogLevelStore, InMemoryTenantLogLevelStore>();
        services.AddSingleton<ITenantLogLevelResolver, TenantLogLevelResolver>();
        services.AddSingleton<ITenantLogLevelConfigurator, TenantLogLevelConfigurator>();
        services.AddSingleton<AsyncLocalTenantContextAccessor>();
        services.AddSingleton<ITenantContextAccessor>(provider => provider.GetRequiredService<AsyncLocalTenantContextAccessor>());
        services.AddSingleton<ITenantContextSetter>(provider => provider.GetRequiredService<AsyncLocalTenantContextAccessor>());

        if (configureTenantLogging is not null)
        {
            services.Configure(configureTenantLogging);
        }

        if (configureTenantLogContext is not null)
        {
            services.Configure(configureTenantLogContext);
        }

        if (configureTenantLogLevelStore is not null)
        {
            services.Configure(configureTenantLogLevelStore);
        }

        return services;
    }
}
