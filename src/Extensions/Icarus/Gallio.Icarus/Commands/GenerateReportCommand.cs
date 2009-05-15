using System;
using System.IO;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Icarus.Controllers.Interfaces;

namespace Gallio.Icarus.Commands
{
    internal class GenerateReportCommand : ICommand
    {
        private readonly IProjectController projectController;
        private readonly ITestController testController;
        private readonly IReportController reportController;

        public GenerateReportCommand(IProjectController projectController, ITestController testController,
            IReportController reportController)
        {
            this.projectController = projectController;
            this.testController = testController;
            this.reportController = reportController;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Generating report", 100))
            {
                var reportFolder = Path.Combine(Path.GetDirectoryName(projectController.ProjectFileName),
                    "Reports");

                if (progressMonitor.IsCanceled)
                    throw new OperationCanceledException();

                testController.ReadReport(report => reportController.GenerateReport(report, 
                    reportFolder, progressMonitor));
            }
        }
    }
}
