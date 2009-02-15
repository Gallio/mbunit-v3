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
using System.Timers;
using Gallio.Concurrency;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.Interfaces;
using Gallio.Icarus.Services.Interfaces;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using Gallio.Runner;
using Gallio.Runner.Events;
using Gallio.Runner.Reports;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Utilities;

namespace Gallio.Icarus.Controllers
{
    public class TestController : ITestController
    {
        private readonly ITestRunnerService testRunnerService;
        private readonly BindingList<TestTreeNode> selectedTests;
        private readonly ITestTreeModel testTreeModel;
        private TestPackageConfig testPackageConfig;
        private bool testPackageLoaded;
        private TestModelData testModelData;
        private readonly Timer timer = new Timer();

        public event EventHandler<TestStepFinishedEventArgs> TestStepFinished;
        public event EventHandler<ShowSourceCodeEventArgs> ShowSourceCode;
        
        public event EventHandler RunStarted;
        public event EventHandler RunFinished;
        public event EventHandler LoadStarted;
        public event EventHandler LoadFinished;
        public event EventHandler UnloadStarted;
        public event EventHandler UnloadFinished;
        
        public string TreeViewCategory
        {
            get;
            set;
        }

        public LockBox<Report> Report
        {
            get { return testRunnerService.Report; }
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
            get { return testRunnerService.TestFrameworks; }
        }

        public int TestCount
        {
            get { return testTreeModel.TestCount; }
        }

        public TestController(ITestRunnerService testRunnerService, ITestTreeModel testTreeModel)
        {
            this.testRunnerService = testRunnerService;
            this.testTreeModel = testTreeModel;

            testRunnerService.TestStepFinished += ((sender, e) =>
            {
                timer.Enabled = true;
                testTreeModel.UpdateTestStatus(e.Test, e.TestStepRun);
                EventHandlerUtils.SafeInvoke(TestStepFinished, this, e);
            });

            selectedTests = new BindingList<TestTreeNode>(new List<TestTreeNode>());

            timer.Interval = 1000;
            timer.AutoReset = false;
            timer.Elapsed += delegate { testTreeModel.Notify(); };
        }

        public void ApplyFilter(string filter, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Applying filter", 3))
            {
                progressMonitor.SetStatus("Parsing filter");
                var f = FilterUtils.ParseTestFilter(filter);
                progressMonitor.Worked(1);

                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                    testTreeModel.ApplyFilter(f, subProgressMonitor);

                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                    testRunnerService.SetFilter(f, subProgressMonitor);
            }
        }

        private bool Explore(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Exploring test package", 2))
            {
                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                    Load(subProgressMonitor);

                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                    if (testModelData == null)
                        testModelData = testRunnerService.Explore(subProgressMonitor);
             
                return testModelData != null;
            }
        }

        public Filter<ITest> GetCurrentFilter(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Getting current filter", 2))
            {
                Filter<ITest> filter;
                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                    filter = testTreeModel.GetCurrentFilter(subProgressMonitor);

                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                    testRunnerService.SetFilter(filter, subProgressMonitor);

                return filter;
            }
        }

        private void Load(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Loading test package", 100))
            {
                if (testPackageLoaded)
                    return;

                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(95))
                    testRunnerService.Load(testPackageConfig, subProgressMonitor);

                testPackageLoaded = true;
                testModelData = null;
            }
        }

        public void Reload(IProgressMonitor progressMonitor)
        {
            if (testPackageConfig != null)
                Reload(testPackageConfig, progressMonitor);
        }

        public void Reload(TestPackageConfig config, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Reloading test package", 100))
            {
                testPackageConfig = config;

                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(10))
                    Unload(subProgressMonitor);

                EventHandlerUtils.SafeInvoke(LoadStarted, this, System.EventArgs.Empty);

                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(40))
                    Explore(subProgressMonitor);

                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(10))
                    if (!testPackageConfig.HostSetup.ShadowCopy)
                        Unload(subProgressMonitor);

                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(40))
                    RefreshTestTree(subProgressMonitor);

                EventHandlerUtils.SafeInvoke(LoadFinished, this, System.EventArgs.Empty);
            }
        }

        public void RefreshTestTree(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Refreshing test tree", 100))
            {
                testRunnerService.Report.Read(report => testTreeModel.BuildTestTree(report.TestModel, 
                    TreeViewCategory, progressMonitor));
            }
        }

        private void Unload(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Unloading test package", 100))
            {
                EventHandlerUtils.SafeInvoke(UnloadStarted, this, System.EventArgs.Empty);

                if (testPackageLoaded)
                {
                    using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(95))
                        testRunnerService.Unload(subProgressMonitor);
                    testPackageLoaded = false;
                    // Note: we specifically do not null out the testModelData because
                    //       it can still be used for View Source operations later.
                }

                EventHandlerUtils.SafeInvoke(UnloadFinished, this, System.EventArgs.Empty);
            }
        }

        public void ResetTests(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Resetting tests", 100))
            {
                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(75))
                    testTreeModel.ResetTestStatus(subProgressMonitor);

                testRunnerService.Report.Write(report => report.TestPackageRun = null);
            }
        }

        public void RunTests(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Running tests", 100))
            {
                EventHandlerUtils.SafeInvoke(RunStarted, this, System.EventArgs.Empty);

                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(5))
                    testTreeModel.ResetTestStatus(subProgressMonitor);

                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(10))
                    if (Explore(subProgressMonitor))
                    {
                        using (var subSubProgressMonitor = progressMonitor.CreateSubProgressMonitor(90))
                            testRunnerService.Run(subSubProgressMonitor);

                        using (var subSubSubProgressMonitor = progressMonitor.CreateSubProgressMonitor(5))
                            if (testPackageConfig != null && !testPackageConfig.HostSetup.ShadowCopy)
                                Unload(subSubSubProgressMonitor);
                    }

                EventHandlerUtils.SafeInvoke(RunFinished, this, System.EventArgs.Empty);
            }
        }

        public void SetTestRunner(ITestRunner testRunner)
        {
            testRunnerService.TestRunner = testRunner;
            testPackageLoaded = false;            
            testRunnerService.Initialize();
        }

        public void UnloadTestPackage(IProgressMonitor progressMonitor)
        {
            Unload(progressMonitor);
        }

        public void ViewSourceCode(string testId, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("View source code", 100))
            {
                // retrieve TestData for selected item
                if (testModelData == null)
                    return;
                TestData testData = testModelData.GetTestById(testId);
                if (testData == null)
                    return;

                // cannot display source for assemblies etc.
                string componentKind = testData.Metadata.GetValue(MetadataKeys.TestKind);
                if (componentKind != TestKinds.Fixture && componentKind != TestKinds.Test)
                    return;

                // fire event for view
                EventHandlerUtils.SafeInvoke(ShowSourceCode, this,
                    new ShowSourceCodeEventArgs(testData.CodeLocation));
            }
        }
    }
}
