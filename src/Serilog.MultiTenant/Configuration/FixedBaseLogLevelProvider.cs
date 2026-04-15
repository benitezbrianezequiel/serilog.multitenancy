using Serilog.Events;
using Serilog.MultiTenant.Abstractions;

namespace Serilog.MultiTenant.Configuration;

public sealed class FixedBaseLogLevelProvider(LogEventLevel baseLevel) : IBaseLogLevelProvider
{
    public LogEventLevel BaseLevel { get; } = baseLevel;
}
