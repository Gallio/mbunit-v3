using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.MSTestAdapter.Model;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.MSTestAdapter.Tests.Model
{
    class StubbedMSTestController : MSTestController
    {
        public StubbedMSTestController(IMSTestProcess msTestProcess)
            : base(msTestProcess)
        {
        }

        public new TestOutcome RunTestsImpl(ITestCommand rootTestCommand, ITestStep parentTestStep, TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            return base.RunTestsImpl(rootTestCommand, parentTestStep, options, progressMonitor);
        }
    }
}
