using System;

namespace Gallio.Runtime.Logging
{
    /// <summary>
    /// Filters another logger to exclude messages below a given level of severity.
    /// </summary>
    public class FilteredLogger : BaseLogger
    {
        private readonly ILogger logger;
        private readonly LogSeverity minSeverity;

        /// <summary>
        /// Creates a filtered logger.
        /// </summary>
        /// <param name="logger">The logger to which filtered log messages are sent</param>
        /// <param name="minSeverity">The lowest severity message type to retain</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null</exception>
        public FilteredLogger(ILogger logger, LogSeverity minSeverity)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            this.logger = logger;
            this.minSeverity = minSeverity;
        }

        /// <inheritdoc />
        protected override void LogInternal(LogSeverity severity, string message, Exception exception)
        {
            if (severity >= minSeverity)
                logger.Log(severity, message, exception);
        }
    }
}
