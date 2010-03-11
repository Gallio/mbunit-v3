using System;
using System.Text;
using Gallio.Common.Concurrency;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.UI.ProgressMonitoring
{
    ///<summary>
    /// Impl of a tool strip status label for Gallio progress monitoring.
    ///</summary>
    public class ToolStripStatusLabel : System.Windows.Forms.ToolStripStatusLabel
    {
        ///<summary>
        /// Update the progress of the control.
        ///</summary>
        ///<param name="progressMonitor">The <see cref="ObservableProgressMonitor"/> to use.</param>
        public void ProgressChanged(ObservableProgressMonitor progressMonitor)
        {
            if (Parent != null)
                Sync.Invoke(Parent, () => UpdateProgress(progressMonitor));
            else
                UpdateProgress(progressMonitor);
        }

        private void UpdateProgress(ObservableProgressMonitor progressMonitor)
        {
            if (progressMonitor.IsDone)
            {
                Text = "";
                return;
            }

            var sb = new StringBuilder();
            sb.Append(progressMonitor.TaskName);
            if (!string.IsNullOrEmpty(progressMonitor.LeafSubTaskName))
            {
                sb.Append(" - ");
                sb.Append(progressMonitor.LeafSubTaskName);
            }

            if (progressMonitor.TotalWorkUnits > 0)
            {
                var progress = progressMonitor.CompletedWorkUnits / progressMonitor.TotalWorkUnits;
                sb.Append(String.Format(" ({0:P0})", progress));
            }

            Text = sb.ToString();
        }
    }
}
