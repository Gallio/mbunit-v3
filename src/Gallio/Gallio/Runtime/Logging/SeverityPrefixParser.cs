using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runtime.Logging
{
    /// <summary>
    /// Parses severity information from log messages encoded by <see cref="SeverityPrefixLogger" />.
    /// </summary>
    /// <seealso cref="SeverityPrefixLogger"/>
    public class SeverityPrefixParser
    {
        private LogSeverity previousSeverity = LogSeverity.Info;

        /// <summary>
        /// Parses a line of log data.
        /// </summary>
        /// <param name="line">The line of log data to parse</param>
        /// <param name="severity">Set to the severity of the message extracted from the line</param>
        /// <param name="message">The message extracted from the line</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="line"/> is null</exception>
        public void ParseLine(string line, out LogSeverity severity, out string message)
        {
            if (line == null)
                throw new ArgumentNullException("line");

            foreach (var pair in SeverityPrefixLogger.SeverityPrefixes)
            {
                if (line.StartsWith(pair.Value))
                {
                    severity = pair.Key;
                    previousSeverity = severity;
                    message = line.Substring(pair.Value.Length);
                    return;
                }
            }

            severity = previousSeverity;
            message = line;
        }
    }
}
