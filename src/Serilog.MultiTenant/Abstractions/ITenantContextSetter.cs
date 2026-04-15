namespace Serilog.MultiTenant.Abstractions;

public interface ITenantContextSetter
{
    IDisposable BeginScope(string? tenantId);
}
