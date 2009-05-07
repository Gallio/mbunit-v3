using System.Collections.Generic;
using Gallio.Runner.Reports;

namespace Gallio.Icarus.Controllers.EventArgs
{
    public class ExecutionLogUpdatedEventArgs : System.EventArgs
    {
        public IList<TestStepRun> TestStepRuns
        {
            get; private set;
        }

        public ExecutionLogUpdatedEventArgs(IList<TestStepRun> testStepRuns)
        {
            TestStepRuns = testStepRuns;
        }
    }
}
