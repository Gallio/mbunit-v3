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
using Gallio.Common.Collections;
using Gallio.Common.Concurrency;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Events;
using Gallio.Icarus.Models;
using Gallio.Icarus.Projects;
using Gallio.Icarus.Services;
using Gallio.Icarus.TreeBuilders;
using Gallio.Model;
using Gallio.Runner;
using Gallio.Runner.Extensions;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Events;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.Controllers
{
    public class TestController : NotifyController, ITestController, Handles<TreeViewCategoryChanged>
    {
        private readonly ITestTreeModel testTreeModel;
        private readonly IOptionsController optionsController;
        private readonly ITaskManager taskManager;
        private readonly IEventAggregator eventAggregator;
        private readonly IFilterService filterService;
        private LockBox<Report> reportLockBox;
        private ITestRunnerFactory testRunnerFactory;
        private TestPackage testPackage;
        private string treeViewCategory = UserOptions.DefaultTreeViewCategory;

        private bool testsFailed;

        public TestController(ITestTreeModel testTreeModel, IOptionsController optionsController,
            ITaskManager taskManager, IEventAggregator eventAggregator,
            IFilterService filterService)
        {
            this.testTreeModel = testTreeModel;
            this.optionsController = optionsController;
            this.taskManager = taskManager;
            this.eventAggregator = eventAggregator;
            this.filterService = filterService;

            testPackage = new TestPackage();
            reportLockBox = new LockBox<Report>(new Report());
        }

        public void Explore(IProgressMonitor progressMonitor,
            IEnumerable<string> testRunnerExtensions)
        {
            using (progressMonitor.BeginTask("Exploring the tests", 100))
            {
                eventAggregator.Send(this, new ExploreStarted());

                DoWithTestRunner(testRunner =>
                {
                    var testExplorationOptions = new TestExplorationOptions();

                    testRunner.Explore(testPackage, testExplorationOptions,
                        progressMonitor.CreateSubProgressMonitor(80));

                    RefreshTestTree(progressMonitor.CreateSubProgressMonitor(10));
                }, progressMonitor, 10, testRunnerExtensions);

                eventAggregator.Send(this, new ExploreFinished());
            }
        }

        public void Run(bool debug, IProgressMonitor progressMonitor,
            IEnumerable<string> testRunnerExtensions)
        {
            using (progressMonitor.BeginTask("Running the tests.", 100))
            {
                eventAggregator.Send(this, new RunStarted());
                testsFailed = false;

                progressMonitor.Worked(5);

                DoWithTestRunner(testRunner => RunTests(debug, testRunner, progressMonitor),
                    progressMonitor, 5, testRunnerExtensions);

                eventAggregator.Send(this, new RunFinished());
            }
        }

        private void RunTests(bool debug, ITestRunner testRunner, IProgressMonitor progressMonitor)
        {
            var testPackageCopy = testPackage.Copy();
            testPackageCopy.DebuggerSetup = debug ? new DebuggerSetup() : null;

            var testExplorationOptions = new TestExplorationOptions();

            var filterSet = filterService.GenerateFilterSetFromSelectedTests();
            var testExecutionOptions = new TestExecutionOptions
            {
                FilterSet = filterSet
            };

            testRunner.Run(testPackageCopy, testExplorationOptions, testExecutionOptions,
                progressMonitor.CreateSubProgressMonitor(85));
        }

        public void SetTestPackage(TestPackage package)
        {
            if (package == null)
                throw new ArgumentNullException("package");

            testPackage = package.Copy();
        }

        public void ReadReport(ReadAction<Report> action)
        {
            reportLockBox.Read(action);
        }

        public void RefreshTestTree(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Refreshing test tree.", 100))
            {
                eventAggregator.Send(this, new TestsReset());

                var options = new TreeBuilderOptions
                {
                    TreeViewCategory = treeViewCategory,
                    NamespaceHierarchy = optionsController.NamespaceHierarchy
                };

                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(99))
                {
                    ReadReport(report => testTreeModel.BuildTestTree(subProgressMonitor,
                        report.TestModel, options));
                }
            }
        }

        public void ResetTestStatus(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Resetting test status.", 5))
            {
                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(4))
                    testTreeModel.ResetTestStatus(subProgressMonitor);

                eventAggregator.Send(this, new TestsReset());
            }
        }

        public void SetTestRunnerFactory(ITestRunnerFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            testRunnerFactory = factory;
        }

        private void DoWithTestRunner(Action<ITestRunner> action, IProgressMonitor progressMonitor,
            double initializationAndDisposalWorkUnits, IEnumerable<string> testRunnerExtensions)
        {
            if (testRunnerFactory == null)
                throw new InvalidOperationException("A test runner factory must be set first.");

            var testRunner = testRunnerFactory.CreateTestRunner();

            try
            {
                var testRunnerOptions = new TestRunnerOptions();
                var logger = RuntimeAccessor.Logger;

                RegisterTestRunnerExtensions(testRunnerExtensions, testRunner);

                var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(initializationAndDisposalWorkUnits / 2);
                testRunner.Initialize(testRunnerOptions, logger, subProgressMonitor);

                WireUpTestRunnerEvents(testRunner);

                action(testRunner);
            }
            finally
            {
                var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(initializationAndDisposalWorkUnits / 2);
                testRunner.Dispose(subProgressMonitor);
            }
        }

        private void RegisterTestRunnerExtensions(IEnumerable<string> testRunnerExtensions,
            ITestRunner testRunner)
        {
            var extensionSpecifications = new List<string>();

            GenericCollectionUtils.AddAllIfNotAlreadyPresent(testRunnerExtensions,
                extensionSpecifications);
            GenericCollectionUtils.AddAllIfNotAlreadyPresent(optionsController.TestRunnerExtensions,
                extensionSpecifications);

            foreach (var extensionSpecification in extensionSpecifications)
            {
                var testRunnerExtension = TestRunnerExtensionUtils.CreateExtensionFromSpecification(extensionSpecification);
                testRunner.RegisterExtension(testRunnerExtension);
            }
        }

        private void WireUpTestRunnerEvents(ITestRunner testRunner)
        {
            WireUpTestStepFinished(testRunner);

            testRunner.Events.RunStarted += (s, e) =>
            {
                reportLockBox = e.ReportLockBox;
                testTreeModel.UpdateTestCount();
            };

            testRunner.Events.ExploreStarted += (s, e) => reportLockBox = e.ReportLockBox;
            testRunner.Events.ExploreFinished += (s, e) => testTreeModel.UpdateTestCount();
        }

        private void WireUpTestStepFinished(ITestRunner testRunner)
        {
            testRunner.Events.TestStepFinished += (sender, e) =>
                taskManager.BackgroundTask(() =>
                {
                    eventAggregator.Send(this, new TestStepFinished(e.Test,
                                                                    e.TestStepRun));

                    if (false == ShouldSendTestsFailedEvent(e.TestStepRun.Result.Outcome.Status)) 
                        return;

                    testsFailed = true;
                    eventAggregator.Send(this, new TestsFailed());
                });
        }

        private bool ShouldSendTestsFailedEvent(TestStatus testStatus)
        {
            return testStatus == TestStatus.Failed && false == testsFailed;
        }

        public void Handle(TreeViewCategoryChanged @event)
        {
            treeViewCategory = @event.TreeViewCategory;
        }
    }
}
