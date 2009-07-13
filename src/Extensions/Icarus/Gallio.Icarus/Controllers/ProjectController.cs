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
using Gallio.Model.Schema;
using Gallio.Runner.Projects;
using Gallio.Runner.Projects.Schema;
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

        public TestPackage TestPackage
        {
            get { return projectTreeModel.TestProject.TestPackage; }
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
            get { return projectTreeModel.TestProject.ReportDirectory; }
        }

        public string ReportNameFormat
        {
            get { return projectTreeModel.TestProject.ReportNameFormat; }
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

                projectTreeModel.TestProject.ClearTestFilters();
                GenericCollectionUtils.ForEach(TestFilters, x => projectTreeModel.TestProject.AddTestFilter(x));
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

                GenericCollectionUtils.ForEach(validAssemblies, x => projectTreeModel.TestProject.TestPackage.AddFile(new FileInfo(x)));
                progressMonitor.Worked(1);
                assemblyWatcher.Add(validAssemblies);
            }
        }

        public void DeleteFilter(FilterInfo filterInfo, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Deleting filter", 1))
                TestFilters.Remove(filterInfo);
        }

        public FilterSet<ITestDescriptor> GetFilterSet(string filterName, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Getting filter", 1))
            {
                foreach (FilterInfo filterInfo in projectTreeModel.TestProject.TestFilters)
                {
                    if (filterInfo.FilterName == filterName)
                        return FilterUtils.ParseTestFilterSet(filterInfo.FilterExpr);
                }
                return null;
            }
        }

        public void RemoveAllAssemblies(IProgressMonitor progressMonitor)
        {
            projectTreeModel.TestProject.TestPackage.ClearFiles();
        }

        public void RemoveAssembly(string fileName, IProgressMonitor progressMonitor)
        {
            projectTreeModel.TestProject.TestPackage.RemoveFile(new FileInfo(fileName));
        }

        public void SaveFilterSet(string filterName, FilterSet<ITestDescriptor> filterSet, IProgressMonitor progressMonitor)
        {
            foreach (FilterInfo filterInfo in TestFilters)
            {
                if (filterInfo.FilterName != filterName)
                    continue;
                filterInfo.FilterExpr = filterSet.ToFilterSetExpr();
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
                var testProjectManager = new DefaultTestProjectManager(fileSystem, xmlSerializer);
                TestProject testProject;

                try
                {
                    testProject = testProjectManager.LoadProject(new FileInfo(projectName));
                }
                catch (Exception ex)
                {
                    unhandledExceptionPolicy.Report("Error loading project file.  The project file may not be compatible with this version of Gallio.", ex);
                    testProject = new TestProject();
                }

                progressMonitor.Worked(50);

                projectTreeModel.FileName = projectName;
                projectTreeModel.TestProject = testProject;

                assemblyWatcher.Clear();
                GenericCollectionUtils.ForEach(testProject.TestPackage.Files, x => assemblyWatcher.Add(x.FullName));

                PublishUpdates();
            }
        }

        public void NewProject(IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Creating new project", 100))
            {
                projectTreeModel.FileName = Paths.DefaultProject;
                projectTreeModel.TestProject = new TestProject();

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

            var testProjectManager = new DefaultTestProjectManager(fileSystem, xmlSerializer);
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

                TestFilters.Clear();
                foreach (var filterInfo in projectTreeModel.TestProject.TestFilters)
                    TestFilters.Add(filterInfo);

                HintDirectories.Clear();
                foreach (var hintDirectory in TestPackage.HintDirectories)
                    HintDirectories.Add(hintDirectory.ToString());

                TestRunnerExtensions.Clear();
                foreach (var testRunnerExtension in projectTreeModel.TestProject.TestRunnerExtensionSpecifications)
                    TestRunnerExtensions.Add(testRunnerExtension);

                OnPropertyChanged(new PropertyChangedEventArgs("TestPackageConfig"));

                updating = false;
            }, null);
        }
    }
}
