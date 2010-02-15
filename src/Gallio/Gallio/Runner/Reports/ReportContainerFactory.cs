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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gallio.Common.IO;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// Abstract factory that provides a <see cref="IReportContainer"/> for reading or saving
    /// a report, according to the specified settings.
    /// </summary>
    public class ReportContainerFactory
    {
        private readonly IFileSystem fileSystem;
        private readonly string reportDirectory;
        private readonly string reportName;

        /// <summary>
        /// Constructs a factory.
        /// </summary>
        /// <param name="fileSystem">A file system wrapper.</param>
        /// <param name="reportDirectory">The report directory path.</param>
        /// <param name="reportName">The report name.</param>
        /// <exception cref="ArgumentNullException">Thrown if any argument is null.</exception>
        public ReportContainerFactory(IFileSystem fileSystem, string reportDirectory, string reportName)
        {
            if (fileSystem == null)
                throw new ArgumentNullException("fileSystem");
            if (reportDirectory == null)
                throw new ArgumentNullException("reportDirectory");
            if (reportName == null)
                throw new ArgumentNullException("reportName");

            this.fileSystem = fileSystem;
            this.reportDirectory = reportDirectory;
            this.reportName = reportName;
        }

        /// <summary>
        /// Makes a report container for a saving operation.
        /// </summary>
        /// <param name="reportArchive">Indicates if the report must be packed in a compressed archive.</param>
        /// <returns>A new instance of report container.</returns>
        /// <exception cref="ArgumentNullException">Thrown if any argument is null.</exception>
        public IReportContainer MakeForSaving(ReportArchive reportArchive)
        {
            if (reportArchive == null)
                throw new ArgumentNullException("reportArchive");

            return (IReportContainer)Activator.CreateInstance(reportArchive.ReportContainerForSavingType, reportDirectory, reportName);
        }

        /// <summary>
        /// Makes a report container for a reading operation.
        /// </summary>
        /// <returns>A new instance of report container.</returns>
        public IReportContainer MakeForReading()
        {
            var archiveFileName = Path.Combine(reportDirectory, reportName + ".zip");

            if (fileSystem.FileExists(archiveFileName))
            {
                return new ArchiveReportContainer(reportDirectory, reportName);
            }

            return new FileSystemReportContainer(reportDirectory, reportName);
        }
    }
}
