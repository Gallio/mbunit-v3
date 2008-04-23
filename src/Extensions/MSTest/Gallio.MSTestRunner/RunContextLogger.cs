using System;
using Gallio.Runtime.Logging;
using Microsoft.VisualStudio.TestTools.Common;
using Microsoft.VisualStudio.TestTools.Execution;

namespace Gallio.MSTestRunner
{
    internal class RunContextLogger : BaseLogger
    {
        private readonly IRunContext runContext;

        public RunContextLogger(IRunContext runContext)
        {
            this.runContext = runContext;
        }

        protected override void LogInternal(LogSeverity severity, string message, Exception exception)
        {
            TestOutcome outcome;
            if (severity == LogSeverity.Warning)
                outcome = TestOutcome.Warning;
            else if (severity == LogSeverity.Error)
                outcome = TestOutcome.Error;
            else
                return;

            TestRunTextResultMessage resultMessage = new TestRunTextResultMessage(runContext.RunConfig.TestRun.Id, message, exception);
            resultMessage.Outcome = outcome;

            runContext.ResultSink.AddResult(resultMessage);
        }
    }
}
