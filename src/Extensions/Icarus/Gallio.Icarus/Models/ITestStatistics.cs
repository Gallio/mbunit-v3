using Gallio.Model;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Models
{
    public interface ITestStatistics
    {
        Observable<int> Passed { get; }
        Observable<int> Failed { get; }
        Observable<int> Skipped { get; }
        Observable<int> Inconclusive { get; }
        void Reset(IProgressMonitor progressMonitor);
        void TestStepFinished(TestStatus testStatus);
    }
}