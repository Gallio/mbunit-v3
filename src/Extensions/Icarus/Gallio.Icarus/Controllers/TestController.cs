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
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.Interfaces;
using Gallio.Icarus.ProgressMonitoring.EventArgs;
using Gallio.Icarus.Services.Interfaces;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using Gallio.Runner.Events;
using Gallio.Runner.Reports;
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
        private readonly TaskManager taskManager = new TaskManager();

        public event EventHandler<TestStepFinishedEventArgs> TestStepFinished;
        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdate;
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

        public Report Report
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

            testRunnerService.ProgressUpdate += delegate(object sender, ProgressUpdateEventArgs e)
            {
                EventHandlerUtils.SafeInvoke(ProgressUpdate, this, e);
            };

            selectedTests = new BindingList<TestTreeNode>(new List<TestTreeNode>());
        }

        public void ApplyFilter(string filter)
        {
            Filter<ITest> f = FilterUtils.ParseTestFilter(filter);
            testTreeModel.ApplyFilter(f);
            testRunnerService.SetFilter(f);
        }

        public void Cancel()
        {
            testRunnerService.Cancel();
        }

        private TestModelData Explore()
        {
            Load();

            if (testModelData == null)
                testModelData = testRunnerService.Explore();

            return testModelData;
        }

        public Filter<ITest> GetCurrentFilter()
        {
            Filter<ITest> filter = testTreeModel.GetCurrentFilter();
            testRunnerService.SetFilter(filter);
            return filter;
        }

        private void Load()
        {
            if (testPackageLoaded)
                return;
            testRunnerService.Load(testPackageConfig);

            testPackageLoaded = true;
            testModelData = null;
        }

        public void Reload()
        {
            if (testPackageConfig != null)
                Reload(testPackageConfig);
        }

        public void Reload(TestPackageConfig config)
        {
            testPackageConfig = config;
            taskManager.StartTask(delegate
            {
                EventHandlerUtils.SafeInvoke(LoadStarted, this, System.EventArgs.Empty);

                Unload();
                testModelData = Explore();
                if (!testPackageConfig.HostSetup.ShadowCopy)
                    Unload();

                testTreeModel.BuildTestTree(testRunnerService.Report.TestModel, treeViewCategory);
                
                EventHandlerUtils.SafeInvoke(LoadFinished, this, System.EventArgs.Empty);
            });
        }
        
        private void Unload()
        {
            EventHandlerUtils.SafeInvoke(UnloadStarted, this, System.EventArgs.Empty);

            if (testPackageLoaded)
            {
                testRunnerService.Unload();
                testPackageLoaded = false;
                testModelData = null;
            }

            EventHandlerUtils.SafeInvoke(UnloadFinished, this, System.EventArgs.Empty);
        }

        public void ResetTests()
        {
            testTreeModel.ResetTestStatus();
        }

        public void RunTests()
        {
            taskManager.StartTask(delegate
            {
                EventHandlerUtils.SafeInvoke(RunStarted, this, System.EventArgs.Empty);

                testTreeModel.ResetTestStatus();

                if (Explore() != null)
                    testRunnerService.Run();

                if (!testPackageConfig.HostSetup.ShadowCopy)
                    Unload();

                EventHandlerUtils.SafeInvoke(RunFinished, this, System.EventArgs.Empty);
            });
        }

        public void UnloadTestPackage()
        {
            taskManager.StartTask(Unload);
        }

        public void ViewSourceCode(string testId)
        {
            TestData testData = testModelData.GetTestById(testId);
            if (testData != null)
                EventHandlerUtils.SafeInvoke(ShowSourceCode, this, new ShowSourceCodeEventArgs(testData.CodeLocation));
        }
    }
}
