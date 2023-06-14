using Serilog;
using Serilog.Events;

namespace ITMO.Dev.ASAP.Tests.Tools;

public class StaticLogger : ILogger
{
    public void Write(LogEvent logEvent)
    {
        Log.Logger.Write(logEvent);
    }
}