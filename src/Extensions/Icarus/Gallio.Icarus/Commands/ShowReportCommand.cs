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

using Gallio.Runtime.ProgressMonitoring;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Common.IO;
using Gallio.UI.Progress;

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
