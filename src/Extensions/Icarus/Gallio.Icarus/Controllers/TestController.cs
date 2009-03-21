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
using System.Threading;
using System.Windows.Forms;
using Gallio.Concurrency;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.Interfaces;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using Gallio.Reflection;
using Gallio.Runner;
using Gallio.Runner.Events;
using Gallio.Runner.Reports;
using Gallio.Runtime;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Utilities;
using Timer=System.Timers.Timer;
using ITestController=Gallio.Icarus.Controllers.Interfaces.ITestController;

namespace Gallio.Icarus.Controllers
{
    public class TestController : ITestController, INotifyPropertyChanged
    {
        private readonly BindingList<TestTreeNode> selectedTests;
        private readonly ITestTreeModel testTreeModel;
        private readonly Timer testTreeUpdateTimer = new Timer();
        private LockBox<Report> reportLockBox;

        private ITestRunnerFactory testRunnerFactory;
        private TestPackageConfig testPackageConfig;
        private SynchronizationContext synchronizationContext;

        public TestController(ITestTreeModel testTreeModel)
        {
            this.testTreeModel = testTreeModel;

            selectedTests = new BindingList<TestTreeNode>(new List<TestTreeNode>());

            testTreeUpdateTimer.Interval = 1000;
            testTreeUpdateTimer.AutoReset = false;
            testTreeUpdateTimer.Elapsed += delegate { testTreeModel.Notify(); };

            testPackageConfig = new TestPackageConfig();
            reportLockBox = new LockBox<Report>(new Report());
        }

        public void Dispose()
        {
            testTreeUpdateTimer.Dispose();
        }

        public event EventHandler<TestStepFinishedEventArgs> TestStepFinished;
        public event EventHandler<ShowSourceCodeEventArgs> ShowSourceCode;
        public event EventHandler RunStarted;
        public event EventHandler RunFinished;
        public event EventHandler ExploreStarted;
        public event EventHandler ExploreFinished;

        public string TreeViewCategory
        {
            get;
            set;
        }

        public BindingList<TestTreeNode> SelectedTests
        {
            get { return selectedTests; }
        }

        public ITestTreeModel Model
        {
            get { return testTreeModel; }
        }

        public IList<string> TestFrameworks
        {
            get
            {
                List<string> frameworks = new List<string>();
                foreach (ITestFramework framework in RuntimeAccessor.Instance.ResolveAll<ITestFramework>())
                    frameworks.Add(framework.Name);
                return frameworks;
            }
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
                testTreeModel.SetFilter(TestStatus.Skipped);
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

        public SynchronizationContext SynchronizationContext
        {
            get { return synchronizationContext; }
            set
            {
                synchronizationContext = value;
                testTreeModel.SynchronizationContext = value;
            }
        }

        public bool FailedTests { get; private set; }

        public void Explore(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Exploring the tests.", 100))
            {
                EventHandlerUtils.SafeInvoke(ExploreStarted, this, System.EventArgs.Empty);

                DoWithTestRunner(testRunner =>
                {
                    var testExplorationOptions = new TestExplorationOptions();

                    testRunner.Explore(testPackageConfig, testExplorationOptions,
                        progressMonitor.CreateSubProgressMonitor(85));

                    RefreshTestTree(progressMonitor.CreateSubProgressMonitor(5));
                }, progressMonitor, 10);

                EventHandlerUtils.SafeInvoke(ExploreFinished, this, System.EventArgs.Empty);
            }
        }
        
        public void Run(bool debug, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Running the tests.", 100))
            {
                EventHandlerUtils.SafeInvoke(RunStarted, this, System.EventArgs.Empty);
                FailedTests = false;

                DoWithTestRunner(testRunner =>
                {
                    var testPackageConfigCopy = testPackageConfig.Copy();
                    testPackageConfigCopy.HostSetup.Debug = debug;

                    var testExplorationOptions = new TestExplorationOptions();
                    var testExecutionOptions = new TestExecutionOptions
                                                   {
                        Filter = GenerateFilterFromSelectedTests(),
                        ExactFilter = true
                    };

                    testRunner.Run(testPackageConfigCopy, testExplorationOptions, testExecutionOptions,
                        progressMonitor.CreateSubProgressMonitor(85));

                }, progressMonitor, 10);

                EventHandlerUtils.SafeInvoke(RunFinished, this, System.EventArgs.Empty);
            }
        }

        public void SetTestPackageConfig(TestPackageConfig testPackageConfig)
        {
            if (testPackageConfig == null)
                throw new ArgumentNullException("testPackageConfig");

            this.testPackageConfig = testPackageConfig.Copy();
        }

        public void ReadReport(ReadAction<Report> action)
        {
            reportLockBox.Read(action);
        }

        public void ApplyFilter(Filter<ITest> filter)
        {
            testTreeModel.ApplyFilter(filter);
        }

        public Filter<ITest> GenerateFilterFromSelectedTests()
        {
            return testTreeModel.GenerateFilterFromSelectedTests();
        }

        public void RefreshTestTree(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Refreshing test tree", 100))
            {
                ReadReport(report => testTreeModel.BuildTestTree(report.TestModel, TreeViewCategory));
            }
        }

        public void ResetTestStatus(IProgressMonitor progressMonitor)
        {
            testTreeModel.ResetTestStatus(progressMonitor);
        }

        public void SetTestRunnerFactory(ITestRunnerFactory testRunnerFactory)
        {
            if (testRunnerFactory == null)
                throw new ArgumentNullException("testRunnerFactory");

            this.testRunnerFactory = testRunnerFactory;
        }

        public void ViewSourceCode(string testId, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("View source code", 100))
            {
                CodeLocation codeLocation = CodeLocation.Unknown;
                ReadReport(report =>
                {
                    if (report.TestModel != null)
                    {
                        TestData testData = report.TestModel.GetTestById(testId);
                        if (testData != null)
                            codeLocation = testData.CodeLocation;
                    }
                });

                if (codeLocation == CodeLocation.Unknown
                    || codeLocation.Path.EndsWith(".dll")
                    || codeLocation.Path.EndsWith(".exe"))
                    return;

                // fire event for view
                EventHandlerUtils.SafeInvoke(ShowSourceCode, this, new ShowSourceCodeEventArgs(codeLocation));
            }
        }

        private void DoWithTestRunner(Action<ITestRunner> action, IProgressMonitor progressMonitor,
            double initializationAndDisposalWorkUnits)
        {
            if (testRunnerFactory == null)
                throw new InvalidOperationException("A test runner factory must be set first.");

            ITestRunner testRunner = testRunnerFactory.CreateTestRunner();
            try
            {
                var testRunnerOptions = new TestRunnerOptions();
                var logger = RuntimeAccessor.Logger;

                testRunner.Initialize(testRunnerOptions, logger, progressMonitor.CreateSubProgressMonitor(initializationAndDisposalWorkUnits / 2));

                testRunner.Events.TestStepFinished += (sender, e) =>
                {
                    testTreeUpdateTimer.Enabled = true;
                    testTreeModel.UpdateTestStatus(e.Test, e.TestStepRun);
                    EventHandlerUtils.SafeInvoke(TestStepFinished, this, e);

                    if (e.TestStepRun.Result.Outcome.Status == TestStatus.Failed)
                        FailedTests = true;
                };

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (SynchronizationContext == null)
                return;

            SynchronizationContext.Post(delegate
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, e);
            }, null);
        }
    }
}
