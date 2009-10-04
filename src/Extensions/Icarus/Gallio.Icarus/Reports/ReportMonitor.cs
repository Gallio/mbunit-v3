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

using System.IO;
using System;
using System.Text.RegularExpressions;
using Gallio.Common.Policies;

namespace Gallio.Icarus.Reports
{
    internal class ReportMonitor
    {
        private FileSystemWatcher reportDirectoryWatcher;

        public event EventHandler ReportDirectoryChanged;

        public ReportMonitor(string reportDirectory, string reportNameFormat)
        {
            if (Directory.Exists(reportDirectory))
            {
                SetupDirectoryWatcher(reportDirectory, reportNameFormat);
            }
            else
            {
                string parentDirectory = Path.GetDirectoryName(reportDirectory);
                if (Directory.Exists(parentDirectory))
                {
                    reportDirectoryWatcher = new FileSystemWatcher
                    {
                        NotifyFilter = NotifyFilters.DirectoryName,
                        Path = parentDirectory,
                        EnableRaisingEvents = true
                    };
                    reportDirectoryWatcher.Created += (sender, e) =>
                    {
                        if (e.FullPath != reportDirectory) 
                            return;

                        reportDirectoryWatcher.EnableRaisingEvents = false;
                        SetupDirectoryWatcher(reportDirectory, reportNameFormat);
                        OnReportDirectoryChanged();
                    };
                }
            }
        }

        private void SetupDirectoryWatcher(string reportDirectory, string reportNameFormat)
        {
            var regex = new Regex("{.}");
            reportNameFormat = regex.Replace(reportNameFormat, "*") + "*.xml";
            reportDirectoryWatcher = new FileSystemWatcher
            {
                Filter = reportNameFormat,
                NotifyFilter = NotifyFilters.FileName,
                Path = reportDirectory,
                EnableRaisingEvents = true
            };

            reportDirectoryWatcher.Changed += (sender, e) => OnReportDirectoryChanged();
            reportDirectoryWatcher.Created += (sender, e) => OnReportDirectoryChanged();
            reportDirectoryWatcher.Deleted += (sender, e) => OnReportDirectoryChanged();
            reportDirectoryWatcher.Renamed += (sender, e) => OnReportDirectoryChanged();
        }

        private void OnReportDirectoryChanged()
        {
            EventHandlerPolicy.SafeInvoke(ReportDirectoryChanged, this, EventArgs.Empty);
        }
    }
}
