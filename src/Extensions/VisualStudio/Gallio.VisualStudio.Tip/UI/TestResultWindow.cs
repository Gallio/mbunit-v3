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
using Gallio.Runner.Reports;
using Gallio.VisualStudio.Shell.UI;

namespace Gallio.VisualStudio.Tip.UI
{
    /// <summary>
    /// UI component which displays results details about a Gallio test.
    /// </summary>
    public partial class TestResultWindow : ShellToolWindow
    {
        private readonly GallioTestResult testResult;
        private readonly TestStepRun testStepRun;

        /// <summary>
        /// Default constructor.  (Designer only)
        /// </summary>
        public TestResultWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="testResult">The test result to be displayed</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testResult"/> is null</exception>
        public TestResultWindow(GallioTestResult testResult)
        {
            if (testResult == null)
                throw new ArgumentNullException("testResult");

            this.testResult = testResult;
            testStepRun = GallioTestResultFactory.GetTestStepRun(testResult);

            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Caption = String.Format("{0} [{1}]", testResult.TestName, Properties.Resources.Results);
            InitializeContent();
        }

        private void InitializeContent()
        {
            InitializeStatusHeader();
            InitializeDataGrid();
            InitializeRunViewer();
        }

        private void InitializeStatusHeader()
        {
            if (testResult.HasPassed)
            {
                pictureBoxStatus.Image = Properties.Resources.Passed;
                labelStatus.Text = Properties.Resources.TestHasPassed;
            }
            else
            {
                pictureBoxStatus.Image = Properties.Resources.Failed;
                labelStatus.Text = Properties.Resources.TestHasFailed;
            }
        }

        private void InitializeDataGrid()
        {
            dataGridView.Rows.Clear();
            AddRow(Properties.Resources.TestName, testResult.TestName);
            AddRow(Properties.Resources.Description, testResult.TestDescription);
            AddRow(Properties.Resources.StartTime, testResult.StartTime.ToString());
            AddRow(Properties.Resources.EndTime, testResult.EndTime.ToString());
            AddRow(Properties.Resources.Duration, testResult.Duration.ToString());
            AddRow(Properties.Resources.ComputerName, testResult.ComputerName);
            AddRow(Properties.Resources.OutcomeText, testResult.OutcomeText);
            AddRow(Properties.Resources.ErrorMessage, testResult.ErrorMessage);
            AddRow(Properties.Resources.ErrorStackTrace, testResult.ErrorStackTrace);
        }

        private void AddRow(string name, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                dataGridView.Rows.Add(name, value);
            }
        }

        private void InitializeRunViewer()
        {
            if (testStepRun != null)
                runViewer.Show(new[] { testStepRun });
        }

        private void pictureBoxGallioLogo_Click(object sender, EventArgs e)
        {
            Shell.DTE.ItemOperations.Navigate(
                Properties.Resources.GallioWebSiteUrl, 
                EnvDTE.vsNavigateOptions.vsNavigateOptionsNewWindow);
        }
    }
}
