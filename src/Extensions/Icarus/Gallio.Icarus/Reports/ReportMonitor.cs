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
using System.Text.RegularExpressions;
using Gallio.Common.Policies;

namespace Gallio.Icarus.Reports
{
    internal class ReportMonitor
    {
        public event EventHandler ReportDirectoryCreated;
        public event EventHandler ReportDirectoryDeleted;
        
        public event EventHandler<ReportCreatedEventArgs> ReportCreated;
        public event EventHandler<ReportDeletedEventArgs> ReportDeleted;
        public event EventHandler<ReportRenamedEventArgs> ReportRenamed;

        public ReportMonitor(string reportDirectory, string reportNameFormat)
        {
            string parentDirectory = Path.GetDirectoryName(reportDirectory);

            if (!Directory.Exists(parentDirectory)) 
                return;

            SetupParentDirectoryWatcher(reportDirectory, parentDirectory, reportNameFormat);

            if (Directory.Exists(reportDirectory))
                SetupDirectoryWatcher(reportDirectory, reportNameFormat);
        }

        private void SetupParentDirectoryWatcher(string reportDirectory, string parentDirectory, string reportNameFormat)
        {
            var reportDirectoryWatcher = new FileSystemWatcher
            {
                NotifyFilter = NotifyFilters.DirectoryName,
                Path = parentDirectory,
                EnableRaisingEvents = true
            };

            reportDirectoryWatcher.Created += (s, e) =>
            {
                if (e.FullPath != reportDirectory)
                    return;

                EventHandlerPolicy.SafeInvoke(ReportDirectoryCreated, this, EventArgs.Empty);
                SetupDirectoryWatcher(reportDirectory, reportNameFormat);
            };

            reportDirectoryWatcher.Deleted += (s, e) =>
            {
                if (e.FullPath == reportDirectory)
                    EventHandlerPolicy.SafeInvoke(ReportDirectoryDeleted, this, EventArgs.Empty);
            };

            reportDirectoryWatcher.Renamed += (s, e) =>
            {
                if (e.OldFullPath == reportDirectory)
                    EventHandlerPolicy.SafeInvoke(ReportDirectoryDeleted, this, EventArgs.Empty);
                else if (e.FullPath == reportDirectory)
                    EventHandlerPolicy.SafeInvoke(ReportDirectoryCreated, this, EventArgs.Empty);
            };
        }

        private void SetupDirectoryWatcher(string reportDirectory, string reportNameFormat)
        {
            var regex = new Regex("{.}");
            reportNameFormat = regex.Replace(reportNameFormat, "*") + "*.xml";
            
            var reportDirectoryWatcher = new FileSystemWatcher
            {
                Filter = reportNameFormat,
                NotifyFilter = NotifyFilters.FileName,
                Path = reportDirectory,
                EnableRaisingEvents = true
            };

            reportDirectoryWatcher.Created += (s, e) => EventHandlerPolicy.SafeInvoke(ReportCreated, 
                this, new ReportCreatedEventArgs(e.FullPath));

            reportDirectoryWatcher.Deleted += (s, e) => EventHandlerPolicy.SafeInvoke(ReportDeleted,
                this, new ReportDeletedEventArgs(e.FullPath));

            reportDirectoryWatcher.Renamed += (s, e) =>
            {
                // if the new filename is a valid report format, then rename the node
                if (new Regex(reportNameFormat.Replace("*", ".*")).Match(e.FullPath).Success)
                    EventHandlerPolicy.SafeInvoke(ReportRenamed, this, 
                        new ReportRenamedEventArgs(e.OldFullPath, e.FullPath));
                else
                    // otherwise delete it
                    EventHandlerPolicy.SafeInvoke(ReportDeleted, this, 
                        new ReportDeletedEventArgs(e.OldFullPath));
            };
        }

        internal class ReportCreatedEventArgs : EventArgs
        {
            public string FileName { get; private set; }

            public ReportCreatedEventArgs(string fileName)
            {
                FileName = fileName;
            }
        }

        internal class ReportDeletedEventArgs : EventArgs
        {
            public string FileName { get; private set; }

            public ReportDeletedEventArgs(string fileName)
            {
                FileName = fileName;
            }
        }

        internal class ReportRenamedEventArgs : EventArgs
        {
            public string OldFileName { get; private set; }
            public string NewFileName { get; private set; }

            public ReportRenamedEventArgs(string oldFileName, string newFileName)
            {
                OldFileName = oldFileName;
                NewFileName = newFileName;
            }
        }
    }
}
