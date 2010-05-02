// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Properties;
using Gallio.Icarus.Reports;
using Gallio.Icarus.Services;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    public class RunTestsCommand : ICommand
    {
        private readonly ITestController testController;
        private readonly IProjectController projectController;
        private readonly IOptionsController optionsController;
        private readonly IReportController reportController;
        private readonly IFilterService filterService;

        public bool AttachDebugger { get; set; }

        public RunTestsCommand(ITestController testController, IProjectController projectController, 
            IOptionsController optionsController, IReportController reportController, 
            IFilterService filterService)
        {
            this.testController = testController;
            this.projectController = projectController;
            this.optionsController = optionsController;
            this.reportController = reportController;
            this.filterService = filterService;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask(Resources.RunTestsCommand_Running_tests, 100))
            {
                ResetTestStatus(progressMonitor);

                SaveLastRunFilter(progressMonitor);

                RunTests(progressMonitor);

                GenerateReport(progressMonitor);
            }
        }

        private void ResetTestStatus(IProgressMonitor progressMonitor)
        {
            using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(5))
                testController.ResetTestStatus(subProgressMonitor);
        }

        private void SaveLastRunFilter(IProgressMonitor progressMonitor)
        {
            using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(5))
            {
                var filterSet = filterService.GenerateFilterSetFromSelectedTests();
                projectController.SaveFilterSet(subProgressMonitor, "LastRun", filterSet);
            }
        }

        private void RunTests(IProgressMonitor progressMonitor)
        {
            using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(85))
                testController.Run(AttachDebugger, subProgressMonitor, 
                                   projectController.TestRunnerExtensions);
        }

        private void GenerateReport(IProgressMonitor progressMonitor)
        {
            using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(5))
            {
                if (!optionsController.GenerateReportAfterTestRun)
                    return;

                var reportOptions = new ReportOptions(projectController.ReportDirectory, 
                    projectController.ReportNameFormat);

                var cmd = new GenerateReportCommand(testController, reportController)
                {
                    ReportOptions = reportOptions
                };
                
                cmd.Execute(subProgressMonitor);
            }
        }
    }
}
