using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    internal class ResetTestsCommand : ICommand
    {
        private readonly ITestController testController;

        public ResetTestsCommand(ITestController testController)
        {
            this.testController = testController;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            testController.ResetTestStatus(progressMonitor);
        }
    }
}
