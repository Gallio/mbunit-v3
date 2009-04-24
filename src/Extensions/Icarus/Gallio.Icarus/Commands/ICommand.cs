using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    interface ICommand
    {
        void Execute(IProgressMonitor progressMonitor);
    }
}
