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

using Gallio.Runner.Projects;
using System.IO;
using System;
using Gallio.Common.Policies;

namespace Gallio.Icarus.Reports
{
    internal class ReportMonitor
    {
        private FileSystemWatcher reportDirectoryWatcher;

        public event EventHandler ReportDirectoryChanged;

        public ReportMonitor(Project project)
        {
            if (File.Exists(project.ReportDirectory))
            {
                SetupDirectoryWatcher(project.ReportDirectory);
            }
            else
            {
                string parentDirectory = Directory.GetParent(project.ReportDirectory).FullName;
                if (Directory.Exists(parentDirectory))
                {
                    reportDirectoryWatcher = new FileSystemWatcher();
                    reportDirectoryWatcher.NotifyFilter = NotifyFilters.DirectoryName;
                    reportDirectoryWatcher.Created += (sender, e) =>
                    {
                        if (e.FullPath == project.ReportDirectory)
                        {
                            reportDirectoryWatcher.EnableRaisingEvents = false;
                            SetupDirectoryWatcher(project.ReportDirectory);
                            OnReportDirectoryChanged();
                        }
                    };
                    reportDirectoryWatcher.Path = parentDirectory;
                    reportDirectoryWatcher.EnableRaisingEvents = true;
                }
            }
        }

        private void SetupDirectoryWatcher(string reportDirectory)
        {
            reportDirectoryWatcher = new FileSystemWatcher();
            reportDirectoryWatcher.Filter = "*.xml";
            reportDirectoryWatcher.NotifyFilter = NotifyFilters.FileName;

            reportDirectoryWatcher.Changed += (sender, e) => OnReportDirectoryChanged();
            reportDirectoryWatcher.Created += (sender, e) => OnReportDirectoryChanged();
            reportDirectoryWatcher.Deleted += (sender, e) => OnReportDirectoryChanged();
            reportDirectoryWatcher.Renamed += (sender, e) => OnReportDirectoryChanged();

            reportDirectoryWatcher.Path = reportDirectory;
            reportDirectoryWatcher.EnableRaisingEvents = true;
        }

        private void OnReportDirectoryChanged()
        {
            EventHandlerPolicy.SafeInvoke(ReportDirectoryChanged, this, EventArgs.Empty);
        }
    }
}
