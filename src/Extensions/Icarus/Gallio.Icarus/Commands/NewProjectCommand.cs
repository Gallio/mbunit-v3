using Gallio.Runtime.ProgressMonitoring;
using System;
using Gallio.Icarus.Controllers.Interfaces;

namespace Gallio.Icarus.Commands
{
    internal class NewProjectCommand : ICommand
    {
        private readonly IProjectController projectController;
        private readonly ITestController testController;

        public NewProjectCommand(IProjectController projectController, ITestController testController)
        {
            this.projectController = projectController;
            this.testController = testController;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Creating new project.", 100))
            {
                // create a new project
                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(10))
                    projectController.NewProject(subProgressMonitor);

                if (progressMonitor.IsCanceled)
                    throw new OperationCanceledException();

                // reload
                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(90))
                {
                    testController.SetTestPackageConfig(projectController.TestPackageConfig);
                    testController.Explore(subProgressMonitor, projectController.TestRunnerExtensions);
                }
            }
        }
    }
}
