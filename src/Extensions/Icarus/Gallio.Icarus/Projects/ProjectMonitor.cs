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
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Events;
using Gallio.UI.Events;

namespace Gallio.Icarus.Projects
{
	public class ProjectMonitor : IProjectMonitor, Handles<ProjectLoaded>, Handles<ApplicationShutdown>,
		Handles<SavingProject>
	{
		private readonly IEventAggregator eventAggregator;
		private FileSystemWatcher fileSystemWatcher;
		private Timer timer;
		private bool savingProject;

		public ProjectMonitor(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
		}

		public void Handle(ProjectLoaded @event)
		{
			TidyUp();

			CreateFileWatcher(@event.ProjectLocation);
		
			SetupTimer(@event);
		}

		private void SetupTimer(ProjectLoaded @event)
		{
			timer = new Timer(1000)
			{
				Enabled = false,
				AutoReset = false
			};
			timer.Elapsed += (se, ev) =>
			{
				if (savingProject == false)
				{
					var projectChanged = new ProjectChanged(@event.ProjectLocation);
					eventAggregator.Send(this, projectChanged);
				}
				else
				{
					savingProject = false;
				}
			};
		}

		private void CreateFileWatcher(string projectLocation)
		{
			var directoryName = Path.GetDirectoryName(projectLocation);

			if (directoryName == null)
				return;

			fileSystemWatcher = new FileSystemWatcher(directoryName)
			{
				EnableRaisingEvents = true,
				NotifyFilter = NotifyFilters.LastWrite,
				Filter = Path.GetFileName(projectLocation),
				IncludeSubdirectories = false
			};

			fileSystemWatcher.Changed += (s, e) => timer.Enabled = true;
		}

		private void TidyUp()
		{
			if (fileSystemWatcher != null)
			{
				timer.Dispose();
				fileSystemWatcher.Dispose();
			}
		}

		public void Handle(ApplicationShutdown @event)
		{
			TidyUp();
		}

		public void Handle(SavingProject @event)
		{
			savingProject = true;
		}
	}
}
