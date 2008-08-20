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
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controls;
using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Icarus.Interfaces;
using Gallio.Model;
using Gallio.Runner.Reports;
using Gallio.Utilities;

namespace Gallio.Icarus
{
    public partial class TestResults : DockWindow
    {
        private IList<string> selectedNodeIds;
        private ITreeModel treeModel;

        public IList<string> SelectedNodeIds
        {
            set
            {
                selectedNodeIds = value;
                UpdateTestResults();
            }
        }

        public ITreeModel TreeModel
        {
            set
            {
                treeModel = value;
                ((TestTreeModel)treeModel).TestResult += TestResults_TestResult;
            }
        }

        public int TotalTests
        {
            set { testProgressStatusBar.Total = value; }
        }

        public TestResults(IOptionsController optionsController)
        {
            selectedNodeIds = new List<string>();
            InitializeComponent();

            testProgressStatusBar.DataBindings.Add("Mode", optionsController, "TestProgressBarStyle");
            testProgressStatusBar.DataBindings.Add("PassedColor", optionsController, "PassedColor");
            testProgressStatusBar.DataBindings.Add("FailedColor", optionsController, "FailedColor");
            testProgressStatusBar.DataBindings.Add("InconclusiveColor", optionsController, "InconclusiveColor");
            testProgressStatusBar.DataBindings.Add("SkippedColor", optionsController, "SkippedColor");
        }

        private void TestResults_TestResult(object sender, TestResultEventArgs e)
        {
            Sync.Invoke(this, delegate
            {
                UpdateProgress(e.TestStepRun);
                UpdateTestResults();
            });
        }

        private void UpdateTestResults()
        {
            testResultsList.BeginUpdate();
            testResultsList.Items.Clear();

            List<TestTreeNode> nodes = new List<TestTreeNode>();
            foreach (string nodeId in selectedNodeIds)
                nodes.AddRange(((TestTreeModel)treeModel).Root.Find(nodeId, true));

            foreach (TestTreeNode node in nodes)
                UpdateTestResults(node, 0);

            testResultsList.Columns[0].Width = -1;
            testResultsList.EndUpdate();
        }

        private void UpdateTestResults(TestTreeNode node, int indentCount)
        {
            foreach (TestStepRun tsr in node.TestStepRuns)
                testResultsList.AddTestStepRun(node.NodeType, tsr, indentCount);

            foreach (Node n in node.Nodes)
            {
                if (n is TestTreeNode)
                    UpdateTestResults((TestTreeNode)n, (indentCount + 1));
            }
        }

        public void Reset()
        {
            testProgressStatusBar.Clear();
            testResultsList.Items.Clear();
        }

        private void UpdateProgress(TestStepRun testStepRun)
        {
            if (testStepRun.Step.IsPrimary && testStepRun.Step.IsTestCase)
            {
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
}