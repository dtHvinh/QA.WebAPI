using Serilog.Core;
using Serilog.Events;

namespace WebAPI.Utilities.Event;

public class LogSinkEvent : ILogEventSink
{
    public void Emit(LogEvent logEvent)
    {
        if (logEvent.Properties.TryGetValue("Application", out var application))
        {
            if (application.ToString() == "WebAPI")
            {
                Console.WriteLine(logEvent.RenderMessage());
            }
        }
    }
}
