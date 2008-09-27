using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runtime.Logging
{
    /// <summary>
    /// Event arguments for the <see cref="EventLogger"/>
    /// </summary>
    public class LogMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a log event.
        /// </summary>
        /// <param name="severity">The log severity</param>
        /// <param name="message">The log message</param>
        /// <param name="exception">The exception, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is null</exception>
        public LogMessageEventArgs(LogSeverity severity, string message, Exception exception)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            Severity = severity;
            Message = message;
            Exception = exception;
        }

        /// <summary>
        /// Gets the log severity.
        /// </summary>
        public LogSeverity Severity { get; private set; }

        /// <summary>
        /// Gets the log message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the exception, or null if none.
        /// </summary>
        public Exception Exception { get; private set; }
    }
}
