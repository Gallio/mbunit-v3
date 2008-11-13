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

using System.Collections.Generic;
using Gallio.Concurrency;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Models.Interfaces;
using Gallio.Icarus.Services.Interfaces;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner.Reports;
using Gallio.Runtime.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;
using Gallio.Model.Serialization;

namespace Gallio.Icarus.Tests.Controllers
{
    [Category("Controllers"), Author("Graham Hay"), TestsOn(typeof(TestController))]
    class TestControllerTest : MockTest
    {
        [Test]
        public void ApplyFilter_Test()
        {
            Filter<ITest> filter = new NoneFilter<ITest>();
            ITestRunnerService testRunnerService = SetupTestRunnerService();
            testRunnerService.SetFilter(filter, null);
            LastCall.IgnoreArguments();

            var testTreeModel = mocks.StrictMock<ITestTreeModel>();
            var progressMonitor = mocks.StrictMock<IProgressMonitor>();
            LastCall.IgnoreArguments();
            Expect.Call(progressMonitor.BeginTask("Applying filter", 3)).Return(
                new ProgressMonitorTaskCookie(progressMonitor));
            progressMonitor.SetStatus("Parsing filter");
            progressMonitor.Worked(1);
            testTreeModel.ApplyFilter(filter, progressMonitor);
            LastCall.IgnoreArguments();
            Expect.Call(progressMonitor.CreateSubProgressMonitor(1)).Return(
                (IProgressMonitor)mocks.Stub(typeof(IProgressMonitor))).Repeat
                .Times(2);
            progressMonitor.Done();
            mocks.ReplayAll();
            var testController = new TestController(testRunnerService, testTreeModel);
            testController.ApplyFilter(filter.ToFilterExpr(), progressMonitor);
        }

        ITestRunnerService SetupTestRunnerService()
        {
            ITestRunnerService testRunnerService = mocks.StrictMock<ITestRunnerService>();
            testRunnerService.TestStepFinished += delegate { };
            LastCall.IgnoreArguments();
            return testRunnerService;
        }

        [Test]
        public void GetCurrentFilter_Test_AnyFilter()
        {
            Filter<ITest> filter = new NoneFilter<ITest>();
            ITestTreeModel testTreeModel = mocks.StrictMock<ITestTreeModel>();
            Expect.Call(testTreeModel.GetCurrentFilter(null)).Return(filter).IgnoreArguments();
            ITestRunnerService testRunnerService = SetupTestRunnerService();
            testRunnerService.SetFilter(filter, null);
            LastCall.IgnoreArguments();
            var progressMonitor = mocks.StrictMock<IProgressMonitor>();
            Expect.Call(progressMonitor.BeginTask("Getting current filter", 2)).Return(
                new ProgressMonitorTaskCookie(progressMonitor));
            Expect.Call(progressMonitor.CreateSubProgressMonitor(1)).Return((IProgressMonitor)mocks.Stub(
                typeof(IProgressMonitor))).Repeat.Times(2);
            progressMonitor.Done();
            mocks.ReplayAll();
            TestController testController = new TestController(testRunnerService, testTreeModel);
            Assert.AreEqual(filter, testController.GetCurrentFilter(progressMonitor));
        }

        [Test]
        public void Selected_Tests_Test()
        {
            ITestRunnerService testRunnerService = SetupTestRunnerService();
            ITestTreeModel testTreeModel = mocks.StrictMock<ITestTreeModel>();

            mocks.ReplayAll();

            TestController testController = new TestController(testRunnerService, testTreeModel);
            Assert.AreEqual(0, testController.SelectedTests.Count);
        }

        [Test]
        public void Model_Test()
        {
            ITestRunnerService testRunnerService = SetupTestRunnerService();
            ITestTreeModel testTreeModel = mocks.StrictMock<ITestTreeModel>();

            mocks.ReplayAll();

            TestController testController = new TestController(testRunnerService, testTreeModel);
            Assert.AreEqual(testTreeModel, testController.Model);
        }

        [Test]
        public void TestFrameworks_Test()
        {
            List<string> testFrameworks = new List<string>();
            ITestRunnerService testRunnerService = SetupTestRunnerService();
            Expect.Call(testRunnerService.TestFrameworks).Return(testFrameworks);
            ITestTreeModel testTreeModel = mocks.StrictMock<ITestTreeModel>();

            mocks.ReplayAll();

            TestController testController = new TestController(testRunnerService, testTreeModel);
            Assert.AreEqual(testFrameworks, testController.TestFrameworks);
        }

