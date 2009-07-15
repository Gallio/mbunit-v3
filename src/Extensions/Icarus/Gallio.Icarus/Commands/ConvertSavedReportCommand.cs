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

using System.Diagnostics;
using Gallio.Common.IO;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    internal class ConvertSavedReportCommand : ICommand
    {
        private readonly IReportController reportController;
        private readonly string fileName;
        private readonly string format;
        private readonly IFileSystem fileSystem;

        public string FileName
        {
            get { return fileName; }
        }

        public string Format
        {
            get { return format; }
        }

        public ConvertSavedReportCommand(IReportController reportController, 
            string fileName, string format, IFileSystem fileSystem)
        {
            this.reportController = reportController;
            this.fileName = fileName;
            this.format = format;
            this.fileSystem = fileSystem;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            string generatedFile = reportController.ConvertSavedReport(fileName, format, progressMonitor);
            if (!string.IsNullOrEmpty(generatedFile) && fileSystem.FileExists(generatedFile))
                fileSystem.OpenFile(generatedFile);
        }
    }
}
