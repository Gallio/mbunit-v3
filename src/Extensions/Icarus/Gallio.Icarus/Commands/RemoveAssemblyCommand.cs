using Gallio.Runtime.ProgressMonitoring;
using Gallio.Icarus.Controllers.Interfaces;

namespace Gallio.Icarus.Commands
{
    internal class RemoveAssemblyCommand : ICommand
    {
        private readonly IProjectController projectController;

        public string FileName
        {
            get;
            set;
        }

        public RemoveAssemblyCommand(IProjectController projectController)
        {
            this.projectController = projectController;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            if (!string.IsNullOrEmpty(FileName))
                projectController.RemoveAssembly(FileName, progressMonitor);
        }
    }
}
