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
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Icarus.Utilities;
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
        private readonly IAssemblyWatcher assemblyWatcher;
        private readonly IUnhandledExceptionPolicy unhandledExceptionPolicy;

        private readonly List<FilterInfo> testFilters = new List<FilterInfo>();
        private readonly List<string> hintDirectories = new List<string>();
        private readonly List<string> testRunnerExtensions = new List<string>();

        private string treeViewCategory;

        private bool updating;

        public event EventHandler<AssemblyChangedEventArgs> AssemblyChanged;

        public IProjectTreeModel Model
        {
            get { return projectTreeModel; }
        }

        public TestPackageConfig TestPackageConfig
        {
            get { return projectTreeModel.Project.TestPackageConfig; }
        }

        public BindingList<FilterInfo> TestFilters { get; private set; }

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
            get { return projectTreeModel.Project.ReportDirectory; }
        }

        public string ReportNameFormat
        {
            get { return projectTreeModel.Project.ReportNameFormat; }
        }

        public ProjectController(IProjectTreeModel projectTreeModel, IOptionsController optionsController, 
            IFileSystem fileSystem, IXmlSerializer xmlSerializer, IAssemblyWatcher assemblyWatcher, 
            IUnhandledExceptionPolicy unhandledExceptionPolicy)
        {
            this.projectTreeModel = projectTreeModel;
            this.optionsController = optionsController;
            this.fileSystem = fileSystem;
            this.xmlSerializer = xmlSerializer;
            this.assemblyWatcher = assemblyWatcher;
            this.unhandledExceptionPolicy = unhandledExceptionPolicy;

            TestFilters = new BindingList<FilterInfo>(testFilters);
            TestFilters.ListChanged += delegate
            {
                if (updating)
                    return;

                projectTreeModel.Project.TestFilters.Clear();
                projectTreeModel.Project.TestFilters.AddRange(TestFilters);
            };

            HintDirectories = new BindingList<string>(hintDirectories);
            HintDirectories.ListChanged += delegate
            {
                if (updating)
                    return;

                projectTreeModel.Project.TestPackageConfig.HintDirectories.Clear();
                projectTreeModel.Project.TestPackageConfig.HintDirectories.AddRange(HintDirectories);
            };

            TestRunnerExtensions = new BindingList<string>(testRunnerExtensions);
            TestRunnerExtensions.ListChanged += delegate
            {
                if (updating)
                    return;

                projectTreeModel.Project.TestRunnerExtensions.Clear();
                projectTreeModel.Project.TestRunnerExtensions.AddRange(TestRunnerExtensions);
            };

            assemblyWatcher.AssemblyChangedEvent += delegate(string fullPath)
            {
                string assemblyName = Path.GetFileNameWithoutExtension(fullPath);
                EventHandlerPolicy.SafeInvoke(AssemblyChanged, this, new AssemblyChangedEventArgs(assemblyName));
            };

            // default tree view category
            TreeViewCategory = "Namespace";
            CollapsedNodes = new List<string>();
        }

        public void AddAssemblies(IList<string> assemblies, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Adding assemblies", (assemblies.Count + 2)))
            {
                IList<string> validAssemblies = new List<string>();
                foreach (string assembly in assemblies)
                {
                    if (fileSystem.FileExists(assembly))
                        validAssemblies.Add(assembly);
                    progressMonitor.Worked(1);
                }
                projectTreeModel.Project.TestPackageConfig.AssemblyFiles.AddRange(validAssemblies);
                progressMonitor.Worked(1);
                assemblyWatcher.Add(validAssemblies);
            }
        }

        public void DeleteFilter(FilterInfo filterInfo, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Deleting filter", 1))
                TestFilters.Remove(filterInfo);
        }

        public FilterSet<ITest> GetFilterSet(string filterName, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Getting filter", 1))
            {
                foreach (FilterInfo filterInfo in projectTreeModel.Project.TestFilters)
                {
                    if (filterInfo.FilterName == filterName)
                        return FilterUtils.ParseTestFilterSet(filterInfo.Filter);
                }
                return null;
            }
        }

        public void RemoveAllAssemblies(IProgressMonitor progressMonitor)
        {
            projectTreeModel.Project.TestPackageConfig.AssemblyFiles.Clear();
        }

        public void RemoveAssembly(string fileName, IProgressMonitor progressMonitor)
        {
            projectTreeModel.Project.TestPackageConfig.AssemblyFiles.Remove(fileName);
        }

        public void SaveFilterSet(string filterName, FilterSet<ITest> filterSet, IProgressMonitor progressMonitor)
        {
            foreach (FilterInfo filterInfo in TestFilters)
            {
                if (filterInfo.FilterName != filterName)
                    continue;
                filterInfo.Filter = filterSet.ToFilterSetExpr();
                return;
            }
            TestFilters.Add(new FilterInfo(filterName, filterSet.ToFilterSetExpr()));
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
                var projectUtils = new ProjectUtils(fileSystem, xmlSerializer);
                Project project;

                try
                {
                    project = projectUtils.LoadProject(projectName);
                }
                catch (Exception ex)
                {
                    unhandledExceptionPolicy.Report("Error loading project file", ex);
                    project = new Project();
                }

                progressMonitor.Worked(50);

                projectTreeModel.FileName = projectName;
                projectTreeModel.Project = project;

                assemblyWatcher.Clear();
                assemblyWatcher.Add(project.TestPackageConfig.AssemblyFiles);

                PublishUpdates();
            }
        }

        public void NewProject(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Creating new project", 100))
            {
                projectTreeModel.FileName = Paths.DefaultProject;
                projectTreeModel.Project = new Project();

                assemblyWatcher.Clear();

                PublishUpdates();
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
            UserOptions userOptions = new UserOptions
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

            ProjectUtils projectUtils = new ProjectUtils(fileSystem, xmlSerializer);
            projectUtils.SaveProject(projectTreeModel.Project, projectName);
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

                TestFilters.Clear();
                foreach (var filterInfo in projectTreeModel.Project.TestFilters)
                    TestFilters.Add(filterInfo);

                HintDirectories.Clear();
                foreach (var hintDirectory in TestPackageConfig.HintDirectories)
                    HintDirectories.Add(hintDirectory);

                TestRunnerExtensions.Clear();
                foreach (var testRunnerExtension in projectTreeModel.Project.TestRunnerExtensions)
                    TestRunnerExtensions.Add(testRunnerExtension);

                OnPropertyChanged(new PropertyChangedEventArgs("TestPackageConfig"));

                updating = false;
            }, null);
        }
    }
}
