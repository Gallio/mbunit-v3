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
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models.Interfaces;
using Gallio.Icarus.Remoting;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner.Projects;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Utilities;

namespace Gallio.Icarus.Controllers
{
    public class ProjectController : NotifyController, IProjectController
    {
        private readonly IProjectTreeModel projectTreeModel;
        private readonly IOptionsController optionsController;
        private readonly IFileSystem fileSystem;
        private readonly IXmlSerializer xmlSerializer;
        private readonly BindingList<FilterInfo> testFilters;
        private readonly List<FilterInfo> testFiltersList = new List<FilterInfo>();
        private readonly BindingList<string> hintDirectories;
        private readonly List<string> hintDirectoriesList = new List<string>();
        private readonly AssemblyWatcher assemblyWatcher = new AssemblyWatcher();
        private string currentProjectName = Paths.DefaultProject;

        public event EventHandler<AssemblyChangedEventArgs> AssemblyChanged;

        public IProjectTreeModel Model
        {
            get { return projectTreeModel; }
        }

        public TestPackageConfig TestPackageConfig
        {
            get { return projectTreeModel.Project.TestPackageConfig; }
        }

        public BindingList<FilterInfo> TestFilters
        {
            get { return testFilters; }
        }

        public BindingList<string> HintDirectories
        {
            get { return hintDirectories; }
        }

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
            get;
            set;
        }

        public ProjectController(IProjectTreeModel projectTreeModel, IOptionsController optionsController, 
            IFileSystem fileSystem, IXmlSerializer xmlSerializer)
        {
            this.projectTreeModel = projectTreeModel;
            this.optionsController = optionsController;
            this.fileSystem = fileSystem;
            this.xmlSerializer = xmlSerializer;

            testFilters = new BindingList<FilterInfo>(testFiltersList);
            testFilters.ListChanged += testFilters_ListChanged;

            hintDirectories = new BindingList<string>(hintDirectoriesList);
            hintDirectories.ListChanged += hintDirectories_ListChanged;

            assemblyWatcher.AssemblyChangedEvent += assemblyWatcher_AssemblyChangedEvent;

            // default tree view category
            TreeViewCategory = "Namespace";
            CollapsedNodes = new List<string>();
        }

        private void testFilters_ListChanged(object sender, ListChangedEventArgs e)
        {
            projectTreeModel.Project.TestFilters.Clear();
            projectTreeModel.Project.TestFilters.AddRange(testFiltersList);
        }

        private void hintDirectories_ListChanged(object sender, ListChangedEventArgs e)
        {
            projectTreeModel.Project.TestPackageConfig.HintDirectories.Clear();
            projectTreeModel.Project.TestPackageConfig.HintDirectories.AddRange(hintDirectoriesList);
        }

        private void assemblyWatcher_AssemblyChangedEvent(string fullPath)
        {
            string assemblyName = Path.GetFileNameWithoutExtension(fullPath);
            EventHandlerUtils.SafeInvoke(AssemblyChanged, this, new AssemblyChangedEventArgs(assemblyName));
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
                testFilters.Remove(filterInfo);
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
            foreach (FilterInfo filterInfo in testFilters)
            {
                if (filterInfo.FilterName != filterName)
                    continue;
                filterInfo.Filter = filterSet.ToFilterSetExpr();
                return;
            }
            testFilters.Add(new FilterInfo(filterName, filterSet.ToFilterSetExpr()));
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
                string projectUserOptionsFile = projectName + ".user";
                if (!fileSystem.FileExists(projectUserOptionsFile))
                    return;

                UserOptions userOptions = xmlSerializer.LoadFromXml<UserOptions>(projectUserOptionsFile);
                TreeViewCategory = userOptions.TreeViewCategory;
                OnPropertyChanged(new PropertyChangedEventArgs("TreeViewCategory"));
                CollapsedNodes = userOptions.CollapsedNodes;
            }
        }

        private void LoadProjectFile(IProgressMonitor progressMonitor, string projectName)
        {
            using (progressMonitor.BeginTask("Loading project file", 100))
            {
                ProjectUtils projectUtils = new ProjectUtils(fileSystem, xmlSerializer);
                Project project = projectUtils.LoadProject(projectName);

                progressMonitor.Worked(50);

                currentProjectName = projectName;
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
                    projectName = currentProjectName;

                // create folder (if necessary)
                string dir = Path.GetDirectoryName(projectName);
                if (!fileSystem.DirectoryExists(dir))
                    fileSystem.CreateDirectory(dir);
                progressMonitor.Worked(10);

                ProjectUtils projectUtils = new ProjectUtils(fileSystem, xmlSerializer);
                projectUtils.SaveProject(projectTreeModel.Project, projectName);
                progressMonitor.Worked(50);

                optionsController.RecentProjects.Add(projectName);

                progressMonitor.SetStatus("Saving user options");
                string projectUserOptionsFile = projectName + ".user";
                UserOptions userOptions = new UserOptions
                                              {
                                                  TreeViewCategory = TreeViewCategory,
                                                  CollapsedNodes = CollapsedNodes
                                              };
                progressMonitor.Worked(10);

                xmlSerializer.SaveToXml(userOptions, projectUserOptionsFile);
            }
        }

        public void RefreshTree(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Refreshing reports list", 100))
                projectTreeModel.Refresh();
        }

        private void PublishUpdates()
        {
            testFilters.ListChanged -= testFilters_ListChanged;
            testFilters.Clear();
            foreach (FilterInfo filterInfo in projectTreeModel.Project.TestFilters)
                testFilters.Add(filterInfo);
            testFilters.ListChanged += testFilters_ListChanged;

            hintDirectories.ListChanged -= hintDirectories_ListChanged;
            hintDirectories.Clear();
            foreach (string hintDirectory in TestPackageConfig.HintDirectories)
                hintDirectories.Add(hintDirectory);
            hintDirectories.ListChanged += hintDirectories_ListChanged;

            OnPropertyChanged(new PropertyChangedEventArgs("TestPackageConfig"));
        }
    }
}
