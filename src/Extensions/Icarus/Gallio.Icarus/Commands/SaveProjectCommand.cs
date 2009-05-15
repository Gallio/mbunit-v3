using Gallio.Runtime.ProgressMonitoring;
using Gallio.Icarus.Controllers.Interfaces;

namespace Gallio.Icarus.Commands
{
    internal class SaveProjectCommand : ICommand
    {
        private readonly IProjectController projectController;

        public string FileName
        {
            get;
            set;
        }

        public SaveProjectCommand(IProjectController projectController)
        {
            this.projectController = projectController;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            projectController.SaveProject(FileName, progressMonitor);
        }
    }
}
