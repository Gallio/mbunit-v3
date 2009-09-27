using System;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    public class DelegateCommand : ICommand
    {
        public Action<IProgressMonitor> Action
        {
            get; set;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            if (Action == null)
                return;

            Action(progressMonitor);
        }
    }
}
