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
using System.ComponentModel;
using System.IO;
using Gallio.Common.Collections;
using Gallio.Common.IO;
using Gallio.Common.Policies;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Events;
using Gallio.Icarus.Models;
using Gallio.Icarus.Projects;
using Gallio.Icarus.Properties;
using Gallio.Icarus.Remoting;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner.Projects;
using Gallio.Runner.Projects.Schema;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Common.Policies;
using Gallio.UI.Common.Synchronization;
using Gallio.UI.DataBinding;
using Gallio.UI.Events;
using Path = System.IO.Path;

namespace Gallio.Icarus.Controllers
{
    public class ProjectController : NotifyController, IProjectController, Handles<ProjectChanged>
    {
        private readonly IProjectTreeModel projectTreeModel;
        private readonly IEventAggregator eventAggregator;
        private readonly IFileSystem fileSystem;
        private readonly IFileWatcher fileWatcher;
        private readonly IUnhandledExceptionPolicy unhandledExceptionPolicy;
        private readonly ITestProjectManager testProjectManager;

        private bool updating;

        public string WorkingDirectory
        {
            get
            {
                return TestPackage.WorkingDirectory != null ? 
                    TestPackage.WorkingDirectory.FullName : "";
            }
        }

        public event EventHandler<FileChangedEventArgs> FileChanged;
        public event EventHandler<ProjectChangedEventArgs> ProjectChanged;

        public string ApplicationBaseDirectory
        {
            get
            {
                return TestPackage.ApplicationBaseDirectory != null ?
                    TestPackage.ApplicationBaseDirectory.FullName : "";
            }
        }

        public IEnumerable<DirectoryInfo> HintDirectories
        {
            get { return TestPackage.HintDirectories; }
        }

        public IProjectTreeModel Model
        {
            get { return projectTreeModel; }
        }

        public bool ShadowCopy
        {
            get { return TestPackage.ShadowCopy; }
        }

        public TestPackage TestPackage
        {
            get { return projectTreeModel.TestProject.TestPackage; }
        }

        public Observable<IList<FilterInfo>> TestFilters { get; private set; }

        public IEnumerable<string> TestRunnerExtensionSpecifications
        {
            get { return projectTreeModel.TestProject.TestRunnerExtensionSpecifications; }
        }

        public string ProjectFileName
        {
            get { return projectTreeModel.FileName; }
        }

        public string ReportDirectory
        {
            get { return projectTreeModel.TestProject.ReportDirectory; }
        }

        public string ReportNameFormat
        {
            get { return projectTreeModel.TestProject.ReportNameFormat; }
        }

        public ProjectController(IProjectTreeModel projectTreeModel, IEventAggregator eventAggregator, IFileSystem fileSystem, 
            IFileWatcher fileWatcher, IUnhandledExceptionPolicy unhandledExceptionPolicy, ITestProjectManager testProjectManager)
        {
            this.projectTreeModel = projectTreeModel;
            this.eventAggregator = eventAggregator;
            this.fileSystem = fileSystem;
            this.fileWatcher = fileWatcher;
            this.unhandledExceptionPolicy = unhandledExceptionPolicy;
            this.testProjectManager = testProjectManager;

            TestFilters = new Observable<IList<FilterInfo>>(new List<FilterInfo>());
            TestFilters.PropertyChanged += (s, e) =>
            {
                if (updating)
                    return;

                projectTreeModel.TestProject.ClearTestFilters();
                GenericCollectionUtils.ForEach(TestFilters.Value, x =>
                    projectTreeModel.TestProject.AddTestFilter(x));
            };

            fileWatcher.FileChangedEvent += delegate(string fullPath)
            {
                string fileName = Path.GetFileName(fullPath);
                EventHandlerPolicy.SafeInvoke(FileChanged, this, new FileChangedEventArgs(fileName));
            };
        }

        public void AddFiles(IProgressMonitor progressMonitor, IList<string> files)
        {
            using (progressMonitor.BeginTask(Resources.AddingFiles, (files.Count + 2)))
            {
                var validFiles = GetValidFiles(progressMonitor, files);

                GenericCollectionUtils.ForEach(validFiles, x =>
                    TestPackage.AddFile(new FileInfo(x)));

                projectTreeModel.NotifyTestProjectChanged();

                progressMonitor.Worked(1);
                fileWatcher.Add(validFiles);
            }
        }

        private IList<string> GetValidFiles(IProgressMonitor progressMonitor, IEnumerable<string> files)
        {
            var validFiles = new List<string>();

            foreach (string file in files)
            {
                var filePath = Path.GetFullPath(file);

                if (fileSystem.FileExists(filePath))
                    validFiles.Add(filePath);

                progressMonitor.Worked(1);
            }

            return validFiles;
        }

        public void AddHintDirectory(string hintDirectory)
        {
            TestPackage.AddHintDirectory(new DirectoryInfo(hintDirectory));
        }

        public void AddTestRunnerExtensionSpecification(string testRunnerExtensionSpecification)
        {
            projectTreeModel.TestProject.AddTestRunnerExtensionSpecification(testRunnerExtensionSpecification);
        }

