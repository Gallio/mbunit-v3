using System;
using System.Windows.Forms;
using Gallio.Common.Concurrency;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.UI.ProgressMonitoring
{
    ///<summary>
    /// Impl of a tool strip progress bar for Gallio progress monitoring.
    ///</summary>
    public class ToolStripProgressBar : System.Windows.Forms.ToolStripProgressBar
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
                Maximum = 0;
                return;
            }

            if (!double.IsNaN(progressMonitor.TotalWorkUnits))
            {
                Style = ProgressBarStyle.Continuous;
                Maximum = Convert.ToInt32(progressMonitor.TotalWorkUnits);
                Value = Convert.ToInt32(progressMonitor.CompletedWorkUnits);
            }
            else
            {
                Style = ProgressBarStyle.Marquee;
            }
        }
    }
}
