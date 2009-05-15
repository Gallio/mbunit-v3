using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runner.Projects;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    internal class DeleteFilterCommand : ICommand
    {
        private readonly IProjectController projectController;
        private readonly FilterInfo filterInfo;

        public DeleteFilterCommand(IProjectController projectController, FilterInfo filterInfo)
        {
            this.projectController = projectController;
            this.filterInfo = filterInfo;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            projectController.DeleteFilter(filterInfo, progressMonitor);
        }
    }
}
