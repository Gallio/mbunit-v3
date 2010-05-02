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
using Gallio.Common.Xml;
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
using Path=System.IO.Path;

namespace Gallio.Icarus.Controllers
{
    public class ProjectController : NotifyController, IProjectController, Handles<ProjectChanged>, 
        Handles<TreeViewCategoryChanged>
    {
        private readonly IProjectTreeModel projectTreeModel;
        private readonly IEventAggregator eventAggregator;
        private readonly IFileSystem fileSystem;
        private readonly IXmlSerializer xmlSerializer;
        private readonly IFileWatcher fileWatcher;
        private readonly IUnhandledExceptionPolicy unhandledExceptionPolicy;
        private readonly ITestProjectManager testProjectManager;

        private readonly List<string> hintDirectories = new List<string>();
        private readonly List<string> testRunnerExtensions = new List<string>();

        private bool updating;
        private string treeViewCategory;

        public event EventHandler<FileChangedEventArgs> FileChanged;
        public event EventHandler<ProjectChangedEventArgs> ProjectChanged;

        public IProjectTreeModel Model
        {
            get { return projectTreeModel; }
        }

        public TestPackage TestPackage
        {
            get { return projectTreeModel.TestProject.TestPackage; }
        }

        public Observable<IList<FilterInfo>> TestFilters { get; private set; }

        public BindingList<string> HintDirectories { get; private set; }

        public BindingList<string> TestRunnerExtensions { get; private set; }

        public string ProjectFileName
        {
            get { return projectTreeModel.FileName; }
        }

        public Observable<IList<string>> CollapsedNodes { get; private set; }

        public string ReportDirectory
        {
            get { return projectTreeModel.TestProject.ReportDirectory; }
        }

        public string ReportNameFormat
        {
            get { return projectTreeModel.TestProject.ReportNameFormat; }
        }

        public ProjectController(IProjectTreeModel projectTreeModel, IEventAggregator eventAggregator, IFileSystem fileSystem, 
            IXmlSerializer xmlSerializer, IFileWatcher fileWatcher, IUnhandledExceptionPolicy unhandledExceptionPolicy)
        {
            this.projectTreeModel = projectTreeModel;
            this.eventAggregator = eventAggregator;
            this.fileSystem = fileSystem;
            this.xmlSerializer = xmlSerializer;
            this.fileWatcher = fileWatcher;
            this.unhandledExceptionPolicy = unhandledExceptionPolicy;

            testProjectManager = new DefaultTestProjectManager(fileSystem, xmlSerializer);

            TestFilters = new Observable<IList<FilterInfo>>(new List<FilterInfo>());
            TestFilters.PropertyChanged += (s, e) =>
            {
                if (updating)
                    return;

                projectTreeModel.TestProject.ClearTestFilters();
                GenericCollectionUtils.ForEach(TestFilters.Value, x => 
                    projectTreeModel.TestProject.AddTestFilter(x));
            };

            HintDirectories = new BindingList<string>(hintDirectories);
            HintDirectories.ListChanged += delegate
            {
                if (updating)
                    return;

                projectTreeModel.TestProject.TestPackage.ClearHintDirectories();
                GenericCollectionUtils.ForEach(HintDirectories, x => projectTreeModel.TestProject.TestPackage.AddHintDirectory(new DirectoryInfo(x)));
            };

            TestRunnerExtensions = new BindingList<string>(testRunnerExtensions);
            TestRunnerExtensions.ListChanged += delegate
            {
                if (updating)
                    return;

                projectTreeModel.TestProject.ClearTestRunnerExtensionSpecifications();
                GenericCollectionUtils.ForEach(TestRunnerExtensions, x => projectTreeModel.TestProject.AddTestRunnerExtensionSpecification(x));
            };

            fileWatcher.FileChangedEvent += delegate(string fullPath)
            {
                string fileName = Path.GetFileName(fullPath);
                EventHandlerPolicy.SafeInvoke(FileChanged, this, new FileChangedEventArgs(fileName));
            };

            // default tree view category
            treeViewCategory = "Namespace";
            CollapsedNodes = new Observable<IList<string>>(new List<string>());
        }

        public void AddFiles(IProgressMonitor progressMonitor, IList<string> files)
        {
            using (progressMonitor.BeginTask(Resources.AddingFiles, (files.Count + 2)))
            {
                IList<string> validFiles = new List<string>();
                foreach (string file in files)
                {
                    string filePath = Path.GetFullPath(file);

                    if (fileSystem.FileExists(filePath))
                        validFiles.Add(filePath);
                    progressMonitor.Worked(1);
                }

                GenericCollectionUtils.ForEach(validFiles, x => projectTreeModel.TestProject.TestPackage.AddFile(new FileInfo(x)));
                projectTreeModel.NotifyTestProjectChanged();

                progressMonitor.Worked(1);
                fileWatcher.Add(validFiles);
            }
        }

        public void DeleteFilter(IProgressMonitor progressMonitor, FilterInfo filterInfo)
        {
            using (progressMonitor.BeginTask(Resources.DeletingFilter, 1))
            {
                projectTreeModel.TestProject.RemoveTestFilter(filterInfo);
                TestFilters.Value = new List<FilterInfo>(projectTreeModel.TestProject.TestFilters); // notify UI
            }
        }

        public void RemoveAllFiles(IProgressMonitor progressMonitor)
        {
            projectTreeModel.TestProject.TestPackage.ClearFiles();
            projectTreeModel.NotifyTestProjectChanged();
        }

        public void RemoveFile(IProgressMonitor progressMonitor, string fileName)
        {
            string filePath = Path.GetFullPath(fileName);
            projectTreeModel.TestProject.TestPackage.RemoveFile(new FileInfo(filePath));
            projectTreeModel.NotifyTestProjectChanged();
        }

        public void SaveFilterSet(IProgressMonitor progressMonitor, string filterName, FilterSet<ITestDescriptor> filterSet)
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
            using (progressMonitor.BeginTask(Resources.OpeningProject, 100))
            {
                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(70))
                    LoadProjectFile(subProgressMonitor, projectLocation);

                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(30))
                    LoadUserOptions(subProgressMonitor, projectLocation);

