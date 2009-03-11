using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runtime.Logging
{
    /// <summary>
    /// The severity prefix logger wraps another loggers and encodes the severity of a
    /// log entry into the log message itself in a standard form.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This wrapper is useful when the base logger may not be able to
    /// report the severity via other means or when severity information must be
    /// reconstructed from plain text.
    /// </para>
    /// </remarks>
    /// <seealso cref="SeverityPrefixParser"/>
    public class SeverityPrefixLogger : BaseLogger
    {
        private readonly ILogger inner;

        internal static readonly Dictionary<LogSeverity, string> SeverityPrefixes = new Dictionary<LogSeverity, string>()
        {
            { LogSeverity.Debug, "[Debug] " },
            { LogSeverity.Info, "[Info] " },
            { LogSeverity.Important, "[Important] " },
            { LogSeverity.Warning, "[Warning] " },
            { LogSeverity.Error, "[Error] " },
        };

        /// <summary>
        /// Creates a prefix logger.
        /// </summary>
        /// <param name="inner">The inner logger</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="inner"/> is null</exception>
        public SeverityPrefixLogger(ILogger inner)
        {
            if (inner == null)
                throw new ArgumentNullException("inner");

            this.inner = inner;
        }

        /// <inheritdoc />
        protected override void LogImpl(LogSeverity severity, string message, Exception exception)
        {
            inner.Log(severity, SeverityPrefixes[severity] + message, exception);
        }
    }
}
