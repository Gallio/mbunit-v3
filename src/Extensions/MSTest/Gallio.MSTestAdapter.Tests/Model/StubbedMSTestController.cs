using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.MSTestAdapter.Model;
using Gallio.MSTestAdapter.Wrapper;
using Gallio.Runner.Caching;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.MSTestAdapter.Tests.Model
{
    internal class StubbedMSTestController : MSTestController
    {
        public StubbedMSTestController(IMSTestCommand msTestCommand, IDiskCache cache)
            : base(msTestCommand, cache)
        {
        }

        public new TestOutcome RunTestsImpl(ITestCommand rootTestCommand, ITestStep parentTestStep, TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            return base.RunTestsImpl(rootTestCommand, parentTestStep, options, progressMonitor);
        }
    }
}
