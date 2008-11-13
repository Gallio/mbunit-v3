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
using System.Threading;
using Gallio.Collections;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Model.Serialization;
using Gallio.Runner.Events;
using Gallio.Runner.Reports;
using Gallio.Utilities;

namespace Gallio.Icarus.Controllers
{
    class ExecutionLogController : IExecutionLogController
    {
        private readonly ITestController testController;
        private readonly HashSet<string> selectedTestIds;

        public ExecutionLogController(ITestController testController)
        {
            this.testController = testController;

            testController.SelectedTests.ListChanged += ListChanged;
            testController.TestStepFinished += TestStepFinished;
            testController.RunStarted += RunStarted;

            selectedTestIds = new HashSet<string>();

            TestStepRuns = new List<TestStepRun>();
            Update();
        }

        public event EventHandler<System.EventArgs> ExecutionLogReset;
        public event EventHandler<System.EventArgs> ExecutionLogUpdated;

        public IList<TestStepRun> TestStepRuns { get; private set; }

        public TestModelData TestModelData { get; private set; }

        private void RunStarted(object sender, System.EventArgs e)
        {
            TestModelData = null;
            TestStepRuns.Clear();

            EventHandlerUtils.SafeInvoke(ExecutionLogReset, true, System.EventArgs.Empty);
        }

        private void ListChanged(object sender, ListChangedEventArgs e)
        {
            selectedTestIds.Clear();

            foreach (TestTreeNode node in testController.SelectedTests)
                selectedTestIds.Add(node.Name);

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
            ThreadPool.QueueUserWorkItem(dummy => testController.Report.Read(report =>
            {
                TestModelData = report.TestModel;

                TestStepRuns.Clear();

                if (report.TestPackageRun != null)
                {
                    // only update log if the test is selected in the tree or, 
                    // if no tests are selected, if it is the root.
                    foreach (TestStepRun run in report.TestPackageRun.AllTestStepRuns)
                        if (selectedTestIds.Contains(run.Step.TestId) || 
                            (selectedTestIds.Count == 0 && run.Step.TestId == testController.Model.Root.Name))
                            TestStepRuns.Add(run);
                }

                EventHandlerUtils.SafeInvoke(ExecutionLogUpdated, this, System.EventArgs.Empty);
            }));
        }
    }
}
