using System.Collections.Generic;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.ProgressMonitoring;
using Gallio.Runner.Projects;

namespace Gallio.Icarus.Mediator.Interfaces
{
    public interface IMediator
    {
        ProgressMonitorProvider ProgressMonitorProvider { get; }
        IProjectController ProjectController { get; set; }
        IReportController ReportController { get; set; }
        TaskManager TaskManager { get; }
        ITestController TestController { get; set; }

        void AddAssemblies(IList<string> assemblyFiles);
        void ApplyFilter(string filter);
        void DeleteFilter(FilterInfo filterInfo);
        void GenerateReport();
        void NewProject();
        void OpenProject(string fileName);
        void Reload();
        void RemoveAllAssemblies();
        void RemoveAssembly(string fileName);
        void ResetTests();
        void RunTests();
        void SaveFilter(string filterName);
        void SaveProject(string projectFileName);
        void ShowReport(string reportFormat);
        void Unload();
        void ViewSourceCode(string testId);
    }
}
