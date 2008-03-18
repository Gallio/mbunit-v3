// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

namespace Gallio.Icarus
{
    public partial class TestResults : DockWindow
    {
        public int TotalTests { set { testProgressStatusBar.Total = value; } }

        public TestResults()
        {
            InitializeComponent();
            filterTestResultsCombo.SelectedIndex = 0;
        }

        public void Reset()
        {
            testProgressStatusBar.Clear();
            testResultsList.Items.Clear();
        }

        public void UpdateTestResults(string testName, TestOutcome testOutcome, double duration, 
            string typeName, string namespaceName, string assemblyName)
        {
            if (testResultsList.InvokeRequired)
            {
                testResultsList.Invoke((MethodInvoker)delegate
                {
                    UpdateTestResults(testName, testOutcome, duration, typeName, namespaceName, assemblyName);
                });
            }
            else
            {
                Color foreColor = Color.Black;
                switch (testOutcome.Status)
                {
                    case TestStatus.Passed:
                        testProgressStatusBar.Passed += 1;
                        foreColor = Color.Green;
                        break;
                    case TestStatus.Failed:
                        testProgressStatusBar.Failed += 1;
                        foreColor = Color.Red;
                        break;
                    case TestStatus.Skipped:
                        testProgressStatusBar.Skipped += 1;
                        foreColor = Color.Yellow;
                        break;
                    case TestStatus.Inconclusive:
                        testProgressStatusBar.Inconclusive += 1;
                        foreColor = Color.SlateGray;
                        break;
                }
                testResultsList.UpdateTestResults(testName, testOutcome, foreColor, duration.ToString(),
                    typeName, namespaceName, assemblyName);
            }
        }

        private void filterTestResultsCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((string)filterTestResultsCombo.SelectedItem)
            {
                case "All tests":
                    testResultsList.Filter = string.Empty;
                    break;
                case "Passed tests":
                    testResultsList.Filter = "passed";
                    break;
                case "Failed tests":
                    testResultsList.Filter = "failed";
                    break;
                case "Skipped tests":
                    testResultsList.Filter = "skipped";
                    break;
                case "Inconclusive tests":
                    testResultsList.Filter = "inconclusive";
                    break;
            }
        }
    }
}