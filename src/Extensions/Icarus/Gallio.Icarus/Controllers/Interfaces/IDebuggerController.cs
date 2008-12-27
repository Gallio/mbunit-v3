using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Controllers.Interfaces
{
    public interface IDebuggerController
    {
        void Attach(IProgressMonitor progressMonitor);
        void Detach(IProgressMonitor progressMonitor);
    }
}