        public void DeleteFilter(IProgressMonitor progressMonitor, FilterInfo filterInfo)
        {
            using (progressMonitor.BeginTask(Resources.DeletingFilter, 1))
            {
                projectTreeModel.TestProject.RemoveTestFilter(filterInfo);
                TestFilters.Value = new List<FilterInfo>(projectTreeModel.TestProject.TestFilters); // notify UI
            }
        }

        public void RemoveAllFiles()
        {
            TestPackage.ClearFiles();
            projectTreeModel.NotifyTestProjectChanged();
        }

        public void RemoveFile(string fileName)
        {
            var filePath = Path.GetFullPath(fileName);
            TestPackage.RemoveFile(new FileInfo(filePath));
            projectTreeModel.NotifyTestProjectChanged();
        }

        public void RemoveHintDirectory(string hintDirectory)
        {
            TestPackage.RemoveHintDirectory(new DirectoryInfo(hintDirectory));
        }

        public void RemoveTestRunnerExtensionSpecification(string testRunnerExtensionSpecification)
        {
            projectTreeModel.TestProject.RemoveTestRunnerExtensionSpecification(testRunnerExtensionSpecification);
        }

        public void SaveFilterSet(string filterName, FilterSet<ITestDescriptor> filterSet)
        {
            foreach (var filterInfo in TestFilters.Value)
            {
                if (filterInfo.FilterName != filterName)
                    continue;

                filterInfo.FilterExpr = filterSet.ToFilterSetExpr();
                return;
            }

            projectTreeModel.TestProject.AddTestFilter(new FilterInfo(filterName,
                filterSet.ToFilterSetExpr()));
            TestFilters.Value = new List<FilterInfo>(projectTreeModel.TestProject.TestFilters); // notify UI
        }

        public void OpenProject(IProgressMonitor progressMonitor, string projectLocation)
        {
            using (progressMonitor.BeginTask(Resources.ProjectController_Loading_project_file, 100))
            {
                var testProject = new TestProject();

                try
                {
                    testProject = testProjectManager.LoadProject(new FileInfo(projectLocation));
                }
                catch (Exception ex)
                {
                    unhandledExceptionPolicy.Report(Resources.ProjectController_Error_loading_project_file, ex);
                }

                progressMonitor.Worked(50);
                LoadProject(testProject, projectLocation);
            }
        }

        private void LoadProject(TestProject testProject, string projectLocation)
        {
            projectTreeModel.FileName = projectLocation;
            projectTreeModel.TestProject = testProject;

            fileWatcher.Clear();
            GenericCollectionUtils.ForEach(testProject.TestPackage.Files, x => fileWatcher.Add(x.FullName));

            PublishUpdates();

            eventAggregator.Send(this, new ProjectLoaded(projectLocation));
        }

        public void NewProject(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Creating new project", 100))
            {
                var projectLocation = Paths.DefaultProject;
                var testProject = testProjectManager.NewProject(projectLocation);
                progressMonitor.Worked(50);
                LoadProject(testProject, projectLocation);
            }
        }

        public void SaveProject(IProgressMonitor progressMonitor, string projectLocation)
        {
            using (progressMonitor.BeginTask("Saving project", 100))
            {
                if (string.IsNullOrEmpty(projectLocation))
                    projectLocation = projectTreeModel.FileName;

                var dir = Path.GetDirectoryName(projectLocation);

                if (fileSystem.DirectoryExists(dir) == false)
                    fileSystem.CreateDirectory(dir);

                progressMonitor.Worked(10);

                testProjectManager.SaveProject(projectTreeModel.TestProject, new FileInfo(projectLocation));
                progressMonitor.Worked(50);

                eventAggregator.Send(this, new ProjectSaved(projectLocation));
            }
        }

        public void SetApplicationBaseDirectory(string applicationBaseDirectory)
        {
            TestPackage.ApplicationBaseDirectory = string.IsNullOrEmpty(applicationBaseDirectory) == false ? 
                new DirectoryInfo(applicationBaseDirectory) : null;
        }

        public void SetReportNameFormat(string reportNameFormat)
        {
            projectTreeModel.TestProject.ReportNameFormat = reportNameFormat;
        }

        public void SetShadowCopy(bool shadowCopy)
        {
            TestPackage.ShadowCopy = shadowCopy;
        }

        public void SetWorkingDirectory(string workingDirectory)
        {
            TestPackage.WorkingDirectory = string.IsNullOrEmpty(workingDirectory) == false ? 
                new DirectoryInfo(workingDirectory) : null;
        }

        private void PublishUpdates()
        {
            // need to deal with x-thread databinding
            SynchronizationContext.Send(delegate
            {
                updating = true;

                TestFilters.Value = new List<FilterInfo>(projectTreeModel.TestProject.TestFilters);

                OnPropertyChanged(new PropertyChangedEventArgs("TestPackage"));

                updating = false;
            }, null);
        }

        public void Handle(ProjectChanged @event)
        {
            if (ProjectChanged != null)
                ProjectChanged(this, new ProjectChangedEventArgs(@event.ProjectLocation));
        }
    }
}
