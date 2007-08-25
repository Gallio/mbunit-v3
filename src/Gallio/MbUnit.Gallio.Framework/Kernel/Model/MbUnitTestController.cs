using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using MbUnit.Framework.Kernel.Events;
using MbUnit.Framework.Kernel.ExecutionLogs;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Controls the execution of MbUnit tests.
    /// </summary>
    public class MbUnitTestController : ITestController
    {
        private TextReader oldConsoleIn;
        private TextWriter oldConsoleOut;
        private TextWriter oldConsoleError;

        private ContextualExecutionLogTraceListener debugListener;
        private ContextualExecutionLogTraceListener traceListener;

        /// <inheritdoc />
        public void Run(IProgressMonitor progressMonitor, TestExecutionOptions options, IEventListener listener,
            IList<ITest> tests)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private void SetUp()
        {
            // Save the old console streams.
            oldConsoleIn = Console.In;
            oldConsoleOut = Console.Out;
            oldConsoleError = Console.Error;

            // Inject debug and trace listeners.
            debugListener = new ContextualExecutionLogTraceListener(ExecutionLogStreams.Debug);
            traceListener = new ContextualExecutionLogTraceListener(ExecutionLogStreams.Trace);
            Debug.Listeners.Add(debugListener);
            Debug.AutoFlush = true;

            Trace.Listeners.Add(traceListener);
            Trace.AutoFlush = true;

            // Inject console streams.

        }

        private void TearDown()
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
