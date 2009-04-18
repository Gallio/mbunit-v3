// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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

using System;
using System.ComponentModel;
using System.Threading;
using Gallio.Concurrency;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Models.Interfaces;
using Gallio.Icarus.Utilities;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Filters;
using Gallio.Runner;
using Gallio.Runner.Events;
using Gallio.Runner.Reports;
using Gallio.Runtime.Logging;
using MbUnit.Framework;
using Rhino.Mocks;
using Gallio.Model.Serialization;

namespace Gallio.Icarus.Tests.Controllers
{
    [MbUnit.Framework.Category("Controllers"), Author("Graham Hay"), TestsOn(typeof(TestController))]
    class TestControllerTest
    {
        [Test]
        public void ApplyFilter_Test()
        {
            Filter<ITest> filter = new NoneFilter<ITest>();
            FilterSet<ITest> filterSet = new FilterSet<ITest>(filter);

            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testTreeModel);
            testController.ApplyFilterSet(filterSet);
            testTreeModel.AssertWasCalled(ttm => ttm.ApplyFilterSet(filterSet));
        }

        [Test]
        public void Explore_Test()
        {
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testTreeModel);
            var exploreStartedFlag = false;
            testController.ExploreStarted += delegate { exploreStartedFlag = true; };
            var exploreFinishedFlag = false;
            testController.ExploreFinished += delegate { exploreFinishedFlag = true; };
            var testRunnerFactory = MockRepository.GenerateStub<ITestRunnerFactory>();
            var testRunner = MockRepository.GenerateStub<ITestRunner>();
            var testRunnerEvents = MockRepository.GenerateStub<ITestRunnerEvents>();
            testRunner.Stub(tr => tr.Events).Return(testRunnerEvents);
            testRunnerFactory.Stub(trf => trf.CreateTestRunner()).Return(testRunner);
            testController.SetTestRunnerFactory(testRunnerFactory);
            const string treeViewCategory = "treeViewCategory";
            testController.TreeViewCategory = treeViewCategory;

            testController.Explore(progressMonitor);
            
