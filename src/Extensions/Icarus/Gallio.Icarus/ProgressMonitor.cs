using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Gallio.Icarus.Interfaces;

namespace Gallio.Icarus
{
    public partial class ProgressMonitor : Form
    {
        public ProgressMonitor()
        {
            InitializeComponent();
        }

        public string TaskName
        {
            set { Text = value; }
        }

        public string SubTaskName
        {
            set { subTaskNameLabel.Text = value; }
        }

        public string Progress
        {
            set { progressLabel.Text = value; }
        }

        public int TotalWorkUnits
        {
            set { progressBar.Maximum = value; }
        }

        public int CompletedWorkUnits
        {
            set { progressBar.Value = value; }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            ((IProjectAdapterView)Owner).CancelRunningOperation();
        }

        private void runInBackgroundButton_Click(object sender, EventArgs e)
        {
            ((IProjectAdapterView)Owner).ShowProgressMonitor = false;
            Hide();
        }
    }
}
