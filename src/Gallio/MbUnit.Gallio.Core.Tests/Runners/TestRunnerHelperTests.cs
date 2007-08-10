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
        public void TestInstantiation_NullProgressMonitorMonitor()
        {
            new TestRunnerHelper(null, null, "");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestInstantiation_NullLogger()
        {
            new TestRunnerHelper(
                delegate { return new NullProgressMonitor(); },
                null, "");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestInstantiation_NullFilter()
        {
            new TestRunnerHelper(
                delegate { return new NullProgressMonitor(); },
                new ConsoleLogger(), "");
        }
    }
}
