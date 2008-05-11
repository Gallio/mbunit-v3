// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Utilities;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// <para>
    /// A file-system based representation of a report container that uses
    /// ordinary files and folders to store report contents.
    /// </para>
    /// </summary>
    public class FileSystemReportContainer : IReportContainer
    {
        private readonly string reportDirectory;
        private readonly string reportName;

        /// <summary>
        /// Creates a file-system based representation of a report container.
        /// </summary>
        /// <param name="reportDirectory">The report directory path</param>
        /// <param name="reportName">The report name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reportDirectory"/> or
        /// <paramref name="reportName"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="reportName"/> is an empty string</exception>
        public FileSystemReportContainer(string reportDirectory, string reportName)
        {
            if (reportDirectory == null)
                throw new ArgumentNullException(@"reportDirectory");
            if (reportName == null)
                throw new ArgumentNullException(@"reportName");
            if (reportName.Length == 0)
                throw new ArgumentException("Report name must not be empty.", @"reportName");

            if (reportDirectory.Length == 0)
                reportDirectory = Environment.CurrentDirectory;

            this.reportDirectory = Path.GetFullPath(reportDirectory);
            if (!reportDirectory.EndsWith(Path.DirectorySeparatorChar.ToString())
                && ! reportDirectory.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
                this.reportDirectory += Path.DirectorySeparatorChar;

            this.reportName = reportName;
        }

        /// <summary>
        /// Gets the full-path of the report directory.
        /// </summary>
        public string ReportDirectory
        {
            get { return reportDirectory; }
        }

        /// <inheritdoc />
        public string ReportName
        {
            get { return reportName; }
        }

        /// <inheritdoc />
        public void DeleteReport()
        {
            DirectoryInfo dir = new DirectoryInfo(reportDirectory);
            if (dir.Exists)
            {
                foreach (FileSystemInfo entry in dir.GetFileSystemInfos(reportName + @".*"))
                {
                    FileUtils.DeleteAll(entry.FullName);
                }
            }
        }

        /// <inheritdoc />
        public Stream OpenRead(string path)
        {
            string reportFilePath = GetReportFilePath(path);

            return File.Open(reportFilePath, FileMode.Open, FileAccess.Read);
        }

        /// <inheritdoc />
        public Stream OpenWrite(string path, string contentType, Encoding encoding)
        {
            string reportFilePath = GetReportFilePath(path);

            string containingDir = Path.GetDirectoryName(reportFilePath);
            if (!string.IsNullOrEmpty(containingDir))
                Directory.CreateDirectory(containingDir);

            return File.Open(reportFilePath, FileMode.Create, FileAccess.Write);
        }

        /// <inheritdoc />
        public string EncodeFileName(string fileName)
        {
            return FileUtils.EncodeFileName(fileName);
        }

        /// <summary>
        /// Lightly validate the path to catch common programming errors.  This isn't
        /// really strong enough for enforcing any real security constraints.
        /// </summary>
        private string GetReportFilePath(string path)
        {
            if (path == null)
                throw new ArgumentNullException(@"path");
            if (Path.IsPathRooted(path))
                throw new ArgumentException(String.Format("Path must not be absolute.  Was: '{0}'.", path), @"path");
            if (path.Length <= reportName.Length || ! path.StartsWith(reportName) ||
                (path[reportName.Length] != '.' && path[reportName.Length] != Path.DirectorySeparatorChar
                && path[reportName.Length] != Path.AltDirectorySeparatorChar))
                throw new ArgumentException(String.Format("Path must begin with the report name followed by a '.' or directory separator.  Was '{0}'.", path), @"path");

            // Note: Assumes reportDirectory ends with a directory separator char.
            string reportFilePath = Path.GetFullPath(Path.Combine(reportDirectory, path));
            if (!reportFilePath.StartsWith(reportDirectory))
                throw new ArgumentException(String.Format("Path must not refer to a parent directory.  Was '{0}'.", path), @"path");
            return reportFilePath;
        }
    }
}
