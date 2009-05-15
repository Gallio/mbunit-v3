using System;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model.Filters;

namespace Gallio.Icarus.Commands
{
    internal class ReloadCommand : ICommand
    {
        private readonly ITestController testController;
        private readonly IProjectController projectController;

        public ReloadCommand(ITestController testController, IProjectController projectController)
        {
            this.testController = testController;
            this.projectController = projectController;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Reloading", 100))
            {
                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(95))
                    testController.Explore(subProgressMonitor, projectController.TestRunnerExtensions);

                if (progressMonitor.IsCanceled)
                    throw new OperationCanceledException();

                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(5))
                    RestoreFilter(subProgressMonitor);
            }
        }

        private void RestoreFilter(IProgressMonitor progressMonitor)
        {
            foreach (var filterInfo in projectController.TestFilters)
            {
                if (progressMonitor.IsCanceled)
                    throw new OperationCanceledException();

                if (filterInfo.FilterName == "AutoSave")
                {
                    var filterSet = FilterUtils.ParseTestFilterSet(filterInfo.Filter);
                    testController.ApplyFilterSet(filterSet);
                    return;
                }
            }
        }
    }
}