        [Test]
        public void TestCount_Test()
        {
            ITestRunnerService testRunnerService = SetupTestRunnerService();
            ITestTreeModel testTreeModel = mocks.StrictMock<ITestTreeModel>();
            const int testCount = 0;
            Expect.Call(testTreeModel.TestCount).Return(testCount);

            mocks.ReplayAll();

            TestController testController = new TestController(testRunnerService, testTreeModel);
            Assert.AreEqual(testCount, testController.TestCount);
        }

        [Test]
        public void ResetTests_Test()
        {
            var testRunnerService = MockRepository.GenerateStub<ITestRunnerService>();
            testRunnerService.Stub(x => x.Report).Return(new LockBox<Report>(new Report()));
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testRunnerService, testTreeModel);
            var progressMonitor = MockProgressMonitor();
            testController.ResetTests(progressMonitor);
            testTreeModel.AssertWasCalled(x => x.ResetTestStatus(progressMonitor));
        }

        IProgressMonitor MockProgressMonitor()
        {
            var progressMonitor = MockRepository.GenerateStub<IProgressMonitor>();
            progressMonitor.Stub(x => x.BeginTask(Arg<string>.Is.Anything, Arg<double>.Is.Anything)).Return(new ProgressMonitorTaskCookie(progressMonitor));
            progressMonitor.Stub(x => x.CreateSubProgressMonitor(Arg<double>.Is.Anything)).Return(progressMonitor).Repeat.Any();
            return progressMonitor;
        }

        [Test]
        public void Reload_Test()
        {
            var testRunnerService = MockRepository.GenerateStub<ITestRunnerService>();
            testRunnerService.Stub(x => x.Report).Return(new LockBox<Report>(new Report()));
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var progressMonitor = MockRepository.GenerateStub<IProgressMonitor>();
            progressMonitor.Stub(x => x.BeginTask(Arg<string>.Is.Anything, Arg<double>.Is.Anything)).Return(new ProgressMonitorTaskCookie(progressMonitor));
            progressMonitor.Stub(x => x.CreateSubProgressMonitor(Arg<double>.Is.Anything)).Return(progressMonitor);
            var testController = new TestController(testRunnerService, testTreeModel);
            bool loadStarted = false;
            testController.LoadStarted += delegate { loadStarted = true; };
            bool loadFinished = false;
            testController.LoadFinished += delegate { loadFinished = true; };
            testController.Reload(new TestPackageConfig(), progressMonitor);
            Assert.IsTrue(loadStarted);
            Assert.IsTrue(loadFinished);
        }

        [Test]
        public void Report_Test()
        {
            var report = new LockBox<Report>();
            var testRunnerService = MockRepository.GenerateStub<ITestRunnerService>();
            testRunnerService.Stub(x => x.Report).Return(report);
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testRunnerService, testTreeModel);
            Assert.AreEqual(report, testController.Report);
        }

        [Test]
        public void TreeViewCategory_Test()
        {
            var testRunnerService = MockRepository.GenerateStub<ITestRunnerService>();
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testRunnerService, testTreeModel);
            Assert.IsNull(testController.TreeViewCategory);
            const string treeViewCategory = "test";
            testController.TreeViewCategory = treeViewCategory;
            Assert.AreEqual(treeViewCategory, testController.TreeViewCategory);
        }

        [Test]
        public void RunTests_Test()
        {
            var testRunnerService = MockRepository.GenerateStub<ITestRunnerService>();
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var progressMonitor = MockRepository.GenerateStub<IProgressMonitor>();
            testRunnerService.Stub(x => x.Explore(progressMonitor)).Return(new TestModelData(new TestData("id", "name", "fullName")));
            progressMonitor.Stub(x => x.BeginTask(Arg<string>.Is.Anything, Arg<double>.Is.Anything)).Return(new ProgressMonitorTaskCookie(progressMonitor));
            progressMonitor.Stub(x => x.CreateSubProgressMonitor(Arg<double>.Is.Anything)).Return(progressMonitor).Repeat.Any();
            var testController = new TestController(testRunnerService, testTreeModel);
            bool runStarted = false;
            bool runFinished = false;
            testController.RunStarted += delegate { runStarted = true; };
            testController.RunFinished += delegate { runFinished = true; };
            testController.RunTests(progressMonitor);
            Assert.IsTrue(runStarted);
            testTreeModel.AssertWasCalled(x => x.ResetTestStatus(Arg<IProgressMonitor>.Is.Anything));
            testRunnerService.AssertWasCalled(x => x.Load(Arg<TestPackageConfig>.Is.Anything, Arg.Is(progressMonitor)));
            testRunnerService.AssertWasCalled(x => x.Run(progressMonitor));
            Assert.IsTrue(runFinished);
        }
    }
}
