using System;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    internal class SaveFilterCommand : ICommand
    {
        private readonly ITestController testController;
        private readonly IProjectController projectController;
        private readonly string filterName;

        public SaveFilterCommand(ITestController testController, IProjectController projectController, 
            string filterName)
        {
            this.testController = testController;
            this.projectController = projectController;
            this.filterName = filterName;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Saving filter", 2))
            {
                var filterSet = testController.GenerateFilterSetFromSelectedTests();

                if (progressMonitor.IsCanceled)
                    throw new OperationCanceledException();

                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(50))
                    projectController.SaveFilterSet(filterName, filterSet, subProgressMonitor);
            }
        }
    }
}
