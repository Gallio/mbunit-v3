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