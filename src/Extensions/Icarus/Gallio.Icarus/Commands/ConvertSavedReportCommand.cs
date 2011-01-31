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

using System;
using Gallio.Common.IO;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    internal class ConvertSavedReportCommand : ICommand
    {
        private readonly IReportController reportController;
        private readonly IFileSystem fileSystem;

        public string FileName { get; set; }
        public string Format { get; set; }

        public ConvertSavedReportCommand(IReportController reportController, IFileSystem fileSystem)
        {
            this.reportController = reportController;
            this.fileSystem = fileSystem;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            if (string.IsNullOrEmpty(FileName))
                throw new ArgumentException("FileName cannot be null or empty");

            if (string.IsNullOrEmpty(Format))
                throw new ArgumentException("Format cannot be null or empty");

            var generatedFile = reportController.ConvertSavedReport(FileName, Format, progressMonitor);

            if (string.IsNullOrEmpty(generatedFile) == false && fileSystem.FileExists(generatedFile))
                fileSystem.OpenFile(generatedFile);
        }
    }
}
