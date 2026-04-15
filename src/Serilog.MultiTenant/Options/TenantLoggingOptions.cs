namespace Serilog.MultiTenant.Options;

public sealed class TenantLoggingOptions
{
    public string TenantPropertyName { get; set; } = "TenantId";
}
