using Microsoft.AspNetCore.Builder;
using Serilog.MultiTenant.Middleware;

namespace Serilog.MultiTenant.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseTenantLogContext(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        return app.UseMiddleware<TenantLogContextMiddleware>();
    }
}
