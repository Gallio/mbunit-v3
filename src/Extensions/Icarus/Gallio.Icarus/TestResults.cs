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

using System.Collections.Generic;
using Aga.Controls.Tree;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.Interfaces;
using Gallio.Model;
using Gallio.Runner.Events;
using Gallio.Runner.Reports;
using Gallio.Utilities;

namespace Gallio.Icarus
{
    public partial class TestResults : DockWindow
    {
        readonly ITestController testController;

        public int TotalTests
        {
            set { testProgressStatusBar.Total = value; }
        }

        public TestResults(ITestController testController, IOptionsController optionsController)
        {
            this.testController = testController;

            InitializeComponent();

            testController.TestStepFinished += testController_TestStepFinished;
            testController.SelectedTests.ListChanged += delegate { Sync.Invoke(this, UpdateTestResults); };
            testController.RunStarted += delegate { Reset(); };

            testProgressStatusBar.DataBindings.Add("Mode", optionsController, "TestProgressBarStyle");
            testProgressStatusBar.DataBindings.Add("PassedColor", optionsController, "PassedColor");
            testProgressStatusBar.DataBindings.Add("FailedColor", optionsController, "FailedColor");
            testProgressStatusBar.DataBindings.Add("InconclusiveColor", optionsController, "InconclusiveColor");
            testProgressStatusBar.DataBindings.Add("SkippedColor", optionsController, "SkippedColor");
        }

        void testController_TestStepFinished(object sender, TestStepFinishedEventArgs e)
        {
            Sync.Invoke(this, delegate
            {
                UpdateProgress(e.TestStepRun);
                UpdateTestResults();
            });
        }

        void UpdateTestResults()
        {
            testProgressStatusBar.Total = testController.TestCount;

            testResultsList.BeginUpdate();
            testResultsList.Items.Clear();

            List<TestTreeNode> nodes = new List<TestTreeNode>();
            if (testController.SelectedTests.Count > 0)
                foreach (string nodeId in testController.SelectedTests)
                    nodes.AddRange(((ITestTreeModel)testController.Model).Root.Find(nodeId, true));
            else
                nodes.Add(((ITestTreeModel)testController.Model).Root);

            foreach (TestTreeNode node in nodes)
                UpdateTestResults(node, 0);

            testResultsList.Columns[0].Width = -1;
            testResultsList.EndUpdate();
        }

        void UpdateTestResults(TestTreeNode node, int indentCount)
        {
            foreach (TestStepRun tsr in node.TestStepRuns)
                testResultsList.AddTestStepRun(node.NodeType, tsr, indentCount);

            foreach (Node n in node.Nodes)
            {
                if (n is TestTreeNode)
                    UpdateTestResults((TestTreeNode)n, (indentCount + 1));
            }
        }

        void Reset()
        {
            Sync.Invoke(this, delegate
            {
                testProgressStatusBar.Clear();
                testResultsList.Items.Clear();
            });
        }

        void UpdateProgress(TestStepRun testStepRun)
        {
            if (!testStepRun.Step.IsPrimary || !testStepRun.Step.IsTestCase)
                return;

            switch (testStepRun.Result.Outcome.Status)
            {
                case TestStatus.Passed:
                    testProgressStatusBar.Passed += 1;
                    break;
                case TestStatus.Failed:
                    testProgressStatusBar.Failed += 1;
                    break;
                case TestStatus.Skipped:
                    testProgressStatusBar.Skipped += 1;
                    break;
                case TestStatus.Inconclusive:
                    testProgressStatusBar.Inconclusive += 1;
                    break;
            }
        }
    }
}