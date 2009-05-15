using Gallio.Runtime.ProgressMonitoring;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Common.IO;

namespace Gallio.Icarus.Commands
{
    internal class ShowReportCommand : ICommand
    {
        private readonly ITestController testController;
        private readonly IReportController reportController;
        private readonly IFileSystem fileSystem;

        public string ReportFormat
        {
            get;
            set;
        }

        public ShowReportCommand(ITestController testController, IReportController reportController, 
            IFileSystem fileSystem)
        {
            this.testController = testController;
            this.reportController = reportController;
            this.fileSystem = fileSystem;
            ReportFormat = string.Empty;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            testController.ReadReport(report =>
            {
                string fileName = reportController.ShowReport(report, ReportFormat, progressMonitor);
                if (!string.IsNullOrEmpty(fileName) && fileSystem.FileExists(fileName))
                    fileSystem.OpenFile(fileName);
            });
        }
    }
}
