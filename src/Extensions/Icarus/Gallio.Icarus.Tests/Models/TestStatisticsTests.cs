using Gallio.Icarus.Models;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Model;
using Gallio.Runtime.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Models
{
    public class TestStatisticsTests
    {
        [Test]
        public void TestStepFinished_should_increment_passed_if_test_step_passed()
        {
            var testStatistics = new TestStatistics();

            testStatistics.TestStepFinished(TestStatus.Passed);

            Assert.AreEqual(1, testStatistics.Passed.Value);
        }

        [Test]
        public void TestStepFinished_should_increment_failed_if_test_step_failed()
        {
            var testStatistics = new TestStatistics();

            testStatistics.TestStepFinished(TestStatus.Failed);

            Assert.AreEqual(1, testStatistics.Failed.Value);
        }

        [Test]
        public void TestStepFinished_should_increment_skipped_if_test_step_was_skipped()
        {
            var testStatistics = new TestStatistics();

            testStatistics.TestStepFinished(TestStatus.Skipped);

            Assert.AreEqual(1, testStatistics.Skipped.Value);
        }

        [Test]
        public void TestStepFinished_should_increment_inconclusive_if_test_step_was_inconclusive()
        {
            var testStatistics = new TestStatistics();

            testStatistics.TestStepFinished(TestStatus.Inconclusive);

            Assert.AreEqual(1, testStatistics.Inconclusive.Value);
        }

        [Test]
        public void Reset_should_set_passed_to_zero()
        {
            var testStatistics = new TestStatistics();
            testStatistics.TestStepFinished(TestStatus.Passed);

            testStatistics.Reset(MockProgressMonitor.Instance);

            Assert.AreEqual(0, testStatistics.Passed.Value);
        }

        [Test]
        public void Reset_should_set_failed_to_zero()
        {
            var testStatistics = new TestStatistics();
            testStatistics.TestStepFinished(TestStatus.Failed);

            testStatistics.Reset(MockProgressMonitor.Instance);

            Assert.AreEqual(0, testStatistics.Failed.Value);
        }

        [Test]
        public void Reset_should_set_skipped_to_zero()
        {
            var testStatistics = new TestStatistics();
            testStatistics.TestStepFinished(TestStatus.Skipped);

            testStatistics.Reset(MockProgressMonitor.Instance);

            Assert.AreEqual(0, testStatistics.Skipped.Value);
        }

        [Test]
        public void Reset_should_set_inconclusive_to_zero()
        {
            var testStatistics = new TestStatistics();
            testStatistics.TestStepFinished(TestStatus.Inconclusive);

            testStatistics.Reset(MockProgressMonitor.Instance);

            Assert.AreEqual(0, testStatistics.Inconclusive.Value);
        }

        [Test]
        public void Reset_should_set_progress_status()
        {
            var testStatistics = new TestStatistics();
            var progressMonitor = MockRepository.GenerateStub<IProgressMonitor>();
            progressMonitor.Stub(x => x.BeginTask(Arg<string>.Is.Anything, Arg<double>.Is.Anything)).Return(
                new ProgressMonitorTaskCookie(progressMonitor));

            testStatistics.Reset(progressMonitor);

            progressMonitor.AssertWasCalled(pm => pm.BeginTask("Resetting statistics.", 4));
        }
    }
}
