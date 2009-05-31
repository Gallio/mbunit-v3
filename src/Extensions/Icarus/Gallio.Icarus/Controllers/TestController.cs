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
using System.Windows.Forms;
using Gallio.Common.Collections;
using Gallio.Common.Concurrency;
using Gallio.Common.Policies;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Helpers;
using Gallio.Icarus.Models;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Filters;
using Gallio.Runner;
using Gallio.Runner.Events;
using Gallio.Runner.Extensions;
using Gallio.Runner.Reports;
using Gallio.Runtime;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Progress;

namespace Gallio.Icarus.Controllers
{
    public class TestController : NotifyController, Interfaces.ITestController
    {
        private readonly ITestTreeModel testTreeModel;
        private readonly IOptionsController optionsController;
        private readonly ITaskManager taskManager;
        private LockBox<Report> reportLockBox;

        private ITestRunnerFactory testRunnerFactory;
        private TestPackageConfig testPackageConfig;
        private FilterSet<ITest> filterSet;

        public event EventHandler<TestStepFinishedEventArgs> TestStepFinished;
        public event EventHandler RunStarted;
        public event EventHandler RunFinished;
        public event EventHandler ExploreStarted;
        public event EventHandler ExploreFinished;

        private readonly List<TestTreeNode> selectedTests = new List<TestTreeNode>();

        public string TreeViewCategory
        {
            get;
            set;
        }

        public IList<TestTreeNode> SelectedTests
        {
            get { return selectedTests; }
        }

        public ITestTreeModel Model
        {
            get { return testTreeModel; }
        }

        public int TestCount
        {
            get { return testTreeModel.TestCount; }
        }

        public bool FilterPassed
        {
            get
            {
                return testTreeModel.FilterPassed;
            }
            // ReSharper disable ValueParameterNotUsed
            set
            // ReSharper restore ValueParameterNotUsed
            {
                testTreeModel.SetFilter(TestStatus.Passed);
            }
        }

        public bool FilterFailed
        {
            get
            {
                return testTreeModel.FilterFailed;
            }
            // ReSharper disable ValueParameterNotUsed
            set
            // ReSharper restore ValueParameterNotUsed
            {
                testTreeModel.SetFilter(TestStatus.Failed);
            }
        }

        public bool FilterInconclusive
        {
            get
            {
                return testTreeModel.FilterInconclusive;
            }
            // ReSharper disable ValueParameterNotUsed
            set
            // ReSharper restore ValueParameterNotUsed
            {
                testTreeModel.SetFilter(TestStatus.Inconclusive);
            }
        }

        public bool SortAsc
        {
            get
            {
                return testTreeModel.SortAsc;
            }
            set
            {
                if (value)
                    testTreeModel.SetSortOrder(SortOrder.Ascending);
                else
                    testTreeModel.SetSortOrder(SortOrder.None);
                OnPropertyChanged(new PropertyChangedEventArgs("SortDesc"));
            }
        }

        public bool SortDesc
        {
            get
            {
                return testTreeModel.SortDesc;
            }
            set
            {
                if (value)
                    testTreeModel.SetSortOrder(SortOrder.Descending);
                else
                    testTreeModel.SetSortOrder(SortOrder.None);
                OnPropertyChanged(new PropertyChangedEventArgs("SortAsc"));
            }
        }

        public int Passed
        {
            get { return Model.Passed; }
        }

        public int Failed
        {
            get { return Model.Failed; }
        }

        public int Skipped
        {
            get { return Model.Skipped; }
        }

        public int Inconclusive
        {
            get { return Model.Inconclusive; }
        }

        public bool FailedTests { get; private set; }

        public TestController(ITestTreeModel testTreeModel, IOptionsController optionsController, 
            ITaskManager taskManager)
        {
            this.testTreeModel = testTreeModel;
            this.optionsController = optionsController;
            this.taskManager = taskManager;

            testPackageConfig = new TestPackageConfig();
            reportLockBox = new LockBox<Report>(new Report());

            testTreeModel.PropertyChanged += (sender, e) => OnPropertyChanged(e);
        }

        public void Explore(IProgressMonitor progressMonitor, 
            IEnumerable<string> testRunnerExtensions)
        {
            using (progressMonitor.BeginTask("Exploring the tests.", 100))
            {
                EventHandlerPolicy.SafeInvoke(ExploreStarted, this, System.EventArgs.Empty);

                DoWithTestRunner(testRunner =>
                {
                    var testExplorationOptions = new TestExplorationOptions();

                    testRunner.Explore(testPackageConfig, testExplorationOptions,
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
                    var testPackageConfigCopy = testPackageConfig.Copy();
                    testPackageConfigCopy.HostSetup.Debug = debug;

                    var testExplorationOptions = new TestExplorationOptions();
                    var testExecutionOptions = new TestExecutionOptions
                    {
                        // re-use filter set generated when saving "last run" filter
                        FilterSet = filterSet
                    };

                    testRunner.Run(testPackageConfigCopy, testExplorationOptions, testExecutionOptions,
                        progressMonitor.CreateSubProgressMonitor(85));

                }, progressMonitor, 5, testRunnerExtensions);

                EventHandlerPolicy.SafeInvoke(RunFinished, this, System.EventArgs.Empty);
            }
        }

        public void SetTestPackageConfig(TestPackageConfig config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            testPackageConfig = config.Copy();
        }

        public void ReadReport(ReadAction<Report> action)
        {
            reportLockBox.Read(action);
        }

        public void ApplyFilterSet(FilterSet<ITest> filter)
        {
            testTreeModel.ApplyFilterSet(filter);
        }

        public FilterSet<ITest> GenerateFilterSetFromSelectedTests()
        {
            filterSet = testTreeModel.GenerateFilterSetFromSelectedTests();
            return filterSet;
        }

        public void RefreshTestTree(IProgressMonitor progressMonitor)
        {
            var options = new TestTreeBuilderOptions 
            { 
                TreeViewCategory = TreeViewCategory,
                SplitNamespaces = optionsController.TestTreeSplitNamespaces
            };
            ReadReport(report => testTreeModel.BuildTestTree(progressMonitor, 
                report.TestModel, options));
        }

        public void ResetTestStatus(IProgressMonitor progressMonitor)
        {
            testTreeModel.ResetTestStatus(progressMonitor);
        }

        public void SetSelection(IList<TestTreeNode> nodes)
        {
            selectedTests.Clear();
            selectedTests.AddRange(nodes);
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
                    testTreeModel.UpdateTestStatus(e.Test, e.TestStepRun);

                    EventHandlerPolicy.SafeInvoke(TestStepFinished, this, e);

                    if (e.TestStepRun.Result.Outcome.Status == TestStatus.Failed)
                        FailedTests = true;
                });

                testRunner.Events.RunStarted += (sender, e) =>
                {
                    reportLockBox = e.ReportLockBox;
                };

                testRunner.Events.ExploreStarted += (sender, e) =>
                {
                    reportLockBox = e.ReportLockBox;
                };

                action(testRunner);
            }
            finally
            {
                testRunner.Dispose(progressMonitor.CreateSubProgressMonitor(initializationAndDisposalWorkUnits / 2));
            }
        }
    }
}
