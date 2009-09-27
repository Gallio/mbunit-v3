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
using Gallio.Common.Collections;
using Gallio.Common.Policies;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Model.Schema;
using Gallio.Runner.Events;
using Gallio.Runner.Reports.Schema;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.Controllers
{
    class ExecutionLogController : IExecutionLogController
    {
        private readonly ITestController testController;
        private readonly ITaskManager taskManager;
        private readonly HashSet<string> selectedTestIds;

        public ExecutionLogController(ITestController testController, ITaskManager taskManager)
        {
            if (testController == null) 
                throw new ArgumentNullException("testController");
            
            if (taskManager == null) 
                throw new ArgumentNullException("taskManager");

            this.testController = testController;
            this.taskManager = taskManager;

            testController.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != "SelectedTests")
                    return;

                SelectedTestsChanged();
            };
            testController.TestStepFinished += TestStepFinished;
            
            testController.RunStarted += (sender, e) =>
            {
                TestModelData = null;

                EventHandlerPolicy.SafeInvoke(ExecutionLogReset, true, System.EventArgs.Empty);
            };
            
            selectedTestIds = new HashSet<string>();
        }

        public event EventHandler<System.EventArgs> ExecutionLogReset;
        public event EventHandler<ExecutionLogUpdatedEventArgs> ExecutionLogUpdated;

        public TestModelData TestModelData { get; private set; }

        private void SelectedTestsChanged()
        {
            selectedTestIds.Clear();

            testController.SelectedTests.Read(sts =>
            {
                foreach (var node in sts)
                    selectedTestIds.Add(node.Name);
            });
            
            Update();
        }

        private void TestStepFinished(object sender, TestStepFinishedEventArgs e)
        {
            if (selectedTestIds.Count == 0 || selectedTestIds.Contains(e.Test.Id))
                Update();
        }

        private void Update()
        {
            // Do this work in the background to avoid a possible deadlock acquiring the report lock
            // on the UI thread.
            taskManager.BackgroundTask(() => testController.ReadReport(report =>
            {
                TestModelData = report.TestModel;

                var testStepRuns = new List<TestStepRun>();

                if (report.TestPackageRun != null)
                {
                    // only update log if the test is selected in the tree or, 
                    // if no tests are selected, if it is the root.
                    foreach (var run in report.TestPackageRun.AllTestStepRuns)
                        if (selectedTestIds.Contains(run.Step.TestId) || 
                            (selectedTestIds.Count == 0 && run.Step.TestId == testController.Model.Root.Name))
                            testStepRuns.Add(run);
                }

                EventHandlerPolicy.SafeInvoke(ExecutionLogUpdated, this, 
                    new ExecutionLogUpdatedEventArgs(testStepRuns));
            }));
        }
    }
}
