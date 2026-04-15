using System.Threading;
using Serilog.MultiTenant.Abstractions;

namespace Serilog.MultiTenant.Context;

public sealed class AsyncLocalTenantContextAccessor : ITenantContextAccessor, ITenantContextSetter
{
    private static readonly AsyncLocal<string?> CurrentTenant = new();

    public string? TenantId => CurrentTenant.Value;

    public IDisposable BeginScope(string? tenantId)
    {
        string? previous = CurrentTenant.Value;
        CurrentTenant.Value = tenantId;

        return new RestoreScope(previous);
    }

    private sealed class RestoreScope(string? previous) : IDisposable
    {
        public void Dispose()
        {
            CurrentTenant.Value = previous;
        }
    }
}