                eventAggregator.Send(new ProjectLoaded(projectLocation));
            }
        }

        private void LoadUserOptions(IProgressMonitor progressMonitor, string projectLocation)
        {
            using (progressMonitor.BeginTask("Loading user options", 100))
            {
                string projectUserOptionsFile = projectLocation + UserOptions.Extension;
                
                if (!fileSystem.FileExists(projectUserOptionsFile))
                    return;

                // if it does, use it!
                try
                {
                    var userOptions = xmlSerializer.LoadFromXml<UserOptions>(projectUserOptionsFile);
                    treeViewCategory = userOptions.TreeViewCategory;
                    eventAggregator.Send(new TreeViewCategoryChanged(userOptions.TreeViewCategory));
                    CollapsedNodes.Value = userOptions.CollapsedNodes;
                }
                catch (Exception ex)
                {
                    unhandledExceptionPolicy.Report("Failed to load user options.", ex);
                }
            }
        }

        private void LoadProjectFile(IProgressMonitor progressMonitor, string projectLocation)
        {
            using (progressMonitor.BeginTask("Loading project file", 100))
            {
                TestProject testProject = new TestProject();

                try
                {
                    testProject = testProjectManager.LoadProject(new FileInfo(projectLocation));
                }
                catch (Exception ex)
                {
                    unhandledExceptionPolicy.Report("Error loading project file.  The project file may not be compatible with this version of Gallio.", ex);
                }

                progressMonitor.Worked(50);

                LoadProject(testProject, projectLocation);
            }
        }

        private void LoadProject(TestProject testProject, string projectName)
        {
            projectTreeModel.FileName = projectName;
            projectTreeModel.TestProject = testProject;

            fileWatcher.Clear();
            GenericCollectionUtils.ForEach(testProject.TestPackage.Files, x => fileWatcher.Add(x.FullName));

            PublishUpdates();
        }

        public void NewProject(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Creating new project", 100))
            {
                var projectName = Paths.DefaultProject;
                var testProject = testProjectManager.NewProject(projectName);
                
                progressMonitor.Worked(50);
                
                LoadProject(testProject, projectName);
            }
        }

        public void SaveProject(IProgressMonitor progressMonitor, string projectLocation)
        {
            using (progressMonitor.BeginTask("Saving project", 100))
            {
                if (string.IsNullOrEmpty(projectLocation))
                    projectLocation = projectTreeModel.FileName;

                SaveProjectFile(progressMonitor, projectLocation);

                SaveUserOptions(progressMonitor, projectLocation);

                eventAggregator.Send(new ProjectSaved(projectLocation));
            }
        }

        private void SaveUserOptions(IProgressMonitor progressMonitor, string projectName)
        {
            progressMonitor.SetStatus("Saving user options");
            string projectUserOptionsFile = projectName + UserOptions.Extension;
            var userOptions = new UserOptions
            {
                TreeViewCategory = treeViewCategory,
                CollapsedNodes = new List<string>(CollapsedNodes.Value)
            };
            progressMonitor.Worked(10);

            xmlSerializer.SaveToXml(userOptions, projectUserOptionsFile);
        }

        private void SaveProjectFile(IProgressMonitor progressMonitor, string projectName)
        {
            string dir = Path.GetDirectoryName(projectName);
            // create folder (if necessary)
            if (!fileSystem.DirectoryExists(dir))
                fileSystem.CreateDirectory(dir);
            progressMonitor.Worked(10);

            testProjectManager.SaveProject(projectTreeModel.TestProject, new FileInfo(projectName));
            progressMonitor.Worked(50);
        }

        private void PublishUpdates()
        {
            // need to deal with x-thread databinding
            SynchronizationContext.Send(delegate
            {
                updating = true;

                TestFilters.Value = new List<FilterInfo>(projectTreeModel.TestProject.TestFilters);

                HintDirectories.Clear();
                foreach (var hintDirectory in TestPackage.HintDirectories)
                    HintDirectories.Add(hintDirectory.ToString());

                TestRunnerExtensions.Clear();
                foreach (var testRunnerExtension in projectTreeModel.TestProject.TestRunnerExtensionSpecifications)
                    TestRunnerExtensions.Add(testRunnerExtension);

                OnPropertyChanged(new PropertyChangedEventArgs("TestPackage"));

                updating = false;
            }, null);
        }

        public void Handle(ProjectChanged @event)
        {
            if (ProjectChanged != null)
                ProjectChanged(this, new ProjectChangedEventArgs(@event.ProjectLocation));
        }

        public void Handle(TreeViewCategoryChanged @event)
        {
            treeViewCategory = @event.TreeViewCategory;
        }
    }
}
