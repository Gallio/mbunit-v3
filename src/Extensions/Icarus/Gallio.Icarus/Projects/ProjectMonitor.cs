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
