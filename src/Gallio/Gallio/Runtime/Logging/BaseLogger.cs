using System;

namespace Gallio.Runtime.Logging
{
    /// <summary>
    /// Base implementation of <see cref="ILogger" /> that performs argument validation
    /// and supports convenience methods.
    /// </summary>
    [Serializable]
    public abstract class BaseLogger : ILogger
    {
        /// <inheritdoc />
        public void Log(LogSeverity severity, string message)
        {
            Log(severity, message, null);
        }

        /// <inheritdoc />
        public void Log(LogSeverity severity, string message, Exception exception)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            LogInternal(severity, message, exception);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="severity">The log message severity</param>
        /// <param name="message">The log message, not null</param>
        /// <param name="exception">The associated exception, or null if none</param>
        protected abstract void LogInternal(LogSeverity severity, string message, Exception exception);
    }
}
