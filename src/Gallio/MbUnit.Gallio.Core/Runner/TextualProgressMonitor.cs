using System;
using MbUnit.Framework.Kernel.Events;

namespace MbUnit.Core.Runner
{
    ///<summary>
    ///</summary>
    public abstract class TextualProgressMonitor : TrackingProgressMonitor
    {
        /// <summary>
        /// Creates a console progress monitor.
        /// </summary>
        public TextualProgressMonitor()
        {
            ConsoleCancelHandler.Cancel += HandleCancel;
        }

        protected override void OnBeginTask(string taskName, double totalWorkUnits)
        {
            base.OnBeginTask(taskName, totalWorkUnits);
            UpdateDisplay();
        }

        protected override void OnWorked(double workUnits)
        {
            base.OnWorked(workUnits);
            UpdateDisplay();
        }

        protected override void OnDone()
        {
            base.OnDone();

            ConsoleCancelHandler.Cancel -= HandleCancel;
            UpdateDisplay();
        }

        protected abstract void UpdateDisplay();

        private void HandleCancel(object sender, EventArgs e)
        {
            NotifyCanceled();
            UpdateDisplay();
        }
    }
}