using System;
using Gallio.Utilities;

namespace Gallio.Runtime.Logging
{
    /// <summary>
    /// A logger that dispatches log messages via events.
    /// </summary>
    public class EventLogger : BaseLogger
    {
        /// <summary>
        /// An event that is fired when a log message is received.
        /// </summary>
        public event EventHandler<LogMessageEventArgs> LogMessage;

        /// <inheritdoc />
        protected override void LogImpl(LogSeverity severity, string message, Exception exception)
        {
            EventHandlerUtils.SafeInvoke(LogMessage, this, new LogMessageEventArgs(severity, message, exception));
        }
    }
}
