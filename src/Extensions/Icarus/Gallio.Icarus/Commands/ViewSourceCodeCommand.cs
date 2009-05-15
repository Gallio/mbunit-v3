using Gallio.Runtime.ProgressMonitoring;
using Gallio.Icarus.Controllers.Interfaces;

namespace Gallio.Icarus.Commands
{
    internal class ViewSourceCodeCommand : ICommand
    {
        private readonly ISourceCodeController sourceCodeController;

        public string TestId
        {
            get;
            set;
        }

        public ViewSourceCodeCommand(ISourceCodeController sourceCodeController)
        {
            this.sourceCodeController = sourceCodeController;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            sourceCodeController.ViewSourceCode(TestId, progressMonitor);
        }
    }
}
