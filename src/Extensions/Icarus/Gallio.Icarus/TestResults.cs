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

namespace Gallio.Icarus
{
    public partial class TestResults : DockWindow
    {
        public int TotalTests
        {
            set { testProgressStatusBar.Total = value; }
        }

        public TestResults()
        {
            InitializeComponent();
        }

        public void Reset()
        {
            testProgressStatusBar.Clear();
            testResultsList.Items.Clear();
        }

        public void UpdateTestResults(TestData testData, TestStepRun testStepRun)
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
            testResultsList.UpdateTestResults(testData, testStepRun);
        }
    }
}