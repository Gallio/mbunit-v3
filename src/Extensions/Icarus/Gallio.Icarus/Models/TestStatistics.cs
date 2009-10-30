using Gallio.Model;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Models
{
    internal class TestStatistics : ITestStatistics
    {
        public Observable<int> Passed { get; private set; }
        public Observable<int> Failed { get; private set; }
        public Observable<int> Skipped { get; private set; }
        public Observable<int> Inconclusive { get; private set; }

        public TestStatistics()
        {
            Passed = new Observable<int>();
            Failed = new Observable<int>();
            Skipped = new Observable<int>();
            Inconclusive = new Observable<int>();
        }

        public void Reset(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Resetting statistics.", 4))
            {
                Passed.Value = 0;
                progressMonitor.Worked(1);
                Failed.Value = 0;
                progressMonitor.Worked(1);
                Skipped.Value = 0;
                progressMonitor.Worked(1);
                Inconclusive.Value = 0;
            }
        }

        public void TestStepFinished(TestStatus testStatus)
        {
            switch (testStatus)
            {
                case TestStatus.Passed:
                    Passed.Value++;
                    break;
                case TestStatus.Failed:
                    Failed.Value++;
                    break;
                case TestStatus.Skipped:
                    Skipped.Value++;
                    break;
                case TestStatus.Inconclusive:
                    Inconclusive.Value++;
                    break;
            }
        }
    }
}
