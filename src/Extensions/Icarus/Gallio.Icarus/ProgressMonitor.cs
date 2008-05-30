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
        private bool showDetails = false;

        public ProgressMonitor()
        {
            InitializeComponent();
        }

        public string StatusText
        {
            set
            {
                statusTextLabel.Text = value;
                detailsTextBox.AppendText(value + Environment.NewLine);
            }
        }

        public int TotalWorkUnits
        {
            set
            {
                progressBar.Maximum = value;
                if (value == 0)
                    detailsTextBox.Clear();
            }
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

        private void detailsButton_Click(object sender, EventArgs e)
        {
            showDetails = !showDetails;
            if (showDetails)
            {
                Height = 445;
                detailsButton.Text = "Details <<";
            }
            else
            {
                Height = 165;
                detailsButton.Text = "Details >>";
            }
        }
    }
}
