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

using Aga.Controls.Tree;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Runner.Events;
using Gallio.Runner.Reports;
using Gallio.Utilities;
using Gallio.Icarus.Mediator.Interfaces;

namespace Gallio.Icarus
{
    public partial class TestResults : DockWindow
    {
        readonly ITestController testController;

        public int TotalTests
        {
            get { return testProgressStatusBar.Total; }
            set { testProgressStatusBar.Total = value; }
        }

        public TestResults(IMediator mediator)
        {
            this.testController = mediator.TestController;

            InitializeComponent();

            testController.TestStepFinished += delegate { Sync.Invoke(this, UpdateTestResults); };
            testController.SelectedTests.ListChanged += delegate { Sync.Invoke(this, UpdateTestResults); };
            testController.RunStarted += delegate { Reset(); };

            testProgressStatusBar.DataBindings.Add("Mode", mediator.OptionsController, "TestStatusBarStyle");
            testProgressStatusBar.DataBindings.Add("PassedColor", mediator.OptionsController, "PassedColor");
            testProgressStatusBar.DataBindings.Add("FailedColor", mediator.OptionsController, "FailedColor");
            testProgressStatusBar.DataBindings.Add("InconclusiveColor", mediator.OptionsController, "InconclusiveColor");
            testProgressStatusBar.DataBindings.Add("SkippedColor", mediator.OptionsController, "SkippedColor");
            
            testProgressStatusBar.DataBindings.Add("Passed", testController, "Model.Passed");
            testProgressStatusBar.DataBindings.Add("Failed", testController, "Model.Failed");
            testProgressStatusBar.DataBindings.Add("Skipped", testController, "Model.Skipped");
            testProgressStatusBar.DataBindings.Add("Inconclusive", testController, "Model.Inconclusive");
        }

        protected void UpdateTestResults()
        {
            testResultsList.BeginUpdate();
            testResultsList.Items.Clear();

            if (testController.Model.Root == null)
                return;

            if (testController.SelectedTests.Count == 0)
                UpdateTestResults(testController.Model.Root, 0);
            else
            {
                foreach (TestTreeNode node in testController.SelectedTests)
                    UpdateTestResults(node, 0);
            }

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
                testProgressStatusBar.Total = testController.TestCount;
                testResultsList.Items.Clear();
            });
        }
    }
}