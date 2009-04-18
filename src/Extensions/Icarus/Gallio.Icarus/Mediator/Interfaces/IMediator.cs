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

using System.Collections.Generic;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.ProgressMonitoring;
using Gallio.Icarus.Utilities;
using Gallio.Runner.Projects;

namespace Gallio.Icarus.Mediator.Interfaces
{
    public interface IMediator
    {
        IAnnotationsController AnnotationsController { get; set; }
        IExecutionLogController ExecutionLogController { get; set; }
        IOptionsController OptionsController { get; set; }
        ProgressMonitorProvider ProgressMonitorProvider { get; }
        IProjectController ProjectController { get; set; }
        IReportController ReportController { get; set; }
        IRuntimeLogController RuntimeLogController { get; }
        ITestController TestController { get; set; }
        ISynchronizationContext SynchronizationContext { set; }
        ITestResultsController TestResultsController { get; set; }

        void AddAssemblies(IList<string> assemblyFiles);
        void ApplyFilter(string filter);
        void Cancel();
        void ConvertSavedReport(string fileName, string format);
        void DeleteFilter(FilterInfo filterInfo);
        void DeleteReport(string fileName);
        void GenerateReport();
        void NewProject();
        void OpenProject(string fileName);
        void RefreshTestTree();
        void Reload();
        void RemoveAllAssemblies();
        void RemoveAssembly(string fileName);
        void ResetTests();
        void RunTests(bool attachDebugger);
        void SaveFilter(string filterName);
        void SaveProject(string projectFileName);
        void ShowReport(string reportFormat);
        void ViewSourceCode(string testId);
    }
}
