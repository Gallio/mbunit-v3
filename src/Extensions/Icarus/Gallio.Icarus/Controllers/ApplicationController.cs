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
using Gallio.Common.IO;
using Gallio.Common.Policies;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Events;
using Gallio.UI.Controls;
using Gallio.Icarus.Helpers;
using Gallio.Runner.Projects;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Common.Policies;
using Gallio.UI.DataBinding;
using Gallio.UI.Events;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.Controllers
{
    public class ApplicationController : NotifyController, IApplicationController, 
        Handles<RunStarted>, Handles<RunFinished>, Handles<ExploreFinished>, 
        Handles<TestsFailed>
    {
        private string projectFileName = string.Empty;

        private readonly IFileSystem fileSystem;
        private readonly IOptionsController optionsController;
        private readonly ITaskManager taskManager;
        private readonly ITestController testController;
        private readonly IProjectController projectController;
        private readonly IUnhandledExceptionPolicy unhandledExceptionPolicy;
        private readonly IEventAggregator eventAggregator;
        private readonly ICommandFactory commandFactory;

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

        public IcarusArguments Arguments { get; set; }

        public Observable<bool> CanRunTests { get; private set; }

        public event EventHandler ExploreFinished;

        public event EventHandler RunStarted = (s, e) => { };
        public event EventHandler RunFinished = (s, e) => { };
        public event EventHandler TestsFailed = (s, e) => { };

        // FIXME: too many dependencies!
        public ApplicationController(IOptionsController optionsController, IFileSystem fileSystem, 
            ITaskManager taskManager, ITestController testController, IProjectController projectController, 
            IUnhandledExceptionPolicy unhandledExceptionPolicy, IEventAggregator eventAggregator,
            ICommandFactory commandFactory)
        {
            this.optionsController = optionsController;
            this.fileSystem = fileSystem;
            this.taskManager = taskManager;
            this.testController = testController;
            this.projectController = projectController;
            this.unhandledExceptionPolicy = unhandledExceptionPolicy;
            this.eventAggregator = eventAggregator;
            this.commandFactory = commandFactory;

            optionsController.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "RecentProjects")
                    OnPropertyChanged(new PropertyChangedEventArgs("RecentProjects"));
            };

            CanRunTests = new Observable<bool>();
        }

        public void Load()
        {
            HandleArguments();
        }

        private void HandleArguments()
        {
            if (Arguments.Files.Length > 0)
            {
                AddFiles();
                return;
            }
            if (optionsController.RestorePreviousSettings && optionsController.RecentProjects.Count > 0)
            {
                var projectName = optionsController.RecentProjects.Items[0];
                if (fileSystem.FileExists(projectName))
                {
                    OpenProject(projectName);
                    return;
                }
            }
            NewProject();
        }

        private void AddFiles()
        {
            var files = new List<string>();
            foreach (var file in Arguments.Files)
            {
                if (!fileSystem.FileExists(file))
                    continue;

                if (Path.GetExtension(file) == TestProject.Extension)
                {
                    OpenProject(file);
                    return;
                }
                files.Add(file);
            }

            var command = commandFactory.CreateAddFilesCommand(files);
            taskManager.QueueTask(command);
        }

        public void OpenProject(string projectName)
        {
            Title = projectName;

            var command = commandFactory.CreateOpenProjectCommand(projectName);
            taskManager.QueueTask(command);
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
            // TODO: DRY, this shouldn't be necessary here
            Title = Paths.DefaultProject;
            var cmd = new NewProjectCommand(projectController, testController);
            taskManager.QueueTask(cmd);
        }

        public void Shutdown()
        {
            eventAggregator.Send(this, new ApplicationShutdown());
            optionsController.Save();
            SaveProject(false);
        }

        public void Handle(RunStarted @event)
        {
            EventHandlerPolicy.SafeInvoke(RunStarted, this, 
                System.EventArgs.Empty);
        }

        public void Handle(RunFinished @event)
        {
            EventHandlerPolicy.SafeInvoke(RunFinished, this, 
                System.EventArgs.Empty);
        }

        public void Handle(ExploreFinished @event)
        {
            EventHandlerPolicy.SafeInvoke(ExploreFinished, this,
                System.EventArgs.Empty);
        }

        public void Handle(TestsFailed @event)
        {
            TestsFailed(this, System.EventArgs.Empty);
        }
    }
}
