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
