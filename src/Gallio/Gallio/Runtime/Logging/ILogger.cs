using System;

namespace Gallio.Runtime.Logging
{
    /// <summary>
    /// Provides support for logging messages from system components.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="severity">The log message severity</param>
        /// <param name="message">The log message</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is null</exception>
        void Log(LogSeverity severity, string message);
        
        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="severity">The log message severity</param>
        /// <param name="message">The log message</param>
        /// <param name="exception">The associated exception, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is null</exception>
        void Log(LogSeverity severity, string message, Exception exception);
    }
}
