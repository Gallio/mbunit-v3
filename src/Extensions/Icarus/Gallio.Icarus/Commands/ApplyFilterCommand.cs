using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    internal class ApplyFilterCommand : ICommand
    {
        private readonly ITestController testController;
        private readonly FilterSet<ITest> filterSet;

        public ApplyFilterCommand(ITestController testController, FilterSet<ITest> filterSet)
        {
            this.testController = testController;
            this.filterSet = filterSet;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            testController.ApplyFilterSet(filterSet);
        }
    }
}
