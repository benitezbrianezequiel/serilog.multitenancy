namespace Serilog.MultiTenant.Abstractions;

public interface ITenantContextAccessor
{
    string? TenantId { get; }
}
