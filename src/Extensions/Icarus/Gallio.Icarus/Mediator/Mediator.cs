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

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Mediator.Interfaces;
using Gallio.Icarus.ProgressMonitoring;
using Gallio.Icarus.Utilities;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner.Projects;
using Gallio.Runner.Reports;
using Gallio.Runtime.ProgressMonitoring;
using System;
using Gallio.Icarus.Commands;

namespace Gallio.Icarus.Mediator
{
    public class Mediator : IMediator
    {
        private readonly ProgressMonitorProvider progressMonitorProvider = new ProgressMonitorProvider();

        public ITaskManager TaskManager { get; set; }

        public IProjectController ProjectController { get; set; }

        public ITestController TestController { get; set; }

        public ITestResultsController TestResultsController { get; set; }

        public IReportController ReportController { get; set; }

        public IExecutionLogController ExecutionLogController { get; set; }

        public IAnnotationsController AnnotationsController { get; set; }

        public IRuntimeLogController RuntimeLogController { get; set; }

        public IOptionsController OptionsController { get; set; }

        public ISourceCodeController SourceCodeController { get; set; }

        public void GenerateReport()
        {
            TaskManager.QueueTask(() => progressMonitorProvider.Run(GenerateReportImpl));
        }

        private void GenerateReportImpl(IProgressMonitor progressMonitor)
        {
            var reportFolder = Path.Combine(Path.GetDirectoryName(ProjectController.ProjectFileName),
                "Reports");

            if (progressMonitor.IsCanceled)
                throw new OperationCanceledException();

            TestController.ReadReport(
                report => ReportController.GenerateReport(report, reportFolder, progressMonitor));            
        }

        public void NewProject()
        {
            TaskManager.QueueTask(() => progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {

            }));
        }

        public void RefreshTestTree()
        {
            progressMonitorProvider.Run(progressMonitor => 
                TestController.RefreshTestTree(progressMonitor));
        }

        public void RemoveAssembly(string fileName)
        {
            TaskManager.QueueTask(() => progressMonitorProvider.Run(progressMonitor => 
                ProjectController.RemoveAssembly(fileName, progressMonitor)));
        }

        public void ResetTests()
        {
            TaskManager.QueueTask(() => progressMonitorProvider.Run(progressMonitor => 
                TestController.ResetTestStatus(progressMonitor)));
        }

        public void SaveProject(string projectFileName)
        {
            // don't run as task, or the project won't get saved at shutdown
            progressMonitorProvider.Run(progressMonitor => ProjectController.SaveProject(projectFileName, progressMonitor));
        }

        public void ViewSourceCode(string testId)
        {
            progressMonitorProvider.Run(progressMonitor => 
                SourceCodeController.ViewSourceCode(testId, progressMonitor));
        }
    }
}
