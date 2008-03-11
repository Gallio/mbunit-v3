using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Utilities;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// Utilities for <see cref="ITestLogWriter" />.
    /// </summary>
    public static class TestLogWriterUtils
    {
        /// <summary>
        /// Writes an exception to the log within its own section with the specified name.
        /// </summary>
        /// <param name="writer">The log writer</param>
        /// <param name="streamName">The stream name</param>
        /// <param name="exception">The exception to write</param>
        /// <param name="sectionName">The section name</param>
        public static void WriteException(ITestLogWriter writer, string streamName, Exception exception, string sectionName)
        {
            writer.BeginSection(streamName, sectionName);
            writer.Write(streamName, ExceptionUtils.SafeToString(exception));
            writer.EndSection(streamName);
        }
    }
}
