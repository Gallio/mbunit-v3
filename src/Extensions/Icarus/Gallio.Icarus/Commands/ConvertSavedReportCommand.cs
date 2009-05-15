using System.Diagnostics;
using Gallio.Common.IO;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Commands
{
    internal class ConvertSavedReportCommand : ICommand
    {
        private readonly IReportController reportController;
        private readonly string fileName;
        private readonly string format;
        private readonly IFileSystem fileSystem;

        public string FileName
        {
            get { return fileName; }
        }

        public string Format
        {
            get { return format; }
        }

        public ConvertSavedReportCommand(IReportController reportController, 
            string fileName, string format, IFileSystem fileSystem)
        {
            this.reportController = reportController;
            this.fileName = fileName;
            this.format = format;
            this.fileSystem = fileSystem;
        }

        public void Execute(IProgressMonitor progressMonitor)
        {
            string generatedFile = reportController.ConvertSavedReport(fileName, format, progressMonitor);
            if (!string.IsNullOrEmpty(generatedFile) && fileSystem.FileExists(generatedFile))
                fileSystem.OpenFile(generatedFile);
        }
    }
}
