using System;
using Gallio.Core.ProgressMonitoring;

namespace Gallio.PowerShellCommands
{
    /// <exclude />
    internal class CommandProgressMonitorProvider : IProgressMonitorProvider
    {
        private readonly BaseCommand cmdlet;

        public CommandProgressMonitorProvider(BaseCommand cmdlet)
        {
            if (cmdlet == null)
                throw new ArgumentNullException("cmdlet");

            this.cmdlet = cmdlet;
        }

        public void Run(TaskWithProgress task)
        {
            using (CommandProgressMonitor progressMonitor = new CommandProgressMonitor(cmdlet))
            {
                task(progressMonitor);
            }
        }
    }
}
