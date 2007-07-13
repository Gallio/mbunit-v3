using System.Diagnostics;
using MbUnit.Framework.Services.Reports;

namespace MbUnit.Framework.Services.Reports
{
    /// <summary>
    /// A contextual report stream trace listener writes trace messages to a stream
    /// in the report associated with the current test execution context at the time
    /// the message was written.
    /// </summary>
    public class ContextualReportTraceListener : TraceListener
    {
        private string reportStreamName;

        /// <summary>
        /// Creates a trace listener that writes to the specified report stream.
        /// </summary>
        /// <param name="reportStreamName">The report stream name</param>
        public ContextualReportTraceListener(string reportStreamName)
        {
            this.reportStreamName = reportStreamName;
        }

        public override void Write(string message)
        {
            WriteIndentIfNeeded();

            CurrentStream.Write(message);
        }

        public override void WriteLine(string message)
        {
            WriteIndentIfNeeded();

            CurrentStream.WriteLine(message);
        }

        private IReportStream CurrentStream
        {
            get { return Report.Streams[reportStreamName]; }
        }

        private void WriteIndentIfNeeded()
        {
            if (NeedIndent)
                WriteIndent();
        }
    }
}