using System;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    internal class RunTestsCommand : ICommand
    {
        private readonly ITestController testController;
        private readonly IProjectController projectController;
        private readonly IOptionsController optionsController;
        private readonly IReportController reportController;

        public bool AttachDebugger
        {
            get;
            set;
        }

        public RunTestsCommand(ITestController testController, IProjectController projectController, 
            IOptionsController optionsController, IReportController reportController)
        {
            this.testController = testController;
            this.projectController = projectController;
            this.optionsController = optionsController;
            this.reportController = reportController;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Running tests", 100))
            {
                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(5))
                    testController.ResetTestStatus(subProgressMonitor);

                // save current filter as last run
                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(5))
                {
                    var filterSet = testController.GenerateFilterSetFromSelectedTests();
                    projectController.SaveFilterSet("LastRun", filterSet, subProgressMonitor);
                }

                // stop if user has canceled
                if (progressMonitor.IsCanceled)
                    throw new OperationCanceledException();

                // run the tests
                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(85))
                    testController.Run(AttachDebugger, subProgressMonitor, projectController.TestRunnerExtensions);

                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(5))
                {
                    if (optionsController.GenerateReportAfterTestRun)
                    {
                        var cmd = new GenerateReportCommand(projectController, testController, reportController);
                        cmd.Execute(subProgressMonitor);
                    }
                }
            }
        }
    }
}