            Assert.IsTrue(exploreStartedFlag);
            testRunner.AssertWasCalled(tr => tr.Initialize(Arg<TestRunnerOptions>.Is.Anything, 
                Arg<ILogger>.Is.Anything, Arg.Is(progressMonitor)));
            testRunner.AssertWasCalled(tr => tr.Explore(Arg<TestPackageConfig>.Is.Anything, 
                Arg<TestExplorationOptions>.Is.Anything, Arg.Is(progressMonitor)));
            testTreeModel.AssertWasCalled(ttm => ttm.BuildTestTree(Arg.Is(progressMonitor), Arg<TestModelData>.Is.Anything, 
                Arg.Is(treeViewCategory)));
            Assert.IsTrue(exploreFinishedFlag);
        }

        [Test]
        public void TestStepFinished_Test()
        {
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testTreeModel);
            var testRunnerFactory = MockRepository.GenerateStub<ITestRunnerFactory>();
            var testRunner = MockRepository.GenerateStub<ITestRunner>();
            var testRunnerEvents = MockRepository.GenerateStub<ITestRunnerEvents>();
            testRunner.Stub(tr => tr.Events).Return(testRunnerEvents);
            testRunnerFactory.Stub(trf => trf.CreateTestRunner()).Return(testRunner);
            testController.SetTestRunnerFactory(testRunnerFactory);
            testController.Explore(progressMonitor);
            TestStepFinishedEventArgs testStepFinishedEventArgs = new TestStepFinishedEventArgs(new Report(), 
                new TestData("id", "name", "fullName"), 
                new TestStepRun(new TestStepData("id", "name", "fullName", "testId")));
            bool testStepFinishedFlag = false;
            testController.TestStepFinished += delegate { testStepFinishedFlag = true; };

            testRunnerEvents.Raise(tre => tre.TestStepFinished += null, testRunner, testStepFinishedEventArgs);

            testTreeModel.AssertWasCalled(ttm => ttm.UpdateTestStatus(testStepFinishedEventArgs.Test, 
                testStepFinishedEventArgs.TestStepRun));
            Assert.IsTrue(testStepFinishedFlag);
        }

        [Test]
        public void FailedTests_Test()
        {
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testTreeModel);
            var testRunnerFactory = MockRepository.GenerateStub<ITestRunnerFactory>();
            var testRunner = MockRepository.GenerateStub<ITestRunner>();
            var testRunnerEvents = MockRepository.GenerateStub<ITestRunnerEvents>();
            testRunner.Stub(tr => tr.Events).Return(testRunnerEvents);
            testRunnerFactory.Stub(trf => trf.CreateTestRunner()).Return(testRunner);
            testController.SetTestRunnerFactory(testRunnerFactory);
            testController.Explore(progressMonitor);
            var testStepRun = new TestStepRun(new TestStepData("id", "name", "fullName", "testId"))
                                  {
                                      Result = new TestResult { Outcome = new TestOutcome(TestStatus.Failed) }
                                  };
            TestStepFinishedEventArgs testStepFinishedEventArgs = new TestStepFinishedEventArgs(new Report(),
                new TestData("id", "name", "fullName"),
                testStepRun);
            Assert.IsFalse(testController.FailedTests);

            testRunnerEvents.Raise(tre => tre.TestStepFinished += null, testRunner, testStepFinishedEventArgs);

            Assert.IsTrue(testController.FailedTests);
        }

        [Test]
        public void RunStarted_Test()
        {
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testTreeModel);
            var testRunnerFactory = MockRepository.GenerateStub<ITestRunnerFactory>();
            var testRunner = MockRepository.GenerateStub<ITestRunner>();
            var testRunnerEvents = MockRepository.GenerateStub<ITestRunnerEvents>();
            testRunner.Stub(tr => tr.Events).Return(testRunnerEvents);
            testRunnerFactory.Stub(trf => trf.CreateTestRunner()).Return(testRunner);
            testController.SetTestRunnerFactory(testRunnerFactory);
            testController.Explore(progressMonitor);
            Report report = new Report();

            testRunnerEvents.Raise(tre => tre.RunStarted += null, testRunner,
                                   new RunStartedEventArgs(new TestPackageConfig(), new TestExplorationOptions(),
                                                           new TestExecutionOptions(), new LockBox<Report>(report)));

            testController.ReadReport(r => Assert.AreEqual(r, report));
        }

        [Test]
        public void ExploreStarted_Test()
        {
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testTreeModel);
            var testRunnerFactory = MockRepository.GenerateStub<ITestRunnerFactory>();
            var testRunner = MockRepository.GenerateStub<ITestRunner>();
            var testRunnerEvents = MockRepository.GenerateStub<ITestRunnerEvents>();
            testRunner.Stub(tr => tr.Events).Return(testRunnerEvents);
            testRunnerFactory.Stub(trf => trf.CreateTestRunner()).Return(testRunner);
            testController.SetTestRunnerFactory(testRunnerFactory);
            testController.Explore(progressMonitor);
            Report report = new Report();

            testRunnerEvents.Raise(tre => tre.ExploreStarted += null, testRunner, 
                new ExploreStartedEventArgs(new TestPackageConfig(), new TestExplorationOptions(), 
                new LockBox<Report>(report)));

            testController.ReadReport(r => Assert.AreEqual(r, report));
        }

        [Test]
        public void DoWithTestRunner_should_throw_if_testRunnerFactory_is_null()
        {
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testTreeModel);

            Assert.Throws<InvalidOperationException>(() => testController.Explore(progressMonitor));
        }

        [Test]
        public void FilterFailed_Get_Test()
        {
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            testTreeModel.Stub(ttm => ttm.FilterFailed).Return(true);
            var testController = new TestController(testTreeModel);

            Assert.IsTrue(testController.FilterFailed);
        }

        [Test]
        public void FilterFailed_Set_Test()
        {
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testTreeModel);

            Assert.IsFalse(testController.FilterFailed);
            testController.FilterFailed = true;
            testTreeModel.AssertWasCalled(ttm => ttm.SetFilter(TestStatus.Failed));
        }

        [Test]
        public void FilterInconclusive_Get_Test()
        {
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            testTreeModel.Stub(ttm => ttm.FilterInconclusive).Return(true);
            var testController = new TestController(testTreeModel);

            Assert.IsTrue(testController.FilterInconclusive);
        }

        [Test]
        public void FilterInconclusive_Set_Test()
        {
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testTreeModel);

            Assert.IsFalse(testController.FilterInconclusive);
            testController.FilterInconclusive = true;
            testTreeModel.AssertWasCalled(ttm => ttm.SetFilter(TestStatus.Inconclusive));
        }

        [Test]
        public void FilterPassed_Get_Test()
        {
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            testTreeModel.Stub(ttm => ttm.FilterPassed).Return(true);
            var testController = new TestController(testTreeModel);

            Assert.IsTrue(testController.FilterPassed);
        }

        [Test]
        public void FilterPassed_Set_Test()
        {
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testTreeModel);

            Assert.IsFalse(testController.FilterPassed);
            testController.FilterPassed = true;
            testTreeModel.AssertWasCalled(ttm => ttm.SetFilter(TestStatus.Passed));
        }        

        [Test]
        public void GenerateFilterFromSelectedTests_Test()
        {
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var filter = new FilterSet<ITest>(new NoneFilter<ITest>());
            testTreeModel.Stub(ttm => ttm.GenerateFilterSetFromSelectedTests()).Return(filter);
            var testController = new TestController(testTreeModel);

            Assert.AreEqual(filter, testController.GenerateFilterSetFromSelectedTests());
        }

        [Test]
        public void Model_Test()
        {
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testTreeModel);

            Assert.AreEqual(testTreeModel, testController.Model);
        }

        [Test]
        public void ResetTestStatus_Test()
        {
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testTreeModel);
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();

            testController.ResetTestStatus(progressMonitor);
            testTreeModel.AssertWasCalled(ttm => ttm.ResetTestStatus(progressMonitor));
        }

        [Test]
        public void RunStarted_is_fired_when_running_tests()
        {
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var filter = new FilterSet<ITest>(new NoneFilter<ITest>());
            testTreeModel.Stub(ttm => ttm.GenerateFilterSetFromSelectedTests()).Return(filter);
            var testController = new TestController(testTreeModel);
            var runStartedFlag = false;
            testController.RunStarted += delegate { runStartedFlag = true; };
            var testRunnerFactory = MockRepository.GenerateStub<ITestRunnerFactory>();
            var testRunner = MockRepository.GenerateStub<ITestRunner>();
            var testRunnerEvents = MockRepository.GenerateStub<ITestRunnerEvents>();
            testRunner.Stub(tr => tr.Events).Return(testRunnerEvents);
            testRunnerFactory.Stub(trf => trf.CreateTestRunner()).Return(testRunner);
            testController.SetTestRunnerFactory(testRunnerFactory);

            testController.Run(false, progressMonitor);

            Assert.IsTrue(runStartedFlag);
        }

        [Test]
        public void Run_Test()
        {
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var filter = new FilterSet<ITest>(new NoneFilter<ITest>());
            testTreeModel.Stub(ttm => ttm.GenerateFilterSetFromSelectedTests()).Return(filter);
            var testController = new TestController(testTreeModel);
            var runStartedFlag = false;
            testController.RunStarted += delegate { runStartedFlag = true; };
            var runFinishedFlag = false;
            testController.RunFinished += delegate { runFinishedFlag = true; };
            var testRunnerFactory = MockRepository.GenerateStub<ITestRunnerFactory>();
            var testRunner = MockRepository.GenerateStub<ITestRunner>();
            var testRunnerEvents = MockRepository.GenerateStub<ITestRunnerEvents>();
            testRunner.Stub(tr => tr.Events).Return(testRunnerEvents);
            testRunnerFactory.Stub(trf => trf.CreateTestRunner()).Return(testRunner);
            testController.SetTestRunnerFactory(testRunnerFactory);
            var testPackageConfig = new TestPackageConfig();
            testPackageConfig.AssemblyFiles.Add("test");

            testController.SetTestPackageConfig(testPackageConfig);
            testController.Run(false, progressMonitor);

            Assert.IsTrue(runStartedFlag);
            testTreeModel.AssertWasCalled(ttm => ttm.GenerateFilterSetFromSelectedTests());
            testRunner.AssertWasCalled(tr => tr.Run(Arg<TestPackageConfig>.Matches(tpc => tpc.AssemblyFiles.Count == 1), 
                Arg<TestExplorationOptions>.Is.Anything, 
                Arg<TestExecutionOptions>.Matches(teo => ((teo.FilterSet == filter) && teo.ExactFilter)), 
                Arg.Is(progressMonitor)));
            Assert.IsTrue(runFinishedFlag);
        }

        [Test]
        public void SelectedTests_Test()
        {
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testTreeModel);

            Assert.AreEqual(0, testController.SelectedTests.Count);
        }

        [Test]
        public void SetTestPackageConfig_should_throw_if_test_package_is_null()
        {
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testTreeModel);

            Assert.Throws<ArgumentNullException>(() => testController.SetTestPackageConfig(null));
        }

        [Test]
        public void SetTestRunnerFactory_should_throw_if_test_factory_is_null()
        {
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testTreeModel);

            Assert.Throws<ArgumentNullException>(() => testController.SetTestRunnerFactory(null));
        }

        [Test]
        public void SortOrder_get()
        {
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testTreeModel);
            testTreeModel.Stub(ttm => ttm.SortAsc).Return(true);

            Assert.IsTrue(testController.SortAsc);
        }

        [Test]
        public void SortAsc_set_true()
        {
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testTreeModel);

            testController.SortAsc = true;

            testTreeModel.AssertWasCalled(ttm => ttm.SetSortOrder(System.Windows.Forms.SortOrder.Ascending));
        }

        [Test]
        public void SortAsc_set_false()
        {
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testTreeModel);

            testController.SortAsc = false;

            testTreeModel.AssertWasCalled(ttm => ttm.SetSortOrder(System.Windows.Forms.SortOrder.None));
        }

        [Test]
        public void SortAsc_set_should_fire_prop_changed_for_opposite()
        {
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testTreeModel);
            var synchronizationContext = MockRepository.GenerateStub<ISynchronizationContext>();
            bool sortDescFlag = false;
            testController.PropertyChanged += delegate(object e, PropertyChangedEventArgs sender) 
            {
                if (sender.PropertyName == "SortDesc")
                    sortDescFlag = true; 
            };

            testController.SynchronizationContext = synchronizationContext;
            testController.SortAsc = true;

            var args = synchronizationContext.GetArgumentsForCallsMadeOn(sc => sc.Send(Arg<SendOrPostCallback>.Is.Anything, 
                Arg.Is(testController)))[0];
            SendOrPostCallback cb = (SendOrPostCallback)args[0];
            cb(args[1]);
            Assert.IsTrue(sortDescFlag);
        }

        [Test]
        public void SortDesc_set_true()
        {
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            
            new TestController(testTreeModel) { SortDesc = true };

            testTreeModel.AssertWasCalled(ttm => ttm.SetSortOrder(System.Windows.Forms.SortOrder.Descending));
        }

        [Test]
        public void SortDesc_set_false()
        {
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            
            new TestController(testTreeModel) { SortDesc = false };

            testTreeModel.AssertWasCalled(ttm => ttm.SetSortOrder(System.Windows.Forms.SortOrder.None));
        }

        [Test]
        public void SortDesc_set_should_fire_prop_changed_for_opposite()
        {
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testTreeModel);
            var synchronizationContext = MockRepository.GenerateStub<ISynchronizationContext>();
            bool sortAscFlag = false;
            testController.PropertyChanged += delegate(object e, PropertyChangedEventArgs sender)
            {
                if (sender.PropertyName == "SortAsc")
                    sortAscFlag = true;
            };

            testController.SynchronizationContext = synchronizationContext;
            testController.SortDesc = true;

            var args = synchronizationContext.GetArgumentsForCallsMadeOn(sc => sc.Send(Arg<SendOrPostCallback>.Is.Anything,
                Arg.Is(testController)))[0];
            SendOrPostCallback cb = (SendOrPostCallback)args[0];
            cb(args[1]);
            Assert.IsTrue(sortAscFlag);
        }

        [Test]
        public void TestCount_should_return_value_from_TestModel()
        {
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            var testController = new TestController(testTreeModel);
            const int testCount = 5;
            testTreeModel.Stub(ttm => ttm.TestCount).Return(testCount);

            Assert.AreEqual(testCount, testController.TestCount);
        }
    }
}
