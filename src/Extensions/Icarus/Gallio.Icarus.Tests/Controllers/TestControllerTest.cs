// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Gallio.Common.Concurrency;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Events;
using Gallio.Icarus.Models;
using Gallio.Icarus.Services;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Icarus.TreeBuilders;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Schema;
using Gallio.Runner;
using Gallio.Runner.Events;
using Gallio.Runner.Extensions;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime.Logging;
using Gallio.UI.Events;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Controllers
{
    [MbUnit.Framework.Category("Controllers"), Author("Graham Hay"), TestsOn(typeof(TestController))]
    public class TestControllerTest
    {
        private TestController testController;
        private ITestTreeModel testTreeModel;
        private IOptionsController optionsController;
        private IEventAggregator eventAggregator;
        private ITestRunnerEvents testRunnerEvents;
        private ITestRunner testRunner;
        private IFilterService filterService;

        [SetUp]
        public void EstablishContext()
        {
            testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            optionsController = MockRepository.GenerateStub<IOptionsController>();
            eventAggregator = MockRepository.GenerateStub<IEventAggregator>();
            filterService = MockRepository.GenerateStub<IFilterService>();
            testController = new TestController(testTreeModel, optionsController, 
                new TestTaskManager(), eventAggregator, filterService);
        }

        [Test]
        public void Explore_should_send_an_ExploreStarted_event()
        {
            var progressMonitor = MockProgressMonitor.Instance;
            optionsController.Stub(oc => oc.TestRunnerExtensions).Return(new BindingList<string>(new List<string>()));
            StubTestRunnerFactory();

            testController.Explore(progressMonitor, new List<string>());

            eventAggregator.AssertWasCalled(ea => ea.Send(Arg.Is(testController), Arg<ExploreStarted>.Is.Anything));
        }

        private void StubTestRunnerFactory()
        {
            var testRunnerFactory = MockRepository.GenerateStub<ITestRunnerFactory>();
            testRunner = MockRepository.GenerateStub<ITestRunner>();
            testRunnerEvents = MockRepository.GenerateStub<ITestRunnerEvents>();
            testRunner.Stub(tr => tr.Events).Return(testRunnerEvents);
            testRunnerFactory.Stub(trf => trf.CreateTestRunner()).Return(testRunner);
            testController.SetTestRunnerFactory(testRunnerFactory);
        }

        // TODO: split this into multiple tests
        [Test]
        public void Explore_Test()
        {
            var progressMonitor = MockProgressMonitor.Instance;
            optionsController.Stub(oc => oc.TestRunnerExtensions).Return(new BindingList<string>(new List<string>()));
            StubTestRunnerFactory();
            testController.Explore(progressMonitor, new List<string>());
            
            testRunner.AssertWasCalled(tr => tr.Initialize(Arg<TestRunnerOptions>.Is.Anything, 
                Arg<ILogger>.Is.Anything, Arg.Is(progressMonitor)));
            testRunner.AssertWasCalled(tr => tr.Explore(Arg<TestPackage>.Is.Anything, 
                Arg<TestExplorationOptions>.Is.Anything, Arg.Is(progressMonitor)));
            testTreeModel.AssertWasCalled(ttm => ttm.BuildTestTree(Arg.Is(progressMonitor), Arg<TestModelData>.Is.Anything,
                Arg<TreeBuilderOptions>.Is.Anything));
        }

        [Test]
        public void Explore_should_send_an_ExploreFinished_event()
        {
            var progressMonitor = MockProgressMonitor.Instance;
            optionsController.Stub(oc => oc.TestRunnerExtensions).Return(new BindingList<string>(new List<string>()));
            StubTestRunnerFactory();

            testController.Explore(progressMonitor, new List<string>());

            eventAggregator.AssertWasCalled(ea => ea.Send(Arg.Is(testController), Arg<ExploreFinished>.Is.Anything));
        }

        [Test]
        public void TestStepFinished_should_bubble_up_from_test_runner()
        {
            var progressMonitor = MockProgressMonitor.Instance;
            var testRunnerExtensions = new BindingList<string>(new List<string>());
            optionsController.Stub(oc => oc.TestRunnerExtensions).Return(testRunnerExtensions);
            StubTestRunnerFactory();
            testController.Explore(progressMonitor, new List<string>());
            var testData = new TestData("id", "name", "fullName");
            var testStepData = new TestStepData("id", "name", "fullName", "testId")
            {
                IsTestCase = true
            };
            var testStepRun = new TestStepRun(testStepData);
            var testStepFinishedEventArgs = new TestStepFinishedEventArgs(new Report(), 
                testData, testStepRun);

            testRunnerEvents.Raise(tre => tre.TestStepFinished += null, testRunner, 
                testStepFinishedEventArgs);

            eventAggregator.AssertWasCalled(ea => ea.Send(Arg.Is(testController), Arg<TestStepFinished>.Matches(tsf => 
                                                                                              tsf.TestData == testData && tsf.TestStepRun == testStepRun)));
        }

        [Test]
        public void If_any_tests_have_failed_an_event_should_be_raised()
        {
            var progressMonitor = MockProgressMonitor.Instance;
            optionsController.Stub(oc => oc.TestRunnerExtensions).Return(new BindingList<string>(new List<string>()));
            StubTestRunnerFactory();
            testController.Explore(progressMonitor, new List<string>());
            var testStepRun = new TestStepRun(new TestStepData("id", "name", "fullName", "testId"))
            {
                Result = new TestResult(TestOutcome.Failed)
            };
            var report = new Report();
            var testData = new TestData("id", "name", "fullName");
            var testStepFinishedEventArgs = new TestStepFinishedEventArgs(report, testData, testStepRun);

            testRunnerEvents.Raise(tre => tre.TestStepFinished += null, testRunner, testStepFinishedEventArgs);

            eventAggregator.AssertWasCalled(ea => ea.Send(Arg.Is(testController), Arg<TestsFailed>.Is.Anything));
        }

        [Test]
        public void RunStarted_Test()
        {
            var progressMonitor = MockProgressMonitor.Instance;
            optionsController.Stub(oc => oc.TestRunnerExtensions).Return(new BindingList<string>(new List<string>()));
            StubTestRunnerFactory();
            testController.Explore(progressMonitor, new List<string>());
            Report report = new Report();

            testRunnerEvents.Raise(tre => tre.RunStarted += null, testRunner, new RunStartedEventArgs(new TestPackage(), 
                new TestExplorationOptions(), new TestExecutionOptions(), new LockBox<Report>(report)));

            testController.ReadReport(r => Assert.AreEqual(r, report));
        }

        [Test]
        public void ExploreStarted_Test()
        {
            var progressMonitor = MockProgressMonitor.Instance;
            optionsController.Stub(oc => oc.TestRunnerExtensions).Return(new BindingList<string>(new List<string>()));
            StubTestRunnerFactory();
            testController.Explore(progressMonitor, new List<string>());
            Report report = new Report();

            testRunnerEvents.Raise(tre => tre.ExploreStarted += null, testRunner, 
                new ExploreStartedEventArgs(new TestPackage(), new TestExplorationOptions(), 
                new LockBox<Report>(report)));

            testController.ReadReport(r => Assert.AreEqual(r, report));
        }

        [Test]
        public void DoWithTestRunner_should_throw_if_testRunnerFactory_is_null()
        {
            var progressMonitor = MockProgressMonitor.Instance;
            optionsController.Stub(oc => oc.TestRunnerExtensions).Return(new BindingList<string>(new List<string>()));

            Assert.Throws<InvalidOperationException>(() => testController.Explore(progressMonitor, new List<string>()));
        }

        [Test]
        public void ResetTestStatus_Test()
        {
            var progressMonitor = MockProgressMonitor.Instance;

            testController.ResetTestStatus(progressMonitor);

            testTreeModel.AssertWasCalled(ttm => ttm.ResetTestStatus(progressMonitor));
        }

        [Test]
        public void RunStarted_is_fired_when_running_tests()
        {
            var progressMonitor = MockProgressMonitor.Instance;
            var filter = new FilterSet<ITestDescriptor>(new NoneFilter<ITestDescriptor>());
            filterService.Stub(fs => fs.GenerateFilterSetFromSelectedTests()).Return(filter);
            var testRunnerExtensions = new BindingList<string>(new List<string>());
            optionsController.Stub(oc => oc.TestRunnerExtensions).Return(testRunnerExtensions);
            StubTestRunnerFactory();

            testController.Run(false, progressMonitor, new List<string>());

            eventAggregator.AssertWasCalled(ea => ea.Send(Arg.Is(testController), Arg<RunStarted>.Is.Anything));
        }

        [Test]
        public void Run_Test()
        {
            var progressMonitor = MockProgressMonitor.Instance;
            var filter = new FilterSet<ITestDescriptor>(new NoneFilter<ITestDescriptor>());
            filterService.Stub(fs => fs.GenerateFilterSetFromSelectedTests()).Return(filter);
            optionsController.Stub(oc => oc.TestRunnerExtensions).Return(new BindingList<string>(new List<string>()));
            StubTestRunnerFactory();
            var testPackage = new TestPackage();
            testPackage.AddFile(new FileInfo("test"));

            testController.SetTestPackage(testPackage);
            testController.Run(false, progressMonitor, new List<string>());

            testRunner.AssertWasCalled(tr => tr.Run(Arg<TestPackage>.Matches(tpc => tpc.Files.Count == 1), 
                Arg<TestExplorationOptions>.Is.Anything, 
                Arg<TestExecutionOptions>.Matches(teo => ((teo.FilterSet == filter) && !teo.ExactFilter)), 
                Arg.Is(progressMonitor)));
        }

        [Test]
        public void RunFinished_is_fired_when_running_tests()
        {
            var progressMonitor = MockProgressMonitor.Instance;
            var filter = new FilterSet<ITestDescriptor>(new NoneFilter<ITestDescriptor>());
            filterService.Stub(fs => fs.GenerateFilterSetFromSelectedTests()).Return(filter);
            optionsController.Stub(oc => oc.TestRunnerExtensions).Return(new BindingList<string>(new List<string>()));
            StubTestRunnerFactory();

            testController.Run(false, progressMonitor, new List<string>());

            eventAggregator.AssertWasCalled(ea => ea.Send(Arg.Is(testController), Arg<RunFinished>.Is.Anything));
        }

        [Test]
        public void TestRunnerExtensions_are_found_from_OptionsController()
        {
            var progressMonitor = MockProgressMonitor.Instance;
            var filter = new FilterSet<ITestDescriptor>(new NoneFilter<ITestDescriptor>());
            filterService.Stub(ttm => ttm.GenerateFilterSetFromSelectedTests()).Return(filter);
            var testRunnerExtensions = new BindingList<string>(new List<string>(new[] {"DebugExtension, Gallio"}));
            optionsController.Stub(oc => oc.TestRunnerExtensions).Return(testRunnerExtensions);
            StubTestRunnerFactory();

            testController.Run(false, progressMonitor, new List<string>());

            testRunner.AssertWasCalled(tr => tr.RegisterExtension(Arg<DebugExtension>.Is.Anything));
        }

        [Test]
        public void TestRunnerExtensions_are_found_from_Project()
        {
            var progressMonitor = MockProgressMonitor.Instance;
            var filter = new FilterSet<ITestDescriptor>(new NoneFilter<ITestDescriptor>());
            filterService.Stub(ttm => ttm.GenerateFilterSetFromSelectedTests()).Return(filter);
            var testRunnerExtensions = new BindingList<string>(new List<string>());
            optionsController.Stub(oc => oc.TestRunnerExtensions).Return(testRunnerExtensions);
            StubTestRunnerFactory();

            testController.Run(false, progressMonitor, new List<string>(new[] { "DebugExtension, Gallio" }));

            testRunner.AssertWasCalled(tr => tr.RegisterExtension(Arg<DebugExtension>.Is.Anything));
        }

        [Test]
        public void Duplicate_TestRunnerExtensions_are_only_added_once()
        {
            var progressMonitor = MockProgressMonitor.Instance;
            var filter = new FilterSet<ITestDescriptor>(new NoneFilter<ITestDescriptor>());
            filterService.Stub(ttm => ttm.GenerateFilterSetFromSelectedTests()).Return(filter);
            var testRunnerExtensions = new BindingList<string>(new List<string>());
            optionsController.Stub(oc => oc.TestRunnerExtensions).Return(testRunnerExtensions);
            StubTestRunnerFactory();

            testController.Run(false, progressMonitor, new List<string>(new[] { "DebugExtension, Gallio", 
                "DebugExtension, Gallio" }));

            testRunner.AssertWasCalled(tr => tr.RegisterExtension(Arg<DebugExtension>.Is.Anything));
        }

        [Test]
        public void SetTestPackage_should_throw_if_test_package_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => testController.SetTestPackage(null));
        }

        [Test]
        public void SetTestRunnerFactory_should_throw_if_test_factory_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => testController.SetTestRunnerFactory(null));
        }
    }
}
