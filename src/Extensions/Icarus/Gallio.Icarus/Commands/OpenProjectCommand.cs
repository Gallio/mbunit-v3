using System;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    internal class OpenProjectCommand : ICommand
    {
        private readonly ITestController testController;
        private readonly IProjectController projectController;
        private readonly string fileName;

        public string FileName
        {
            get { return fileName; }
        }

        public OpenProjectCommand(ITestController testController, IProjectController projectController, 
            string fileName)
        {
            this.testController = testController;
            this.projectController = projectController;
            this.fileName = fileName;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Opening project", 100))
            {
                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(5))
                    testController.ResetTestStatus(subProgressMonitor);

                if (progressMonitor.IsCanceled)
                    throw new OperationCanceledException();

                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(5))
                    projectController.OpenProject(fileName, subProgressMonitor);

                if (progressMonitor.IsCanceled)
                    throw new OperationCanceledException();

                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(85))
                {
                    testController.SetTestPackageConfig(projectController.TestPackageConfig);
                    testController.Explore(subProgressMonitor, projectController.TestRunnerExtensions);
                }

                if (progressMonitor.IsCanceled)
                    throw new OperationCanceledException();

                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(5))
                {
                    var applyFilterCommand = new RestoreFilterCommand(testController, projectController);
                    applyFilterCommand.Execute(subProgressMonitor);
                }
            }
        }
    }
}
