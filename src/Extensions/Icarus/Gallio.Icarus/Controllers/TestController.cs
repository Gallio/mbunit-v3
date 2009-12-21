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
using System.Collections.Generic;
using System.ComponentModel;
using Gallio.Common.Collections;
using Gallio.Common.Concurrency;
using Gallio.Common.Policies;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Icarus.TreeBuilders;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner;
using Gallio.Runner.Events;
using Gallio.Runner.Extensions;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.Controllers
{
    internal class TestController : NotifyController, ITestController
    {
        private readonly ITestTreeModel testTreeModel;
        private readonly IOptionsController optionsController;
        private readonly ITaskManager taskManager;
        private readonly ITestStatistics testStatistics;
        private LockBox<Report> reportLockBox;

        private ITestRunnerFactory testRunnerFactory;
        private TestPackage testPackage;
        private FilterSet<ITestDescriptor> filterSet;

        public event EventHandler<TestStepFinishedEventArgs> TestStepFinished;
        public event EventHandler RunStarted;
        public event EventHandler RunFinished;
        public event EventHandler ExploreStarted;
        public event EventHandler ExploreFinished;

        private readonly LockBox<IList<TestTreeNode>> selectedTests = new LockBox<IList<TestTreeNode>>(new List<TestTreeNode>());

        public string TreeViewCategory
        {
            get;
            set;
        }

        public LockBox<IList<TestTreeNode>> SelectedTests
        {
            get { return selectedTests; }
        }

        public bool FilterPassed
        {
            get
            {
                return testTreeModel.FilterPassed;
            }
            set
            {
                if (value)
                    testTreeModel.SetFilter(TestStatus.Passed);
                else
                    testTreeModel.RemoveFilter(TestStatus.Passed);
            }
        }

        public bool FilterFailed
        {
            get
            {
                return testTreeModel.FilterFailed;
            }
            set
            {
                if (value)
                    testTreeModel.SetFilter(TestStatus.Failed);
                else
                    testTreeModel.RemoveFilter(TestStatus.Failed);
            }
        }

        public bool FilterInconclusive
        {
            get
            {
                return testTreeModel.FilterInconclusive;
            }
            set
            {
                if (value)
                    testTreeModel.SetFilter(TestStatus.Inconclusive);
                else
                    testTreeModel.RemoveFilter(TestStatus.Inconclusive);
            }
        }

        public bool FailedTests { get; private set; }

        public TestController(ITestTreeModel testTreeModel, IOptionsController optionsController, 
            ITaskManager taskManager, ITestStatistics testStatistics)
        {
            this.testTreeModel = testTreeModel;
            this.optionsController = optionsController;
            this.taskManager = taskManager;
            this.testStatistics = testStatistics;

            testPackage = new TestPackage();
            reportLockBox = new LockBox<Report>(new Report());
        }

        public void Explore(IProgressMonitor progressMonitor, 
            IEnumerable<string> testRunnerExtensions)
        {
            using (progressMonitor.BeginTask("Exploring the tests", 100))
            {
                EventHandlerPolicy.SafeInvoke(ExploreStarted, this, System.EventArgs.Empty);

                DoWithTestRunner(testRunner =>
                {
                    var testExplorationOptions = new TestExplorationOptions();

                    testRunner.Explore(testPackage, testExplorationOptions,
                        progressMonitor.CreateSubProgressMonitor(80));

                    RefreshTestTree(progressMonitor.CreateSubProgressMonitor(10));
                }, progressMonitor, 10, testRunnerExtensions);

                EventHandlerPolicy.SafeInvoke(ExploreFinished, this, System.EventArgs.Empty);
            }
        }

        public void Run(bool debug, IProgressMonitor progressMonitor, 
            IEnumerable<string> testRunnerExtensions)
        {
            FailedTests = false;

            using (progressMonitor.BeginTask("Running the tests.", 100))
            {    
                EventHandlerPolicy.SafeInvoke(RunStarted, this, System.EventArgs.Empty);

                progressMonitor.Worked(5);

                DoWithTestRunner(testRunner =>
                {
                    var testPackageCopy = testPackage.Copy();
                    testPackageCopy.DebuggerSetup = debug ? new DebuggerSetup() : null;

                    var testExplorationOptions = new TestExplorationOptions();
                    var testExecutionOptions = new TestExecutionOptions
                    {
                        // re-use filter set generated when saving "last run" filter
                        FilterSet = filterSet
                    };

                    testRunner.Run(testPackageCopy, testExplorationOptions, testExecutionOptions,
                        progressMonitor.CreateSubProgressMonitor(85));

                }, progressMonitor, 5, testRunnerExtensions);

                EventHandlerPolicy.SafeInvoke(RunFinished, this, System.EventArgs.Empty);
            }
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

        public void ApplyFilterSet(FilterSet<ITestDescriptor> filter)
        {
            testTreeModel.ApplyFilterSet(filter);
        }

        public FilterSet<ITestDescriptor> GenerateFilterSetFromSelectedTests()
        {
            filterSet = testTreeModel.GenerateFilterSetFromSelectedTests();
            return filterSet;
        }

        public void RefreshTestTree(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Refreshing test tree.", 100))
            {
                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                    testStatistics.Reset(subProgressMonitor);

                var options = new TreeBuilderOptions
                {
                    TreeViewCategory = TreeViewCategory,
                    NamespaceHierarchy = optionsController.NamespaceHierarchy
                };

                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(99))
                    ReadReport(report => testTreeModel.BuildTestTree(subProgressMonitor, 
                        report.TestModel, options));
            }
        }

        public void ResetTestStatus(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Resetting test status.", 3))
            {
                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                    testStatistics.Reset(subProgressMonitor);

                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(2))
                    testTreeModel.ResetTestStatus(subProgressMonitor);
            }
        }

        public void SetSelection(IList<TestTreeNode> nodes)
        {
            SelectedTests.Write(sts =>
            {
                sts.Clear();
                foreach (var node in nodes)
                    sts.Add(node);
            });
            OnPropertyChanged(new PropertyChangedEventArgs("SelectedTests"));
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

            ITestRunner testRunner = testRunnerFactory.CreateTestRunner();
            try
            {
                var testRunnerOptions = new TestRunnerOptions();
                var logger = RuntimeAccessor.Logger;

                List<string> extensionSpecifications = new List<string>();
                GenericCollectionUtils.AddAllIfNotAlreadyPresent(testRunnerExtensions, extensionSpecifications);
                GenericCollectionUtils.AddAllIfNotAlreadyPresent(optionsController.TestRunnerExtensions, 
                    extensionSpecifications);

                foreach (string extensionSpecification in extensionSpecifications)
                {
                    var testRunnerExtension = TestRunnerExtensionUtils.CreateExtensionFromSpecification(extensionSpecification);
                    testRunner.RegisterExtension(testRunnerExtension);
                }

                testRunner.Initialize(testRunnerOptions, logger, 
                    progressMonitor.CreateSubProgressMonitor(initializationAndDisposalWorkUnits / 2));

                testRunner.Events.TestStepFinished += (sender, e) => taskManager.BackgroundTask(() =>
                {
                    testTreeModel.TestStepFinished(e.Test, e.TestStepRun);

                    if (e.TestStepRun.Step.IsTestCase)
                        testStatistics.TestStepFinished(e.TestStepRun.Result.Outcome.Status);

                    EventHandlerPolicy.SafeInvoke(TestStepFinished, this, e);

                    if (e.TestStepRun.Result.Outcome.Status == TestStatus.Failed)
                        FailedTests = true;
                });

                testRunner.Events.RunStarted += (s, e) =>
                {
                    reportLockBox = e.ReportLockBox;
                    testTreeModel.UpdateTestCount();
                };

                testRunner.Events.ExploreStarted += (s, e) => reportLockBox = e.ReportLockBox;
                testRunner.Events.ExploreFinished += (s, e) => testTreeModel.UpdateTestCount();

                action(testRunner);
            }
            finally
            {
                testRunner.Dispose(progressMonitor.CreateSubProgressMonitor(initializationAndDisposalWorkUnits / 2));
            }
        }
    }
}
