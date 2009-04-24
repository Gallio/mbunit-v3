using System;
using System.Collections.Generic;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    internal class AddAssembliesCommand : ICommand
    {
        private readonly IProjectController projectController;
        private readonly ITestController testController;
        private readonly IList<string> assemblyFiles;

        public AddAssembliesCommand(IProjectController projectController, ITestController testController, 
            IList<string> assemblyFiles)
        {
            this.projectController = projectController;
            this.testController = testController;
            this.assemblyFiles = assemblyFiles;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Adding assemblies", 100))
            {
                // add assemblies to test package
                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(10))
                    projectController.AddAssemblies(assemblyFiles, subProgressMonitor);

                if (progressMonitor.IsCanceled)
                    throw new OperationCanceledException();

                // reload tests
                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(90))
                {
                    testController.SetTestPackageConfig(projectController.TestPackageConfig);
                    testController.Explore(subProgressMonitor, projectController.TestRunnerExtensions);
                }
            }
        }
    }
}
