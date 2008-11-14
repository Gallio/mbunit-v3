using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using Rhino.Mocks;
using Gallio.Runner;
using Gallio.Icarus.Services;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime.Logging;
using Gallio.Runner.Events;
using Gallio.Runner.Reports;
using Gallio.Model.Serialization;
using Gallio.Model;
using Gallio.Concurrency;

namespace Gallio.Icarus.Tests.Services
{
    class TestRunnerServiceTest
    {
        [Test]
        public void Dispose_Test()
        {
            var testRunner = MockRepository.GenerateStub<ITestRunner>();
            testRunner.Stub(x => x.Events).Return(MockRepository.GenerateStub<ITestRunnerEvents>());
            using (var testRunnerService = new TestRunnerService(testRunner))
                Assert.IsTrue(true);
            testRunner.AssertWasCalled(x => x.Dispose(Arg<IProgressMonitor>.Is.Anything));
        }

        [Test]
        public void Initialize_Test()
        {
            var testRunner = MockRepository.GenerateStub<ITestRunner>();
            testRunner.Stub(x => x.Events).Return(MockRepository.GenerateStub<ITestRunnerEvents>());
            var testRunnerService = new TestRunnerService(testRunner);
            testRunnerService.Initialize();
            testRunner.AssertWasCalled(x => x.Initialize(Arg<TestRunnerOptions>.Is.Anything, 
                Arg<ILogger>.Is.Anything, Arg<IProgressMonitor>.Is.Anything));
        }

        [Test]
        public void TestStepFinished_Test()
        {
            var testRunner = MockRepository.GenerateStub<ITestRunner>();
            var testRunnerEvents = MockRepository.GenerateStub<ITestRunnerEvents>();
            testRunner.Stub(x => x.Events).Return(testRunnerEvents);
            var testRunnerService = new TestRunnerService(testRunner);
            TestStepFinishedEventArgs testStepFinishedEventArgs =  new TestStepFinishedEventArgs(new Report(), 
                new TestData("id", "name", "fullName"), new TestStepRun(new TestStepData("id", "name", "fullName", "testId")));
            testRunnerService.TestStepFinished += delegate(object sender, TestStepFinishedEventArgs e)
            {
                Assert.AreEqual(testStepFinishedEventArgs, e);
            };
            testRunnerEvents.Raise(x => x.TestStepFinished += null, testRunnerEvents, testStepFinishedEventArgs);
        }

        [Test]
        public void Explore_Test()
        {
            var testRunner = MockRepository.GenerateStub<ITestRunner>();
            testRunner.Stub(x => x.Events).Return(MockRepository.GenerateStub<ITestRunnerEvents>());
            var report = new Report();
            report.TestModel = new TestModelData(new TestData("id", "name", "fullName"));
            testRunner.Stub(x => x.Report).Return(new LockBox<Report>(report));
            var progressMonitor = MockProgressMonitor();
            var testRunnerService = new TestRunnerService(testRunner);
            Assert.AreEqual(report.TestModel, testRunnerService.Explore(progressMonitor));
            testRunner.AssertWasCalled(x => x.Explore(Arg<TestExplorationOptions>.Is.Anything, Arg.Is(progressMonitor)));
        }

        [Test]
        public void Load_Test()
        {
            var testRunner = MockRepository.GenerateStub<ITestRunner>();
            testRunner.Stub(x => x.Events).Return(MockRepository.GenerateStub<ITestRunnerEvents>());
            var report = new Report();
            testRunner.Stub(x => x.Report).Return(new LockBox<Report>(report));
            var progressMonitor = MockProgressMonitor();
            var testRunnerService = new TestRunnerService(testRunner);
            TestPackageConfig testPackageConfig = new TestPackageConfig();
            testRunnerService.Load(testPackageConfig, progressMonitor);
            testRunner.AssertWasCalled(x => x.Unload(progressMonitor));
            testRunner.AssertWasCalled(x => x.Load(testPackageConfig, progressMonitor));
        }

        [Test]
        public void Report_Test()
        {
            var testRunner = MockRepository.GenerateStub<ITestRunner>();
            testRunner.Stub(x => x.Events).Return(MockRepository.GenerateStub<ITestRunnerEvents>());
            var report = new LockBox<Report>(new Report());
            testRunner.Stub(x => x.Report).Return(report);
            var testRunnerService = new TestRunnerService(testRunner);
            Assert.AreEqual(report, testRunner.Report);
        }

        IProgressMonitor MockProgressMonitor()
        {
            var progressMonitor = MockRepository.GenerateStub<IProgressMonitor>();
            progressMonitor.Stub(x => x.BeginTask(Arg<string>.Is.Anything, Arg<double>.Is.Anything)).Return(new ProgressMonitorTaskCookie(progressMonitor));
            progressMonitor.Stub(x => x.CreateSubProgressMonitor(Arg<double>.Is.Anything)).Return(progressMonitor).Repeat.Any();
            return progressMonitor;
        }
    }
}
