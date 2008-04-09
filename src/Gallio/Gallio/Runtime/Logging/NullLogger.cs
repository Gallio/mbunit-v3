using System;

namespace Gallio.Runtime.Logging
{
    /// <summary>
    /// A null implementation of <see cref="ILogger" /> that does nothing.
    /// </summary>
    public sealed class NullLogger : BaseLogger
    {
        /// <summary>
        /// Gets a singleton instance of the null logger.
        /// </summary>
        public static readonly NullLogger Instance = new NullLogger();

        /// <inheritdoc />
        protected override void LogInternal(LogSeverity severity, string message, Exception exception)
        {
        }
    }
}
