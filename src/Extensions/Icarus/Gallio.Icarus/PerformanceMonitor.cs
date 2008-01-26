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
    public partial class PerformanceMonitor : DockContent
    {
        public PerformanceMonitor()
        {
            InitializeComponent();
            graphFilter.SelectedIndex = 0;
            // refresh graph
            testResultsGraph.DisplayGraph();
        }

        public void UpdateTestResults(string testOutcome, string typeName, string namespaceName, string assemblyName)
        {
            testResultsGraph.UpdateTestResults(testOutcome, typeName, namespaceName, assemblyName);
        }

        private void graphFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            testResultsGraph.Mode = (string)graphFilter.SelectedItem;
        }
    }
}