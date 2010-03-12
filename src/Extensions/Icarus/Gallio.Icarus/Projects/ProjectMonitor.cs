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

using System.IO;
using System.Timers;
using Gallio.Icarus.Events;

namespace Gallio.Icarus.Projects
{
    public class ProjectMonitor : IProjectMonitor, Handles<ProjectOpened>, Handles<ApplicationShutdown>
    {
        private readonly IEventAggregator eventAggregator;
        private FileSystemWatcher fileSystemWatcher;
        private Timer timer;

        public ProjectMonitor(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public void Handle(ProjectOpened @event)
        {
            var directoryName = Path.GetDirectoryName(@event.ProjectLocation);

            if (fileSystemWatcher != null)
            {
                timer.Dispose();
                fileSystemWatcher.Dispose();
            }

            fileSystemWatcher = new FileSystemWatcher(directoryName)
            {
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = Path.GetFileName(@event.ProjectLocation),
                IncludeSubdirectories = false
            };

            var projectChanged = new ProjectChanged(@event.ProjectLocation);
            timer = new Timer(1000)
            {
                Enabled = false,
                AutoReset = false
            };
            fileSystemWatcher.Changed += (s, e) => timer.Enabled = true;
            timer.Elapsed += (se, ev) => eventAggregator.Send(projectChanged);
        }

        public void Handle(ApplicationShutdown @event)
        {
            if (fileSystemWatcher == null) 
                return;

            fileSystemWatcher.EnableRaisingEvents = false;
            fileSystemWatcher.Dispose();
        }
    }
}
