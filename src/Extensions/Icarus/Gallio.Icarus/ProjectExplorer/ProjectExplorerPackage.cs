// Copyright 2011 Gallio Project - http://www.gallio.org/
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

using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.WindowManager;
using Gallio.UI.Menus;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.ProjectExplorer
{
    public class ProjectExplorerPackage : IPackage
    {
        private readonly IWindowManager windowManager;
    	private readonly IProjectController projectController;
    	private readonly IReportController reportController;
    	private readonly ITaskManager taskManager;
    	private readonly ICommandFactory commandFactory;

    	public const string WindowId = "Gallio.Icarus.ProjectExplorer";

        public ProjectExplorerPackage(IWindowManager windowManager, IProjectController projectController, IReportController reportController, 
			ITaskManager taskManager, ICommandFactory commandFactory)
        {
        	this.windowManager = windowManager;
        	this.projectController = projectController;
        	this.reportController = reportController;
        	this.taskManager = taskManager;
        	this.commandFactory = commandFactory;
        }

    	public void Load()
        {
            RegisterWindow();
            AddMenuItem();
        }

        private void RegisterWindow()
        {
            windowManager.Register(WindowId, () =>
            {
                var view = new ProjectExplorerView(projectController, reportController, taskManager, commandFactory, windowManager);
                windowManager.Add(WindowId, view, ProjectExplorerResources.Project_Explorer, ProjectExplorerResources.ProjectExplorerIcon);
            }, Location.Left);
        }

        private void AddMenuItem()
        {
            windowManager.MenuManager.Add("View", () => new MenuCommand
            {
                Command = new DelegateCommand(pm => windowManager.Show(WindowId)),
				Text = ProjectExplorerResources.Project_Explorer,
				Image = ProjectExplorerResources.ProjectExplorerIcon.ToBitmap(),
            });
        }

        public void Dispose() { }         
    }
}
