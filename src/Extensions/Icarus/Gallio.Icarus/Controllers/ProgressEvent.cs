using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Controllers
{
    public class ProgressEvent : System.EventArgs
    {
        public ObservableProgressMonitor ProgressMonitor { get; private set; }

        public ProgressEvent(ObservableProgressMonitor progressMonitor)
        {
            ProgressMonitor = progressMonitor;
        }
    }
}