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
using Gallio.Icarus.Models;
using Gallio.Icarus.Remoting;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner.Projects;
using Gallio.Runner.Projects.Schema;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Common.Policies;
using Gallio.UI.Common.Synchronization;

namespace Gallio.Icarus.Controllers
{
    internal class ProjectController : NotifyController, IProjectController
    {
        private readonly IProjectTreeModel projectTreeModel;
        private readonly IOptionsController optionsController;
        private readonly IFileSystem fileSystem;
        private readonly IXmlSerializer xmlSerializer;
        private readonly IFileWatcher fileWatcher;
        private readonly IUnhandledExceptionPolicy unhandledExceptionPolicy;
        private readonly ITestProjectManager testProjectManager;

        private readonly List<string> hintDirectories = new List<string>();
        private readonly List<string> testRunnerExtensions = new List<string>();

        private string treeViewCategory;

        private bool updating;

        public event EventHandler<FileChangedEventArgs> FileChanged;

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

        public List<string> CollapsedNodes
        {
            get;
            set;
        }

        public string TreeViewCategory
        {
            get
            {
                return treeViewCategory;
            }
            set
            {
                treeViewCategory = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TreeViewCategory"));
            }
        }

        public string ReportDirectory
        {
            get { return projectTreeModel.TestProject.ReportDirectory; }
        }

        public string ReportNameFormat
        {
            get { return projectTreeModel.TestProject.ReportNameFormat; }
        }

        public ProjectController(IProjectTreeModel projectTreeModel, IOptionsController optionsController, 
            IFileSystem fileSystem, IXmlSerializer xmlSerializer, IFileWatcher fileWatcher, 
            IUnhandledExceptionPolicy unhandledExceptionPolicy)
        {
            this.projectTreeModel = projectTreeModel;
            this.optionsController = optionsController;
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
            TreeViewCategory = "Namespace";
            CollapsedNodes = new List<string>();
        }

        public void AddFiles(IList<string> files, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Adding files.", (files.Count + 2)))
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

        public void DeleteFilter(FilterInfo filterInfo, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Deleting filter", 1))
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

        public void RemoveFile(string fileName, IProgressMonitor progressMonitor)
        {
            string filePath = Path.GetFullPath(fileName);
            projectTreeModel.TestProject.TestPackage.RemoveFile(new FileInfo(filePath));
            projectTreeModel.NotifyTestProjectChanged();
        }

        public void SaveFilterSet(string filterName, FilterSet<ITestDescriptor> filterSet, 
            IProgressMonitor progressMonitor)
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

        public void OpenProject(string projectName, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Opening project", 100))
            {
                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(70))
                    LoadProjectFile(subProgressMonitor, projectName);

                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(30))
                    LoadUserOptions(projectName, subProgressMonitor);
            }
        }

        private void LoadUserOptions(string projectName, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Loading user options", 100))
            {
                // check if the project has a user options file
                string projectUserOptionsFile = projectName + UserOptions.Extension;
                if (!fileSystem.FileExists(projectUserOptionsFile))
                    return;

                // if it does, use it!
                try
                {
                    var userOptions = xmlSerializer.LoadFromXml<UserOptions>(projectUserOptionsFile);
                    TreeViewCategory = userOptions.TreeViewCategory;
                    CollapsedNodes = userOptions.CollapsedNodes;
                }
                catch
                {
                    // eat any errors
                }
            }
        }

        private void LoadProjectFile(IProgressMonitor progressMonitor, string projectName)
        {
            using (progressMonitor.BeginTask("Loading project file", 100))
            {
                TestProject testProject = new TestProject();

                try
                {
                    testProject = testProjectManager.LoadProject(new FileInfo(projectName));
                }
                catch (Exception ex)
                {
                    unhandledExceptionPolicy.Report("Error loading project file.  The project file may not be compatible with this version of Gallio.", ex);
                }

                progressMonitor.Worked(50);

                LoadProject(testProject, projectName);
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

        public void SaveProject(string projectName, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Saving project", 100))
            {
                if (string.IsNullOrEmpty(projectName))
                    projectName = projectTreeModel.FileName;

                SaveProjectFile(progressMonitor, projectName);

                SaveUserOptions(progressMonitor, projectName);
            }
        }

        private void SaveUserOptions(IProgressMonitor progressMonitor, string projectName)
        {
            progressMonitor.SetStatus("Saving user options");
            string projectUserOptionsFile = projectName + UserOptions.Extension;
            var userOptions = new UserOptions
            {
                TreeViewCategory = TreeViewCategory,
                CollapsedNodes = CollapsedNodes
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

            optionsController.RecentProjects.Add(projectName);
        }

        private void PublishUpdates()
        {
            if (SynchronizationContext.Instance == null)
                return;

            // need to deal with x-thread databinding
            SynchronizationContext.Instance.Send(delegate
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
    }
}
