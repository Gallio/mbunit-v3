using Gallio.Icarus.ProgressMonitoring;
using Gallio.Icarus.ProgressMonitoring.EventArgs;

namespace Gallio.Icarus.Controllers.EventArgs
{
    internal class DisplayProgressDialogEventArgs : System.EventArgs
    {
        public ProgressMonitorPresenter ProgressMonitor
        {
            get; private set;
        }

        public ProgressUpdateEventArgs ProgressUpdateEventArgs
        {
            get; private set;
        }

        public DisplayProgressDialogEventArgs(ProgressMonitorPresenter progressMonitor, 
            ProgressUpdateEventArgs progressUpdateEventArgs)
        {
            ProgressMonitor = progressMonitor;
            ProgressUpdateEventArgs = progressUpdateEventArgs;
        }
    }
}
