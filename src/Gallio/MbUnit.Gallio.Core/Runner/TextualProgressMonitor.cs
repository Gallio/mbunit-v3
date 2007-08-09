using MbUnit.Framework.Kernel.Events;

namespace MbUnit.Core.Runner
{
    ///<summary>
    /// A textual progress monitor provides a little assistance with building
    /// primarily text-based progress monitors.
    ///</summary>
    public abstract class TextualProgressMonitor : TrackingProgressMonitor
    {
        /// <inheritdoc />
        protected override void OnBeginTask(string taskName, double totalWorkUnits)
        {
            base.OnBeginTask(taskName, totalWorkUnits);
            UpdateDisplay();
        }

        /// <inheritdoc />
        protected override void OnWorked(double workUnits)
        {
            base.OnWorked(workUnits);
            UpdateDisplay();
        }

        /// <inheritdoc />
        protected override void OnDone()
        {
            base.OnDone();
            UpdateDisplay();
        }

        /// <summary>
        /// Updates the display.
        /// </summary>
        protected abstract void UpdateDisplay();
    }
}