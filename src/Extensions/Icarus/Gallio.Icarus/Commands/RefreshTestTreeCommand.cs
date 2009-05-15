using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    internal class RefreshTestTreeCommand : ICommand
    {
        private readonly ITestController testController;

        public RefreshTestTreeCommand(ITestController testController)
        {
            this.testController = testController;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            testController.RefreshTestTree(progressMonitor);
        }
    }
}
