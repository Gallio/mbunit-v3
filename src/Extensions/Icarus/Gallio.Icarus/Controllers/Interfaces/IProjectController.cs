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

using System;
using System.Collections.Generic;
using System.IO;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Models;
using Gallio.Icarus.Projects;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner.Projects.Schema;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.DataBinding;

namespace Gallio.Icarus.Controllers.Interfaces
{
    public interface IProjectController
    {
        string ApplicationBaseDirectory { get; }
        IEnumerable<DirectoryInfo> HintDirectories { get; }
        IProjectTreeModel Model { get; }
        string ReportDirectory { get; }
        string ReportNameFormat { get; }
        bool ShadowCopy { get; }
        TestPackage TestPackage { get; }
        Observable<IList<FilterInfo>> TestFilters { get; }
        IEnumerable<string> TestRunnerExtensionSpecifications { get; }
        string WorkingDirectory { get; }

        event EventHandler<FileChangedEventArgs> FileChanged;
        event EventHandler<ProjectChangedEventArgs> ProjectChanged;

        void AddFiles(IProgressMonitor progressMonitor, IList<string> files);
        void AddHintDirectory(string hintDirectory);
        void AddTestRunnerExtensionSpecification(string testRunnerExtensionSpecification);
        void DeleteFilter(IProgressMonitor progressMonitor, FilterInfo filterInfo);
        void NewProject(IProgressMonitor progressMonitor);
        void OpenProject(IProgressMonitor progressMonitor, string projectLocation);
        void RemoveAllFiles();
        void RemoveFile(string fileName);
        void RemoveHintDirectory(string hintDirectory);
        void RemoveTestRunnerExtensionSpecification(string testRunnerExtensionSpecification);
        void SaveFilterSet(string filterName, FilterSet<ITestDescriptor> filterSet);
        void Save(string projectLocation, IProgressMonitor progressMonitor);
        void SetApplicationBaseDirectory(string applicationBaseDirectory);
        void SetReportNameFormat(string reportNameFormat);
        void SetShadowCopy(bool shadowCopy);
        void SetWorkingDirectory(string workingDirectory);
    }
}
