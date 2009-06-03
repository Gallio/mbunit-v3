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
using System.Drawing;
using System.IO;
using Gallio.Common.IO;
using Gallio.Common.Policies;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Controls;
using Gallio.Icarus.Helpers;
using Gallio.Runner.Projects;
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Common.Policies;
using Gallio.UI.Progress;

namespace Gallio.Icarus.Controllers
{
    internal class ApplicationController : NotifyController, IApplicationController
    {
        private readonly IcarusArguments arguments;
        private string projectFileName = string.Empty;

        private readonly IFileSystem fileSystem;
        private readonly IOptionsController optionsController;
        private readonly ITaskManager taskManager;
        private readonly ITestController testController;
        private readonly IProjectController projectController;
        private readonly IUnhandledExceptionPolicy unhandledExceptionPolicy;

        public event EventHandler<AssemblyChangedEventArgs> AssemblyChanged;
        
        public string Title
        {
            get
            {
                return string.IsNullOrEmpty(projectFileName) ? Properties.Resources.ApplicationName :
                    string.Format("{0} - {1}", Path.GetFileNameWithoutExtension(projectFileName), 
                    Properties.Resources.ApplicationName);
            }
            set
            {
                projectFileName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ProjectFileName"));
            }
        }

        public ToolStripMenuItem[] RecentProjects
        {
            get 
            {
                var menuListHelper = new MenuListHelper(optionsController, fileSystem);
                return menuListHelper.GetRecentProjectsMenuList(OpenProject);
            }
        }

        public Size Size
        {
            get { return optionsController.Size; }
            set { optionsController.Size = value; }
        }

        public Point Location
        {
            get { return optionsController.Location; }
            set { optionsController.Location = value; }
        }

        public bool FailedTests
        {
            get { return testController.FailedTests; }
        }

        public ApplicationController(IcarusArguments arguments, IServiceLocator serviceLocator)
        {
            if (arguments == null) 
                throw new ArgumentNullException("arguments");

            if (serviceLocator == null)
                throw new ArgumentNullException("serviceLocator");

            this.arguments = arguments;

            optionsController = serviceLocator.Resolve<IOptionsController>();
            optionsController.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "RecentProjects")
                    OnPropertyChanged(new PropertyChangedEventArgs("RecentProjects"));
            };

            fileSystem = serviceLocator.Resolve<IFileSystem>();
            taskManager = serviceLocator.Resolve<ITaskManager>();
            testController = serviceLocator.Resolve<ITestController>();
            
            projectController = serviceLocator.Resolve<IProjectController>();
            projectController.AssemblyChanged += (sender, e) => EventHandlerPolicy.SafeInvoke(AssemblyChanged, this, e);
            
            unhandledExceptionPolicy = serviceLocator.Resolve<IUnhandledExceptionPolicy>();
        }

        public void Load()
        {
            LoadPackages();

            HandleArguments();
        }

        private void HandleArguments()
        {
            var assemblyFiles = new List<string>();
            if (arguments.Files.Length > 0)
            {
                foreach (var assembly in arguments.Files)
                {
                    if (!fileSystem.FileExists(assembly))
                        continue;

                    if (Path.GetExtension(assembly) == Project.Extension)
                    {
                        OpenProject(assembly);
                        return;
                    }
                    assemblyFiles.Add(assembly);
                }
                var cmd = new AddAssembliesCommand(projectController, testController) 
                    { AssemblyFiles = assemblyFiles };
                taskManager.QueueTask(cmd);
            }
            else if (optionsController.RestorePreviousSettings && optionsController.RecentProjects.Count > 0)
            {
                string projectName = optionsController.RecentProjects.Items[0];
                if (File.Exists(projectName))
                    OpenProject(projectName);
            }
        }

        private void LoadPackages()
        {
            var serviceLocator = RuntimeAccessor.ServiceLocator;
            foreach (var package in serviceLocator.ResolveAll<IPackage>())
            {
                try
                {
                    package.Load(serviceLocator);
                }
                catch (Exception ex)
                {
                    unhandledExceptionPolicy.Report("Error loading package", ex);
                }
            }
        }

        private void UnloadPackages()
        {
            var serviceLocator = RuntimeAccessor.ServiceLocator;
            foreach (var package in serviceLocator.ResolveAll<IPackage>())
            {
                try
                {
                    package.Unload();
                }
                catch (Exception ex)
                {
                    unhandledExceptionPolicy.Report("Error unloading package", ex);
                }
            }
        }

        public void OpenProject(string projectName)
        {
            Title = projectName;

            var openProjectCommand = new OpenProjectCommand(testController, projectController, projectName);
            taskManager.QueueTask(openProjectCommand);
        }

        public void SaveProject(bool queueTask)
        {
            var cmd = new SaveProjectCommand(projectController) { FileName = projectFileName };
            if (queueTask)
            {
                taskManager.QueueTask(cmd);
            }
            else
            {
                // we're shutting down, so eat any errors
                try
                {
                    cmd.Execute(NullProgressMonitor.CreateInstance());
                }
                catch (Exception ex)
                {
                    unhandledExceptionPolicy.Report("Error saving project", ex);
                }
            }
        }

        public void NewProject()
        {
            Title = string.Empty;

            var cmd = new NewProjectCommand(projectController, testController);
            taskManager.QueueTask(cmd);
        }
    }
}
