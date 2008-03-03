using MbUnit.Framework;

namespace Gallio.Icarus.Tests
{
    [TestFixture]
    public class PerformanceMonitorTest
    {
        [Test]
        public void UpdateTestResults_Test()
        {
            PerformanceMonitor performanceMonitor = new PerformanceMonitor();
            performanceMonitor.UpdateTestResults("Passed", "Type", "Namespace", "Assembly");
        }
    }
}
