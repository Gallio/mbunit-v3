// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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

using System;
using System.IO;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Reports;
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

                if (progressMonitor.IsCanceled)
                    throw new OperationCanceledException();

                // save current filter as last run
                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(5))
                {
                    var filterSet = testController.GenerateFilterSetFromSelectedTests();
                    projectController.SaveFilterSet("LastRun", filterSet, subProgressMonitor);
                }

                if (progressMonitor.IsCanceled)
                    throw new OperationCanceledException();

                // run the tests
                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(85))
                    testController.Run(AttachDebugger, subProgressMonitor, 
                        projectController.TestRunnerExtensions);

                if (progressMonitor.IsCanceled)
                    throw new OperationCanceledException();

                using (var subProgressMonitor = progressMonitor.CreateSubProgressMonitor(5))
                {
                    if (!optionsController.GenerateReportAfterTestRun)
                        return;

                    var reportOptions = new ReportOptions(projectController.ReportDirectory, 
                        projectController.ReportNameFormat);
                    var cmd = new GenerateReportCommand(testController, reportController);
                    cmd.ReportOptions = reportOptions;
                    cmd.Execute(subProgressMonitor);
                }
            }
        }
    }
}
