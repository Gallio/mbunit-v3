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
using Gallio.Common.Policies;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Events;
using Gallio.Icarus.Models;
using Gallio.Model.Schema;
using Gallio.Runner.Reports.Schema;
using Gallio.UI.Events;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.Controllers
{
    internal class ExecutionLogController : IExecutionLogController, Handles<RunStarted>, 
        Handles<TestSelectionChanged>, Handles<TestStepFinished>
    {
        private readonly ITestController testController;
        private readonly ITestTreeModel testTreeModel;
        private readonly ITaskManager taskManager;
        private readonly HashSet<string> selectedTestIds;
        private const string QueueId = "Gallio.Icarus.ExecutionLog";

        public ExecutionLogController(ITestController testController, ITestTreeModel testTreeModel, 
            ITaskManager taskManager)
        {
            this.testController = testController;
            this.testTreeModel = testTreeModel;
            this.taskManager = taskManager;

            selectedTestIds = new HashSet<string>();
        }

        public event EventHandler<System.EventArgs> ExecutionLogReset;
        public event EventHandler<ExecutionLogUpdatedEventArgs> ExecutionLogUpdated;

        public TestModelData TestModelData { get; private set; }

        private void Update()
        {
            // Do this work in the background to avoid a possible deadlock acquiring the report lock
            // on the UI thread.
            taskManager.ClearQueue(QueueId);
            taskManager.QueueTask(QueueId, new DelegateCommand((pm => testController.ReadReport(report =>
            {
                TestModelData = report.TestModel;

                var testStepRuns = new List<TestStepRun>();

                if (report.TestPackageRun != null)
                {
                    foreach (var testStepRun in report.TestPackageRun.AllTestStepRuns)
                        if (RelevantStep(testStepRun))
                            testStepRuns.Add(testStepRun);
                }

                EventHandlerPolicy.SafeInvoke(ExecutionLogUpdated, this, 
                    new ExecutionLogUpdatedEventArgs(testStepRuns));
            }))));
        }

        private bool RelevantStep(TestStepRun testStepRun)
        {
            // only update log if the test is selected in the tree or, 
            // if no tests are selected, if it is the root.
            return selectedTestIds.Contains(testStepRun.Step.TestId) || 
                (selectedTestIds.Count == 0 && testStepRun.Step.TestId == testTreeModel.Root.Id);
        }

        public void Handle(RunStarted @event)
        {
            TestModelData = null;           
            EventHandlerPolicy.SafeInvoke(ExecutionLogReset, 
                true, System.EventArgs.Empty);
        }

        public void Handle(TestSelectionChanged @event)
        {
            selectedTestIds.Clear();

            foreach (var node in @event.Nodes)
                selectedTestIds.Add(node.Id);

            Update();
        }

        public void Handle(TestStepFinished @event)
        {
            if (selectedTestIds.Count == 0 || selectedTestIds.Contains(@event.TestData.Id))
                Update();
        }
    }
}
