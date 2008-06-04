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
using System.Drawing;
using System.Windows.Forms;

using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;
using Gallio.Icarus.Controls.Interfaces;
using Aga.Controls.Tree;
using Gallio.Icarus.Controls;
using Gallio.Icarus.Core.CustomEventArgs;
using System.Collections.Generic;
using Gallio.Utilities;

namespace Gallio.Icarus
{
    public partial class TestResults : DockWindow
    {
        private string selectedNodeId = string.Empty;
        private ITreeModel treeModel;

        public string SelectedNodeId
        {
            set
            {
                selectedNodeId = value;
                UpdateTestResults();
            }
        }

        public ITreeModel TreeModel
        {
            set
            {
                treeModel = value;
                ((TestTreeModel)treeModel).TestResult += new EventHandler<TestResultEventArgs>(TestResults_TestResult);
            }
        }

        public int TotalTests
        {
            set { testProgressStatusBar.Total = value; }
        }

        public TestResults()
        {
            InitializeComponent();
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
            if (selectedNodeId != string.Empty)
                nodes = ((TestTreeModel)treeModel).Root.Find(selectedNodeId, true);
            else
                nodes.Add(((TestTreeModel)treeModel).Root);

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