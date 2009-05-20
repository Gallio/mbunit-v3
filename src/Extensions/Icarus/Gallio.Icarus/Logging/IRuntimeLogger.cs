using System;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Runtime.Logging;

namespace Gallio.Icarus.Logging
{
    public interface IRuntimeLogger
    {
        event EventHandler<RuntimeLogEventArgs> LogMessage;
        LogSeverity MinLogSeverity { get; set; }
    }
}
