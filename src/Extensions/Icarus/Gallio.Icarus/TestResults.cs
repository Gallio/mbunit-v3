// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

namespace Gallio.Icarus
{
    public partial class TestResults : DockContent
    {
        public int Passed
        {
            get { return testProgressStatusBar.Passed; }
            set { testProgressStatusBar.Passed = value; }
        }

        public int Failed
        {
            get { return testProgressStatusBar.Failed; }
            set { testProgressStatusBar.Failed = value; }
        }

        public int Inconclusive
        {
            get { return testProgressStatusBar.Inconclusive; }
            set { testProgressStatusBar.Inconclusive = value; }
        }

        public int Total
        {
            get { return testProgressStatusBar.Total; }
            set { testProgressStatusBar.Total = value; }
        }

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

        public void UpdateTestResults(string testName, string testOutcome, Color foreColor, 
            string duration, string typeName, string namespaceName, string assemblyName)
        {
            testResultsList.UpdateTestResults(testName, testOutcome, foreColor, duration, 
                typeName, namespaceName, assemblyName);
        }

        private void filterTestResultsCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((string)filterTestResultsCombo.SelectedItem)
            {
                case "All tests":
                    testResultsList.Filter = string.Empty;
                    break;
                case "Passed tests":
                    testResultsList.Filter = "Passed";
                    break;
                case "Failed tests":
                    testResultsList.Filter = "Failed";
                    break;
                case "Inconclusive tests":
                    testResultsList.Filter = "Inconclusive";
                    break;
            }
        }
    }
}