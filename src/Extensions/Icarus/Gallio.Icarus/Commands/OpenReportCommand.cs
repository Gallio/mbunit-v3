using Gallio.Common.IO;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    internal class OpenReportCommand : ICommand
    {
        private readonly IFileSystem fileSystem;

        public string FileName
        {
            get;
            set;
        }

        public OpenReportCommand(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Opening report", 100))
            {
                if (!string.IsNullOrEmpty(FileName))
                    fileSystem.OpenFile(FileName);
            }
        }
    }
}
