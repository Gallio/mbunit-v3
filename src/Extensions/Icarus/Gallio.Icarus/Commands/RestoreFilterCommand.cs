using System;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model.Filters;

namespace Gallio.Icarus.Commands
{
    internal class RestoreFilterCommand : ICommand
    {
        private readonly ITestController testController;
        private readonly IProjectController projectController;

        public RestoreFilterCommand(ITestController testController, IProjectController projectController)
        {
            this.testController = testController;
            this.projectController = projectController;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            foreach (var filterInfo in projectController.TestFilters)
            {
                if (progressMonitor.IsCanceled)
                    throw new OperationCanceledException();

                if (filterInfo.FilterName == "AutoSave")
                {
                    var filterSet = FilterUtils.ParseTestFilterSet(filterInfo.Filter);
                    var applyFilterCommand = new ApplyFilterCommand(testController, filterSet);
                    applyFilterCommand.Execute(progressMonitor);
                    return;
                }
            }
        }
    }
}
