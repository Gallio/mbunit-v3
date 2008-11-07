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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Gallio.Concurrency;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.Interfaces;
using Gallio.Icarus.Services.Interfaces;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
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
        private string treeViewCategory = string.Empty;
        private TestPackageConfig testPackageConfig;
        private bool testPackageLoaded;
        private TestModelData testModelData;

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
            get { return treeViewCategory; }
            set { treeViewCategory = value; }
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

            testRunnerService.TestStepFinished += delegate(object sender, TestStepFinishedEventArgs e)
            {
                testTreeModel.UpdateTestStatus(e.Test, e.TestStepRun);
                EventHandlerUtils.SafeInvoke(TestStepFinished, this, e);
            };

            selectedTests = new BindingList<TestTreeNode>(new List<TestTreeNode>());
        }

        public void ApplyFilter(string filter, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Applying filter", 3))
            {
                progressMonitor.SetStatus("Parsing filter");
                Filter<ITest> f = FilterUtils.ParseTestFilter(filter);
                progressMonitor.Worked(1);

                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                    testTreeModel.ApplyFilter(f, subProgressMonitor);

                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                    testRunnerService.SetFilter(f, subProgressMonitor);
            }
        }

        private bool Explore(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Exploring test package", 2))
            {
                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                    Load(subProgressMonitor);

                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
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
                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                    filter = testTreeModel.GetCurrentFilter(subProgressMonitor);

                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                    testRunnerService.SetFilter(filter, subProgressMonitor);

                return filter;
            }
        }

        private void Load(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Loading test package", 1))
            {
                if (testPackageLoaded)
                    return;

                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
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
            using (progressMonitor.BeginTask("Reloading test package", 4))
            {
                testPackageConfig = config;
                EventHandlerUtils.SafeInvoke(LoadStarted, this, System.EventArgs.Empty);

                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                    Unload(subProgressMonitor);

                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                    Explore(subProgressMonitor);

                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                    if (!testPackageConfig.HostSetup.ShadowCopy)
                        Unload(subProgressMonitor);

                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                    testRunnerService.Report.Read(report => testTreeModel.BuildTestTree(
                        report.TestModel, treeViewCategory, subProgressMonitor));

                EventHandlerUtils.SafeInvoke(LoadFinished, this, System.EventArgs.Empty);
            }
        }

        private void Unload(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Unloading test package", 1))
            {
                EventHandlerUtils.SafeInvoke(UnloadStarted, this, System.EventArgs.Empty);

                if (testPackageLoaded)
                {
                    using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
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
            using (progressMonitor.BeginTask("Resetting tests", 2))
            {
                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                    testTreeModel.ResetTestStatus(subProgressMonitor);

                testRunnerService.Report.Write(report => report.TestPackageRun = null);
            }
        }

        public void RunTests(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Running tests", 100))
            {
                EventHandlerUtils.SafeInvoke(RunStarted, this, System.EventArgs.Empty);

                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(5))
                    testTreeModel.ResetTestStatus(subProgressMonitor);

                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(10))
                    if (Explore(subProgressMonitor))
                        using (IProgressMonitor subSubProgressMonitor = progressMonitor.CreateSubProgressMonitor(90))
                            testRunnerService.Run(subSubProgressMonitor);

                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(5))
                    if (!testPackageConfig.HostSetup.ShadowCopy)
                        Unload(subProgressMonitor);

                EventHandlerUtils.SafeInvoke(RunFinished, this, System.EventArgs.Empty);
            }
        }

        public void UnloadTestPackage(IProgressMonitor progressMonitor)
        {
            Unload(progressMonitor);
        }

        public void ViewSourceCode(string testId, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("View source code", 1))
            {
                if (testModelData == null)
                    return;

                TestData testData = testModelData.GetTestById(testId);

                if (testData != null)
                    EventHandlerUtils.SafeInvoke(ShowSourceCode, this,
                        new ShowSourceCodeEventArgs(testData.CodeLocation));
            }
        }
    }
}
