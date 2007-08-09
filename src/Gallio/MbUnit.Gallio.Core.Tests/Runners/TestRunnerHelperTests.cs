extern alias MbUnit2;
using System;
using MbUnit.Core.Runner;
using MbUnit.Framework.Kernel.Events;
using Castle.Core.Logging;
using MbUnit2::MbUnit.Framework;

namespace MbUnit.Core.Tests.Runners
{
    [TestFixture]
    [TestsOn(typeof(TestRunnerHelper))]
    [Author("Julian", "jhidalgo@mercatus.cl")]
    public class TestRunnerHelperTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestInstatiation_NullProgressMonitorMonitor()
        {
            TestRunnerHelper runner = new TestRunnerHelper(null, null, Verbosity.Quiet, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestInstatiation_NullLogger()
        {
            TestRunnerHelper runner = new TestRunnerHelper(
                delegate { return new NullProgressMonitor(); },
                null, Verbosity.Quiet, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestInstatiation_NullFilter()
        {
            TestRunnerHelper runner = new TestRunnerHelper(
                delegate { return new NullProgressMonitor(); },
                new ConsoleLogger(), Verbosity.Quiet, null);
        }
    }
}
