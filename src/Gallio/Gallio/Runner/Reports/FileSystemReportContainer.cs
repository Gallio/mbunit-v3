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
using System.IO;
using System.Text;
using Gallio.Common.IO;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// A file-system based representation of a report container that uses
    /// ordinary files and folders to store report contents.
    /// </summary>
    public class FileSystemReportContainer : AbstractReportContainer
    {
        /// <summary>
        /// Creates a file-system based representation of a report container.
        /// </summary>
        /// <param name="reportDirectory">The report directory path.</param>
        /// <param name="reportName">The report name.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reportDirectory"/> or
        /// <paramref name="reportName"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="reportName"/> is an empty string.</exception>
        public FileSystemReportContainer(string reportDirectory, string reportName)
            : base(reportDirectory, reportName)
        {
        }

        /// <inheritdoc />
        public override void DeleteReport()
        {
            var directory = new DirectoryInfo(ReportDirectory);
            
            if (directory.Exists)
            {
                foreach (FileSystemInfo entry in directory.GetFileSystemInfos(ReportName + @".*"))
                {
                    FileUtils.DeleteAll(entry.FullName);
                }
            }
        }

        /// <inheritdoc />
        public override Stream OpenRead(string path)
        {
            string reportFilePath = ValidateFilePath(path);
            return File.Open(reportFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        /// <inheritdoc />
        public override Stream OpenWrite(string path, string contentType, Encoding encoding)
        {
            string reportFilePath = ValidateFilePath(path);
            string containingDirectory = Path.GetDirectoryName(reportFilePath);

            if (!string.IsNullOrEmpty(containingDirectory))
            {
                Directory.CreateDirectory(containingDirectory);
            }

            return File.Open(reportFilePath, FileMode.Create, FileAccess.Write);
        }
    }
}
