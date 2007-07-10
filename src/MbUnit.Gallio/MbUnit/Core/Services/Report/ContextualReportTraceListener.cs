using System.Diagnostics;

namespace MbUnit.Core.Services.Report
{
    /// <summary>
    /// A contextual report stream trace listener writes trace messages to a report stream
    /// associated with the test execution context at the time it was written.
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
            get { return ReportUtils.GetCurrentReport().Streams[reportStreamName]; }
        }

        private void WriteIndentIfNeeded()
        {
            if (NeedIndent)
                WriteIndent();
        }
    }
}