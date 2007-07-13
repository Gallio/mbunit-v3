using System;
using System.Diagnostics;
using System.IO;
using MbUnit.Framework.Services.Reports;

namespace MbUnit.Framework.Core
{
    /// <summary>
    /// Sets up and tears down the environment used for running MbUnit tests.
    /// </summary>
    public class MbUnitTestHarness
    {
        private TextReader oldConsoleIn;
        private TextWriter oldConsoleOut;
        private TextWriter oldConsoleError;

        private ContextualReportTraceListener debugListener;
        private ContextualReportTraceListener traceListener;

        public void SetUp()
        {
            // Save the old console streams.
            oldConsoleIn = Console.In;
            oldConsoleOut = Console.Out;
            oldConsoleError = Console.Error;

            // Inject debug and trace listeners.
            debugListener = new ContextualReportTraceListener(Report.DebugStreamName);
            traceListener = new ContextualReportTraceListener(Report.TraceStreamName);
            Debug.Listeners.Add(debugListener);
            Debug.AutoFlush = true;

            Trace.Listeners.Add(traceListener);
            Trace.AutoFlush = true;

            // Inject console streams.

        }

        public void TearDown()
        {
            // Remove debug and trace listeners.
            Debug.Listeners.Remove(debugListener);
            Trace.Listeners.Remove(traceListener);

            // Restore the old console streams.
            Console.SetIn(oldConsoleIn);
            Console.SetOut(oldConsoleOut);
            Console.SetError(oldConsoleError);
        }
    }
}
