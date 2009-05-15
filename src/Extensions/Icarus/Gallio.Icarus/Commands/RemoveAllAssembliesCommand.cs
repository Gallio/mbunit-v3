using System;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    internal class RemoveAllAssembliesCommand : ICommand
    {
        private readonly ITestController testController;
        private readonly IProjectController projectController;

        public RemoveAllAssembliesCommand(ITestController testController, IProjectController projectController)
        {
            this.testController = testController;
            this.projectController = projectController;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Removing all assemblies.", 100))
            {
                // remove all assemblies from test package
                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(50))
                    projectController.RemoveAllAssemblies(subProgressMonitor);

                if (progressMonitor.IsCanceled)
                    throw new OperationCanceledException();

                // reload
                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(50))
                {
                    testController.SetTestPackageConfig(projectController.TestPackageConfig);
                    testController.Explore(subProgressMonitor, projectController.TestRunnerExtensions);
                }
            }
        }
    }
}
