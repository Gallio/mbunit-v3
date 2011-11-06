// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Windows.Forms;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Common.Synchronization;

namespace Gallio.UI.ProgressMonitoring
{
    ///<summary>
    /// Progress dialog.
    ///</summary>
    public partial class ProgressMonitorDialog : Form
    {
        private readonly ObservableProgressMonitor progressMonitor;

        ///<summary>
        /// Default constructor.
        ///</summary>
        ///<param name="progressMonitor">The progress monitor to display information for.</param>
        public ProgressMonitorDialog(ObservableProgressMonitor progressMonitor)
        {
            InitializeComponent();

            this.progressMonitor = progressMonitor;

            progressMonitor.Changed += OnProgressMonitorOnChanged;
        }

        private void OnProgressMonitorOnChanged(object sender, EventArgs e)
        {
			SyncContext.Post(cb => ProgressUpdate(), null);
        }

        /// <inheritdoc />
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ProgressUpdate();
        }

        private void ProgressUpdate()
        {
            if (Visible == false)
                return;

            UpdateProgressBar();

            UpdateProgressText();

            // if we're finished, then close the window
            if (progressMonitor.IsDone)
                Close();
        }

        private void UpdateProgressText()
        {
            Text = progressMonitor.TaskName;

            subTaskNameLabel.Text = progressMonitor.LeafSubTaskName;

            if (subTaskNameLabel.Text.Length == 0)
                subTaskNameLabel.Text = progressMonitor.TaskName;

            statusLabel.Text = progressMonitor.LeafStatus;

            percentLabel.Text = progressMonitor.TotalWorkUnits > 0
                ? string.Format("({0:P0})", progressMonitor.CompletedWorkUnits / progressMonitor.TotalWorkUnits) 
                : string.Empty;
        }

        private void UpdateProgressBar()
        {
            if (double.IsNaN(progressMonitor.TotalWorkUnits))
            {
                progressBar.Style = ProgressBarStyle.Marquee;
            }
            else
            {
                progressBar.Style = ProgressBarStyle.Continuous;
                progressBar.Maximum = Convert.ToInt32(progressMonitor.TotalWorkUnits);
                progressBar.Value = Convert.ToInt32(progressMonitor.CompletedWorkUnits);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            progressMonitor.Cancel();
        }

        private void runInBackgroundButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <inherit />
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            progressMonitor.Changed -= OnProgressMonitorOnChanged;
        }
    }
}