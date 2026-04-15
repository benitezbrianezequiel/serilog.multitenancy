using Serilog.Events;

namespace Serilog.MultiTenant.Abstractions;

public interface IBaseLogLevelProvider
{
    LogEventLevel BaseLevel { get; }
}
