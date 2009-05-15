using Gallio.Runtime.ProgressMonitoring;
using Gallio.Icarus.Controllers.Interfaces;

namespace Gallio.Icarus.Commands
{
    internal class DeleteReportCommand : ICommand
    {
        private readonly IReportController reportController;

        public string FileName
        {
            get;
            set;
        }

        public DeleteReportCommand(IReportController reportController)
        {
            this.reportController = reportController;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            reportController.DeleteReport(FileName, progressMonitor);
        }
    }
}
