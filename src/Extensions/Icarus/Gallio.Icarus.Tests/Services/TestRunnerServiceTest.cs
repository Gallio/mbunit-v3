// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using MbUnit.Framework;
using Rhino.Mocks;
using Gallio.Runner;
using Gallio.Icarus.Services;
using Gallio.Runtime.ProgressMonitoring;
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
            using (var testRunnerService = new TestRunnerService())
                testRunnerService.TestRunner = testRunner;
            testRunner.AssertWasCalled(x => x.Dispose(Arg<IProgressMonitor>.Is.Anything));
        }

        [Test]
        public void TestStepFinished_Test()
        {
            var testRunner = MockRepository.GenerateStub<ITestRunner>();
            var testRunnerEvents = MockRepository.GenerateStub<ITestRunnerEvents>();
            testRunner.Stub(x => x.Events).Return(testRunnerEvents);
            var testRunnerService = new TestRunnerService {TestRunner = testRunner};
            TestStepFinishedEventArgs testStepFinishedEventArgs =  new TestStepFinishedEventArgs(new Report(), 
                new TestData("id", "name", "fullName"), new TestStepRun(new TestStepData("id", "name", "fullName", "testId")));
            testRunnerService.TestStepFinished += ((sender, e) => Assert.AreEqual(testStepFinishedEventArgs, e));
            testRunnerEvents.Raise(x => x.TestStepFinished += null, testRunnerEvents, testStepFinishedEventArgs);
        }

        [Test]
        public void Explore_Test()
        {
            var testRunner = MockRepository.GenerateStub<ITestRunner>();
            testRunner.Stub(x => x.Events).Return(MockRepository.GenerateStub<ITestRunnerEvents>());
            var report = new Report
                             {
                                 TestModel = new TestModelData(new TestData("id", "name", "fullName"))
                             };
            testRunner.Stub(x => x.Report).Return(new LockBox<Report>(report));
            var progressMonitor = MockProgressMonitor();
            var testRunnerService = new TestRunnerService { TestRunner = testRunner };
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
            var testRunnerService = new TestRunnerService { TestRunner = testRunner };
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
            var testRunnerService = new TestRunnerService();
            testRunnerService.TestRunner = testRunner;
            Assert.AreEqual(report, testRunner.Report);
        }

        static IProgressMonitor MockProgressMonitor()
        {
            var progressMonitor = MockRepository.GenerateStub<IProgressMonitor>();
            progressMonitor.Stub(x => x.BeginTask(Arg<string>.Is.Anything, Arg<double>.Is.Anything)).Return(new ProgressMonitorTaskCookie(progressMonitor));
            progressMonitor.Stub(x => x.CreateSubProgressMonitor(Arg<double>.Is.Anything)).Return(progressMonitor).Repeat.Any();
            return progressMonitor;
        }
    }
}
